using ERService.Business;
using ERService.HardwareModule.Data.Repository;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.OrderModule.Repository;
using ERService.OrderModule.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ERService.OrderModule.ViewModels
{
    public class OrderViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        private Customer _customer;
        private Hardware _hardware;
        private IRegionNavigationService _navigationService;
        private OrderWrapper _order;
        private IOrderRepository _orderRepository;
        private IRegionManager _regionManager;
        private OrderStatus _selectedOrderStatus;
        private OrderType _selectedOrderType;
        private IOrderStatusRepository _statusRepository;
        private IOrderTypeRepository _typeRepository;

        private bool _wizardMode;
        private string _cost;
        private string _externalNumber;

        public OrderViewModel(IRegionManager regionManager, IOrderRepository orderRepository, IOrderTypeRepository typeRepository,
            IOrderStatusRepository statusRepository, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _orderRepository = orderRepository;
            _typeRepository = typeRepository;
            _statusRepository = statusRepository;
            _regionManager = regionManager;

            OrderStatuses = new ObservableCollection<OrderStatus>();
            OrderTypes = new ObservableCollection<OrderType>();

            GoBackCommand = new DelegateCommand(OnGoBackExecute);
        }

        public string Cost { get => _cost; set { SetProperty(ref _cost, value); } }

        public string ExternalNumber { get => _externalNumber; set { SetProperty(ref _externalNumber, value); } }

        public Customer Customer { get => _customer; set { SetProperty(ref _customer, value); } }

        public DelegateCommand GoBackCommand { get; private set; }

        public Hardware Hardware { get => _hardware; set { SetProperty(ref _hardware, value); } }

        public OrderWrapper Order { get => _order; set { SetProperty(ref _order, value); } }

        public ObservableCollection<OrderStatus> OrderStatuses { get; private set; }

        public ObservableCollection<OrderType> OrderTypes { get; private set; }

        public OrderStatus SelectedOrderStatus
        {
            get { return _selectedOrderStatus; }
            set { SetProperty(ref _selectedOrderStatus, value); Order.Model.OrderStatusId = value.Id; }
        }

        public OrderType SelectedOrderType
        {
            get { return _selectedOrderType; }
            set { SetProperty(ref _selectedOrderType, value); Order.Model.OrderTypeId = value.Id; }
        }

        public bool WizardMode { get => _wizardMode; set { SetProperty(ref _wizardMode, value); } }

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
                if (customer != null)
                    Hardware = hardware;

                await LoadAsync(Guid.Empty);

                //Order.DateAdded = DateTime.Now;
                Order.Model.Hardwares.Clear();
                Order.Model.Hardwares.Add(Hardware);
                Order.Model.Customer = Customer;
            }
            else
            {
                //var id = navigationContext.Parameters.GetValue<string>("ID");
                //if (!String.IsNullOrWhiteSpace(id))
                //{
                //    await LoadAsync(Guid.Parse(id));
                //}
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
            var order = id != Guid.Empty ? await _orderRepository.GetByIdAsync(id) : GetNewDetail();

            //ustaw Id dla detailviewmodel, taki sam jak pobranego modelu z repo
            ID = id;

            InitializeOrder(order);
            InitializeComboBoxes();
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
                RaiseDetailSavedEvent(Customer.Id, $"{Customer.FirstName} {Customer.LastName}");
                _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            });
        }

        //TODO: Refactor?
        private Order GetNewDetail()
        {
            var order = new Order();
            order.DateAdded = DateTime.Now;
            order.DateEnded = DateTime.Now.AddDays(14);
            order.Number = OrderNumberGenerator.GetNext();
            _orderRepository.Add(order);

            return order;
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
        }

        private async void LoadOrderTypesAsync()
        {
            OrderTypes.Clear();
            var types = await _typeRepository.GetAllAsync();
            foreach (var type in types)
            {
                OrderTypes.Add(type);
            }
        }

        private void SetTitle()
        {
            Title = $"{Order.Number}";
        }

        #endregion Overrides
    }

    internal class OrderNumberGenerator
    {
        internal static string GetNext()
        {
            //TODO: Random number generator
            return new Random().Next().ToString();
        }
    }
}