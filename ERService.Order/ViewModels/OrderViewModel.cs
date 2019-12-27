﻿using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.OrderModule.Data.Repository;
using ERService.OrderModule.Repository;
using ERService.OrderModule.Wrapper;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using ERService.OrderModule.OrderNumeration;
using ERService.Infrastructure.Dialogs;
using ERService.RBAC;
using ERService.TemplateEditor.Data.Repository;
using ERService.CustomerModule.Wrapper;
using ERService.HardwareModule;
using ERService.Infrastructure.Interfaces;
using ERService.Infrastructure.Helpers;

namespace ERService.OrderModule.ViewModels
{
    public class OrderViewModel : DetailViewModelBase
    {
        private string _cost;
        private string _externalNumber;
        private Customer _customer;
        private Hardware _hardware;
        private OrderWrapper _order;
        private Blob _selectedAttachment;
        private OrderType _selectedOrderType;
        private OrderStatus _selectedOrderStatus;
        private IRegionManager _regionManager;
        private IOrderRepository _orderRepository;
        private readonly IRBACManager _rBACManager;
        private IOrderTypeRepository _typeRepository;
        private IOrderStatusRepository _statusRepository;
        private INumerationRepository _numerationRepository;
        private IRegionNavigationService _navigationService;
        private readonly IPrintTemplateRepository _templateRepository;
        private readonly ISettingsManager _settingsManager;
        private bool _wizardMode;

        public OrderViewModel(IRegionManager regionManager, IOrderRepository orderRepository, IOrderTypeRepository typeRepository,
            IOrderStatusRepository statusRepository, IEventAggregator eventAggregator,
            INumerationRepository numerationRepository, IMessageDialogService messageDialogService, IRBACManager rBACManager,
            IPrintTemplateRepository templateRepository, ISettingsManager settingsManager) : base(eventAggregator, messageDialogService)
        {
            _orderRepository = orderRepository;
            _typeRepository = typeRepository;
            _statusRepository = statusRepository;
            _numerationRepository = numerationRepository;
            _rBACManager = rBACManager;
            _templateRepository = templateRepository;
            _settingsManager = settingsManager;
            _regionManager = regionManager;

            OrderStatuses = new ObservableCollection<OrderStatus>();
            OrderTypes = new ObservableCollection<OrderType>();
            Attachments = new ObservableCollection<Blob>();
            PrintTemplates = new ObservableCollection<PrintTemplate>();

            AddAttachmentCommand = new DelegateCommand(OnAddAttachmentExecute);
            RemoveAttachmentCommand = new DelegateCommand(OnRemoveAttachmentExecute, OnRemoveAttachmentCanExecute);
            GoBackCommand = new DelegateCommand(OnGoBackExecute);
            PrintCommand = new DelegateCommand<object>(OnPrintExecute);
        }

        private async void OnPrintExecute(object parameter)
        {
            var template = parameter as PrintTemplate;
            if (template != null)
            {
                var companyConfig = await _settingsManager.GetConfigAsync(ConfigNames.CompanyInfoConfig);
                var parameters = new NavigationParameters();
                parameters.Add("ID", template.Id);
                parameters.Add("IsReadOnly", true);
                parameters.Add("IsToolbarVisible", false);
                parameters.Add("ModelWrappers", new object[] 
                { 
                    new CustomerWrapper(Customer), 
                    new HardwareWrapper(Hardware), 
                    Order,
                    companyConfig,
                    new AddressWrapper(Customer.CustomerAddresses.FirstOrDefault())
                });

                _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.PrintTemplateEditorView, parameters);
            }
        }

        public DelegateCommand AddAttachmentCommand { get; private set; }

        public ObservableCollection<Blob> Attachments { get; private set; }
        public ObservableCollection<PrintTemplate> PrintTemplates { get; private set; }

        public string Cost { get => _cost; set { SetProperty(ref _cost, value); } }

