using ERService.Business;
using ERService.CustomerModule.Wrapper;
using ERService.HardwareModule;
using ERService.HardwareModule.Data.Repository;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Helpers;
using ERService.Infrastructure.Interfaces;
using ERService.OrderModule.Data.Repository;
using ERService.OrderModule.OrderNumeration;
using ERService.OrderModule.Repository;
using ERService.OrderModule.Wrapper;
using ERService.RBAC;
using ERService.TemplateEditor.Data.Repository;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.OrderModule.ViewModels
{
    public class OrderViewModel : DetailViewModelBase
    {
        private readonly IHardwareTypeRepository _hardwareTypesRepository;
        private readonly IRBACManager _rBACManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IPrintTemplateRepository _templateRepository;
        private string _cost;
        private CustomerWrapper _customer;
        private string _externalNumber;
        private HardwareWrapper _hardware;
        private IRegionNavigationService _navigationService;
        private INumerationRepository _numerationRepository;
        private OrderWrapper _order;
        private IOrderRepository _orderRepository;
        private IRegionManager _regionManager;
        private Blob _selectedAttachment;
        private HardwareType _selectedHardwareType;
        private OrderStatus _selectedOrderStatus;
        private OrderType _selectedOrderType;
        private IOrderStatusRepository _statusRepository;
        private IOrderTypeRepository _typeRepository;

        //TODO: Za duży konstruktor, Dodać IOrderContext.
        public OrderViewModel(IRegionManager regionManager, IOrderRepository orderRepository, IOrderTypeRepository typeRepository,
            IOrderStatusRepository statusRepository, IEventAggregator eventAggregator, IHardwareTypeRepository hardwareTypesRepository,
            INumerationRepository numerationRepository, IMessageDialogService messageDialogService, IRBACManager rBACManager,
            IPrintTemplateRepository templateRepository, ISettingsManager settingsManager) : base(eventAggregator, messageDialogService)
        {
            _orderRepository = orderRepository;
            _typeRepository = typeRepository;
            _statusRepository = statusRepository;
            _hardwareTypesRepository = hardwareTypesRepository;
            _numerationRepository = numerationRepository;
            _rBACManager = rBACManager;
            _templateRepository = templateRepository;
            _settingsManager = settingsManager;
            _regionManager = regionManager;

            OrderStatuses = new ObservableCollection<OrderStatus>();
            OrderTypes = new ObservableCollection<OrderType>();
            HardwareTypes = new ObservableCollection<HardwareType>();
            Attachments = new ObservableCollection<Blob>();
            PrintTemplates = new ObservableCollection<PrintTemplate>();

            AddAttachmentCommand = new DelegateCommand(OnAddAttachmentExecute);
            RemoveAttachmentCommand = new DelegateCommand(OnRemoveAttachmentExecute, OnRemoveAttachmentCanExecute);
            PrintCommand = new DelegateCommand<object>(OnPrintExecute);
            ShowHardwareDetailFlyoutCommand = new DelegateCommand(OnShowHardwareFlyoutExecute);
            ShowCustomerDetailFlyoutCommand = new DelegateCommand(OnShowCustomerFlyoutExecute);
        }

        public DelegateCommand AddAttachmentCommand { get; private set; }

        public ObservableCollection<Blob> Attachments { get; private set; }

        public string Cost { get => _cost; set { SetProperty(ref _cost, value); } }

        public CustomerWrapper Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }

        public string ExternalNumber { get => _externalNumber; set { SetProperty(ref _externalNumber, value); } }

        public DelegateCommand GoBackCommand { get; private set; }

        public HardwareWrapper Hardware
        {
            get { return _hardware; }
            set { SetProperty(ref _hardware, value); }
        }

        public ObservableCollection<HardwareType> HardwareTypes { get; private set; }

        public OrderWrapper Order { get => _order; set { SetProperty(ref _order, value); } }

        public ObservableCollection<OrderStatus> OrderStatuses { get; private set; }

        public ObservableCollection<OrderType> OrderTypes { get; private set; }

        public DelegateCommand<object> PrintCommand { get; }

        public ObservableCollection<PrintTemplate> PrintTemplates { get; private set; }

        public DelegateCommand RemoveAttachmentCommand { get; private set; }

        public Blob SelectedAttachment
        {
            get { return _selectedAttachment; }
            set { SetProperty(ref _selectedAttachment, value); RemoveAttachmentCommand.RaiseCanExecuteChanged(); }
        }

        public HardwareType SelectedHardwareType
        {
            get { return _selectedHardwareType; }
            set { SetProperty(ref _selectedHardwareType, value); }
        }

        public OrderStatus SelectedOrderStatus
        {
            get { return _selectedOrderStatus; }
            set
            {
                SetProperty(ref _selectedOrderStatus, value);
                Order.Model.OrderStatusId = value?.Id;
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

        public DelegateCommand ShowCustomerDetailFlyoutCommand { get; }

        public DelegateCommand ShowHardwareDetailFlyoutCommand { get; }

        private void RaiseSideMenuButtonToggled(SideFlyouts flyout, Guid detailID, string viewName)
        {
            _eventAggregator
                                        .GetEvent<AfterSideMenuButtonToggled>()
                                        .Publish(new AfterSideMenuButtonToggledArgs
                                        {
                                            Flyout = flyout,
                                            DetailID = detailID,
                                            ViewName = viewName,
                                            IsReadOnly = true
                                        });
        }

        #region Navigation

        public override bool KeepAlive => false;

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            var id = navigationContext.Parameters.GetValue<Guid>("ID");
            if (id != null)
            {
                await LoadAsync(id);
            }

            if (!_rBACManager.LoggedUserHasPermission(AclVerbNames.CanEditOrder))
                IsReadOnly = true;
        }        

        #endregion Navigation

        #region Overrides
        public override async Task LoadAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            
            ID = id;

            InitializeComboBoxes();
            InitializePrintTemplates();
            InitializeOrder(order);
            InitializeHardware();
            InitializeCustomer();
            InitializeAttachments();
        }

        protected override void OnCancelEditExecute()
        {
            _navigationService.Journal.GoBack();
        }

        protected override bool OnSaveCanExecute()
        {
            return Order != null && !Order.HasErrors && HasChanges;
        }

        protected async override void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_orderRepository.SaveAsync, () =>
            {
                HasChanges = _orderRepository.HasChanges();
                ID = Order.Id;

                _navigationService.Journal.GoBack();
            });
        }

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
                    Customer,
                    Hardware,
                    Order,
                    companyConfig,
                    new AddressWrapper(Customer.Model.CustomerAddresses.FirstOrDefault())
                });

                _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.PrintTemplateEditorView, parameters);
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

        private void OnShowCustomerFlyoutExecute()
        {
            RaiseSideMenuButtonToggled(SideFlyouts.DetailFlyout, Customer.Id, ViewNames.CustomerFlyoutDetailView);
        }

        private void OnShowHardwareFlyoutExecute()
        {
            RaiseSideMenuButtonToggled(SideFlyouts.DetailFlyout, Hardware.Id, ViewNames.HardwareFlyoutDetailView);
        }

        private void InitializeCustomer()
        {
            Customer = new CustomerWrapper(Order.Model.Customer);
        }

        private void InitializeHardware()
        {
            var hardware = Order.Model.Hardwares.FirstOrDefault();
            if (hardware != null)
            {
                SelectedHardwareType = hardware.HardwareType;
                Hardware = new HardwareWrapper(hardware);
                Hardware.PropertyChanged += ((sender, args) =>
                {
                    if (!HasChanges)
                    {
                        HasChanges = _orderRepository.HasChanges();
                        CancelCommand.RaiseCanExecuteChanged();
                    }

                    SaveCommand.RaiseCanExecuteChanged();                    
                });
            }
        }

        private void InitializeOrder(Order order)
        {
            Order = new OrderWrapper(order);
            Order.PropertyChanged += ((sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _orderRepository.HasChanges();
                    CancelCommand.RaiseCanExecuteChanged();
                }
                
                if (args.PropertyName == nameof(Order.HasErrors))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }

                SaveCommand.RaiseCanExecuteChanged();                
            });

            SaveCommand.RaiseCanExecuteChanged();            
        }

        private void InitializeAttachments()
        {
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
            LoadHardwareTypesAsync();
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

        private async void LoadHardwareTypesAsync()
        {
            HardwareTypes.Clear();
            var types = await _hardwareTypesRepository.GetAllAsync();
            foreach (var type in types)
            {
                HardwareTypes.Add(type);
            }
        }

        private async void LoadOrderStatusesAsync()
        {
            OrderStatuses.Clear();
            var statuses = await _statusRepository.GetAllAsync();
            foreach (var status in statuses)
            {
                OrderStatuses.Add(status);
            }

            SelectedOrderStatus = Order.Model.OrderStatus;
        }

        private async void LoadOrderTypesAsync()
        {
            OrderTypes.Clear();
            var types = await _typeRepository.GetAllAsync();
            foreach (var type in types)
            {
                OrderTypes.Add(type);
            }

            SelectedOrderType = Order.Model.OrderType;
        }        

        #endregion Overrides
    }
}