using ERService.Business;
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

namespace ERService.OrderModule.ViewModels
{
    public class OrderViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        private string _cost;
        private string _externalNumber;
        private Customer _customer;
        private Hardware _hardware;
        private OrderWrapper _order;
        private Blob _selectedAttachment;
        private OrderStatus _selectedOrderStatus;
        private OrderType _selectedOrderType;
        private IRegionManager _regionManager;
        private IBlobRepository _blobRepository;
        private IOrderRepository _orderRepository;
        private IOrderTypeRepository _typeRepository;
        private IOrderStatusRepository _statusRepository;
        private IRegionNavigationService _navigationService;
        private INumerationRepository _numerationRepository;

        private bool _wizardMode;
        public OrderViewModel(IRegionManager regionManager, IOrderRepository orderRepository, IOrderTypeRepository typeRepository,
            IOrderStatusRepository statusRepository, IBlobRepository blobRepository, IEventAggregator eventAggregator,
            INumerationRepository numerationRepository) : base(eventAggregator)
        {
            _orderRepository = orderRepository;
            _typeRepository = typeRepository;
            _statusRepository = statusRepository;
            _blobRepository = blobRepository;
            _numerationRepository = numerationRepository;
            _regionManager = regionManager;

            OrderStatuses = new ObservableCollection<OrderStatus>();
            OrderTypes = new ObservableCollection<OrderType>();
            Attachments = new ObservableCollection<Blob>();

            AddAttachmentCommand = new DelegateCommand(OnAddAttachmentExecute);
            RemoveAttachmentCommand = new DelegateCommand(OnRemoveAttachmentExecute, OnRemoveAttachmentCanExecute);
            GoBackCommand = new DelegateCommand(OnGoBackExecute);
        }

        public DelegateCommand AddAttachmentCommand { get; private set; }

        public ObservableCollection<Blob> Attachments { get; private set; }

        public string Cost { get => _cost; set { SetProperty(ref _cost, value); } }

        public Customer Customer { get => _customer; set { SetProperty(ref _customer, value); } }

        public string ExternalNumber { get => _externalNumber; set { SetProperty(ref _externalNumber, value); } }

        public DelegateCommand GoBackCommand { get; private set; }

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

        //TODO: Move to Infrastructure helpers
        private byte[] GetFileBinary(string fileName)
        {
            byte[] fileBytes;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                fileBytes = new byte[fs.Length];
                fs.Read(fileBytes, 0, Convert.ToInt32(fs.Length));
            }

            return fileBytes;
        }

        private void OnAddAttachmentExecute()
        {
            //TODO: Make open file dialog service
            var openFileDialog = new OpenFileDialog();
            var attachment = new Blob();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileBinary = GetFileBinary(openFileDialog.FileName);
                attachment.Data = fileBinary;
                attachment.FileName = openFileDialog.SafeFileName;
                attachment.Size = fileBinary.Length;
                //attachment.Order = Order.Model;

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

        public bool KeepAlive => false;

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
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

        protected override bool OnCancelEditCanExecute()
        {
            return true;
        }

        protected override void OnCancelEditExecute()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
        }

        protected override bool OnSaveCanExecute()
        {
            return true;
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

        //TODO: Refactor?
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

        private void SetTitle()
        {
            Title = $"{Order.Number}";
        }

        public override Task LoadAsync()
        {
            throw new NotImplementedException();
        }

        #endregion Overrides
    }
}