        public Customer Customer { get => _customer; set { SetProperty(ref _customer, value); } }

        public string ExternalNumber { get => _externalNumber; set { SetProperty(ref _externalNumber, value); } }

        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand<object> PrintCommand { get; }

        public Hardware Hardware { get => _hardware; set { SetProperty(ref _hardware, value); } }

        public OrderWrapper Order { get => _order; set { SetProperty(ref _order, value); } }

        public ObservableCollection<OrderStatus> OrderStatuses { get; private set; }

        public ObservableCollection<OrderType> OrderTypes { get; private set; }

        public DelegateCommand RemoveAttachmentCommand { get; private set; }

        public Blob SelectedAttachment
        {
            get { return _selectedAttachment; }
            set { SetProperty(ref _selectedAttachment, value); RemoveAttachmentCommand.RaiseCanExecuteChanged(); }
        }

        public OrderStatus SelectedOrderStatus
        {
            get { return _selectedOrderStatus; }
            set
            {
                SetProperty(ref _selectedOrderStatus, value);
                Order.Model.OrderStatusId = value.Id;
            }
        }

        public OrderType SelectedOrderType
        {
            get { return _selectedOrderType; }
            set
            {
                SetProperty(ref _selectedOrderType, value);
                Order.Model.OrderTypeId = value.Id;
            }
        }

        public bool WizardMode { get => _wizardMode; set { SetProperty(ref _wizardMode, value); } }

        private void OnAddAttachmentExecute()
        {
            //TODO: Make open file dialog service
            var openFileDialog = new OpenFileDialog();
            var attachment = new Blob();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileBinary = FileUtils.GetFileBinary(openFileDialog.FileName);
                attachment.Data = fileBinary;
                attachment.FileName = openFileDialog.SafeFileName;
                attachment.Size = fileBinary.Length;
                attachment.Description = $"File attachment for order: {Order.Number}";
                attachment.Checksum = Cryptography.CalculateMD5(openFileDialog.FileName);

                Attachments.Add(attachment);
                Order.Model.Attachments.Add(attachment);
            }
        }

        private bool OnRemoveAttachmentCanExecute()
        {
            return SelectedAttachment != null;
        }

        private void OnRemoveAttachmentExecute()
        {
            Order.Model.Attachments.Remove(SelectedAttachment);
            Attachments.Remove(SelectedAttachment);
        }

        #region Navigation

        public override bool KeepAlive => false;

        public override void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
            WizardMode = navigationContext.Parameters.GetValue<bool>("Wizard");

            if (WizardMode)
            {
                //TODO: REFACTOR, czy możemy użyć tutaj budowniczego np. OrderBuilder?
                var customer = navigationContext.Parameters.GetValue<Customer>("Customer");
                if (customer != null)
                    Customer = customer;

                var hardware = navigationContext.Parameters.GetValue<Hardware>("Hardware");
                if (hardware != null)
                    Hardware = hardware;

                await LoadAsync(Guid.Empty);

                //Order.DateAdded = DateTime.Now;
                Order.Model.Hardwares.Clear();
                Order.Model.Hardwares.Add(Hardware);
                if (Customer.Id != Guid.Empty)
                {
                    Order.Model.CustomerId = Customer.Id;
                }
                else
                {
                    Order.Model.Customer = Customer;
                }
            }
            else
            {
                var id = navigationContext.Parameters.GetValue<string>("ID");
                if (!String.IsNullOrWhiteSpace(id))
                {
                    await LoadAsync(Guid.Parse(id));
                }
            }

            if (!_rBACManager.LoggedUserHasPermission(AclVerbNames.CanEditOrder))
                IsReadOnly = true;
        }

        private void OnGoBackExecute()
        {
            _navigationService.Journal.GoBack();
        }

        #endregion Navigation

        #region Overrides

        //TODO: Refactor?
        public override async Task LoadAsync(Guid id)
        {
            var order = id != Guid.Empty ? await _orderRepository.GetByIdAsync(id) : await GetNewDetail();

            //ustaw Id dla detailviewmodel, taki sam jak pobranego modelu z repo
            ID = id;

            InitializeOrder(order);
            InitializeHarware();
            InitializeCustomer();
            InitializeComboBoxes();
            InitializeAttachments();
            InitializePrintTemplates();
        }

        private async void InitializePrintTemplates()
        {
            PrintTemplates.Clear();
            var templates = await _templateRepository.GetAllAsync();
            foreach (var template in templates)
            {
                PrintTemplates.Add(template);
            }
        }

        private void InitializeCustomer()
        {
            if (WizardMode) return;
            Customer = Order.Model.Customer;
        }

        private void InitializeHarware()
        {
            if (WizardMode) return;
            Hardware = Order.Model.Hardwares.FirstOrDefault();
        }

        protected override bool OnSaveCanExecute()
        {
            return Order != null && !Order.HasErrors && HasChanges && !WizardMode;
        }

        protected async override void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_orderRepository.SaveAsync, () =>
            {
                HasChanges = _orderRepository.HasChanges(); // Po zapisie ustawiamy flagę na false jeśli nie ma zmian w repo
                ID = Customer.Id; //odśwież Id

                //Powiadom agregator eventów, że zapisano
                //RaiseDetailSavedEvent(Customer.Id, $"{Customer.FirstName} {Customer.LastName}");
                _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            });
        }

        //TODO: Refactor? Może zrobić tu fabrykę w bazowym?
        private async Task<Order> GetNewDetail()
        {
            var numeration = await _numerationRepository.FindByAsync(n => n.Name == "default");

            var order = new Order();
            order.Number = OrderNumberGenerator.GetNumberFromPattern(numeration.FirstOrDefault().Pattern);
            order.DateAdded = DateTime.Now;
            order.DateEnded = DateTime.Now.AddDays(14);
            _orderRepository.Add(order);

            return order;
        }

        private void InitializeAttachments()
        {
            if (WizardMode) return;
            Attachments.Clear();
            foreach (var att in Order.Model.Attachments)
            {
                Attachments.Add(att);
            }
        }

        private void InitializeComboBoxes()
        {
            LoadOrderStatusesAsync();
            LoadOrderTypesAsync();
        }

        private void InitializeOrder(Order order)
        {
            //Opakowanie modelu detala w ModelWrapper aby korzystał z walidacji propertisów
            Order = new OrderWrapper(order);

            //Po załadowaniu detala i każdej zmianie propertisa sprawdzamy CanExecute Sejwa
            Order.PropertyChanged += ((sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _orderRepository.HasChanges();
                    ((DelegateCommand)CancelCommand).RaiseCanExecuteChanged();
                }

                //sprawdzamy czy zmieniony propert w modelu ma błędy i ustawiamy SaveButton
                if (args.PropertyName == nameof(Order.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

                if (args.PropertyName == nameof(Order.Number))
                {
                    SetTitle();
                }
            });
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            SetTitle();
        }

        private async void LoadOrderStatusesAsync()
        {
            OrderStatuses.Clear();
            var statuses = await _statusRepository.GetAllAsync();
            foreach (var status in statuses)
            {
                OrderStatuses.Add(status);
            }

            if (!WizardMode)
            {
                SelectedOrderStatus = Order.Model.OrderStatus;
            }
        }

        private async void LoadOrderTypesAsync()
        {
            OrderTypes.Clear();
            var types = await _typeRepository.GetAllAsync();
            foreach (var type in types)
            {
                OrderTypes.Add(type);
            }

            if (!WizardMode)
            {
                SelectedOrderType = Order.Model.OrderType;
            }
        }

        public override Task LoadAsync()
        {
            throw new NotImplementedException();
        }

        private void SetTitle()
        {
            Title = $"{Order.Number}";
        }

        #endregion Overrides
    }
}