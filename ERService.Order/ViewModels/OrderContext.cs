using ERService.Business;
using ERService.CustomerModule.Repository;
using ERService.CustomerModule.Wrapper;
using ERService.HardwareModule;
using ERService.HardwareModule.Data.Repository;
using ERService.HardwareModule.ViewModels;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.OrderModule.Data.Repository;
using ERService.OrderModule.OrderNumeration;
using ERService.OrderModule.Repository;
using ERService.OrderModule.Wrapper;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.OrderModule.ViewModels
{
    //TODO: Wydzielić z DetailViewModelBase DetailModelBase
    public class OrderContext : DetailViewModelBase, IOrderContext
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomItemRepository _customItemRepository;
        private readonly IHardwareTypeRepository _hardwareTypeRepository;
        private readonly IHwCustomItemRepository _hwCustomItemRepository;
        private readonly INumerationRepository _numerationRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly IOrderTypeRepository _orderTypeRepository;
        
        private CustomerWrapper _customer;
        private HardwareWrapper _hardware;
        private OrderWrapper _order;
        private Blob _selectedAttachment;
        private HardwareType _selectedHardwareType;
        private OrderStatus _selectedOrderStatus;
        private OrderType _selectedOrderType;

        public OrderContext(IEventAggregator eventAggregator, IMessageDialogService messageDialogService, IOrderRepository orderRepository,
            ICustomerRepository customerRepository, ICustomItemRepository customItemRepository, IHwCustomItemRepository hwCustomItemRepository,
            IHardwareTypeRepository hardwareTypeRepository, IOrderStatusRepository orderStatusRepository, IOrderTypeRepository orderTypeRepository,
            INumerationRepository numerationRepository) : base(eventAggregator, messageDialogService)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _customItemRepository = customItemRepository;
            _hwCustomItemRepository = hwCustomItemRepository;
            _hardwareTypeRepository = hardwareTypeRepository;
            _orderStatusRepository = orderStatusRepository;
            _orderTypeRepository = orderTypeRepository;
            _numerationRepository = numerationRepository;

            Customers = new ObservableCollection<Customer>();
            HardwareTypes = new ObservableCollection<HardwareType>();
            HardwareCustomItems = new ObservableCollection<HwCustomItem>();
            DisplayableCustomItems = new ObservableCollection<DisblayableCustomItem>();
            OrderStatuses = new ObservableCollection<OrderStatus>();
            OrderTypes = new ObservableCollection<OrderType>();

            Attachments = new ObservableCollection<Blob>();            

            Initialize();
        }

        public ObservableCollection<Blob> Attachments { get; }
        public ObservableCollection<Customer> Customers { get; }
        public ObservableCollection<DisblayableCustomItem> DisplayableCustomItems { get; }
        public ObservableCollection<HwCustomItem> HardwareCustomItems { get; }
        public ObservableCollection<HardwareType> HardwareTypes { get; }
        public ObservableCollection<OrderStatus> OrderStatuses { get; }
        public ObservableCollection<OrderType> OrderTypes { get; }

        public CustomerWrapper Customer { get => _customer; set => SetProperty(ref _customer, value); }
        public CustomerAddress CustomerAddress { get; private set; }
        public HardwareWrapper Hardware { get => _hardware; set => SetProperty(ref _hardware, value); }
        public OrderWrapper Order { get => _order; set => SetProperty(ref _order, value); }

        public bool HasErrors
        {
            get { return Customer.HasErrors || Hardware.HasErrors || Order.HasErrors; }
        }

        public Blob SelectedAttachment
        {
            get { return _selectedAttachment; }
            set { SetProperty(ref _selectedAttachment, value); }
        }

        public Customer SelectedCustomer { get; set; }

        public HardwareType SelectedHardwareType
        {
            get => _selectedHardwareType;
            set
            {
                SetProperty(ref _selectedHardwareType, value);
                Hardware.Model.HardwareTypeID = value?.Id;
            }
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

        private async void Initialize()
        {
            InitializeCustomersCombo();

            if (Customer == null)
                InitializeCustomer(new Customer());

            if (Hardware == null)
                InitializeHardware(new Hardware());

            if (Order == null)
                await InitializeOrder(new Order());

            await LoadHardwareTypesAsync()
                                            .ContinueWith((t) =>
                                            {
                                                SelectedHardwareType = HardwareTypes.FirstOrDefault();
                                            });

            await LoadOrderStatusesAsync()
                                            .ContinueWith((t) =>
                                            {
                                                SelectedOrderStatus = OrderStatuses.FirstOrDefault();
                                            });

            await LoadOrderTypesAsync()
                                            .ContinueWith((t) =>
                                            {
                                                SelectedOrderType = OrderTypes.FirstOrDefault();
                                            });

            PropertyChanged += async (s, a) =>
            {
                if (a.PropertyName == nameof(SelectedHardwareType))
                    await LoadHardwareCustomItemsAsync();
            };

            await LoadHardwareCustomItemsAsync();
        }        

        public void InitializeAddress(CustomerAddress customerAddress)
        {
            CustomerAddress = customerAddress ?? new CustomerAddress();

            if (customerAddress == null)
                Customer.Model.CustomerAddresses.Add(CustomerAddress);
        }

        public void InitializeCustomer(Customer customer)
        {
            if (customer == null) return;

            Customer = new CustomerWrapper(customer);

            InitializeAddress(customer.CustomerAddresses.FirstOrDefault());

            if (Customer.Id == Guid.Empty)
            {
                Customer.LastName = "";
                Customer.PhoneNumber = "";
            }
        }

        public async void InitializeHardware(Hardware hardware)
        {
            Hardware = new HardwareWrapper(hardware);

            if (hardware.HardwareType != null)
            {
                SelectedHardwareType = HardwareTypes.SingleOrDefault(ht => ht.Id == hardware.HardwareType.Id);
            }

            HardwareCustomItems.Clear();
            var hwCustomItems = await _hwCustomItemRepository.FindByAsync(i => i.HardwareId == hardware.Id);
            foreach (var item in hwCustomItems)
            {
                HardwareCustomItems.Add(item);
            }

            Hardware.Name = "";
            Hardware.SerialNumber = "";
        }

        public async Task InitializeOrder(Order order)
        {
            var numeration = await _numerationRepository.FindByAsync(n => n.Name == "default");

            if (numeration.Any())
            {
                order.Number = OrderNumberGenerator.GetNumberFromPattern(numeration.FirstOrDefault().Pattern);
            }

            order.DateRegistered = DateTime.Now;

            _orderRepository.Add(order);
            Order = new OrderWrapper(order);            
        }

        public async Task Save()
        {
            if (Customer.Id != Guid.Empty)
            {
                Order.Model.CustomerId = Customer.Id;
            }
            else
            {
                Order.Model.Customer = Customer.Model;
            }

            Hardware.Model.HardwareCustomItems.Clear();
            foreach (var item in HardwareCustomItems)
            {
                Hardware.Model.HardwareCustomItems.Add(item);
            }

            var hasHardware = Order.Model.Hardwares.Contains(Hardware.Model);
            if (!hasHardware)
            {
                Order.Model.Hardwares.Add(Hardware.Model);
            }

            await SaveWithOptimisticConcurrencyAsync(_orderRepository.SaveAsync, () =>
            {
                HasChanges =    _orderRepository.HasChanges()
                            &&  _customerRepository.HasChanges();
            });
        }        

        private async void InitializeCustomersCombo()
        {
            try
            {
                Customers.Clear();
                var customers = await _customerRepository.GetAllAsync();
                foreach (var customer in customers)
                {
                    Customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async Task LoadHardwareCustomItemsAsync()
        {
            if (SelectedHardwareType == null) return;

            DisplayableCustomItems.Clear();

            try
            {
                var items = await _customItemRepository
                                                        .FindByAsync(i => i.HardwareTypeId == SelectedHardwareType.Id);

                foreach (var item in items)
                {
                    if (HardwareCustomItems.Any(i => i.CustomItemId == item.Id))
                        continue;

                    HardwareCustomItems
                                        .Add(new HwCustomItem
                                        {
                                            CustomItemId = item.Id,
                                            Hardware = Hardware.Model,
                                            Value = ""
                                        });
                }

                var query = from hci in HardwareCustomItems
                            from ci in items
                            where hci.CustomItemId == ci.Id
                            select new DisblayableCustomItem { HwCustomItem = hci, CustomItem = ci };

                var displayableItems = query.ToList();

                foreach (var item in displayableItems)
                {
                    if (!DisplayableCustomItems.Contains(item))
                        DisplayableCustomItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                _logger.Error(ex);
            }
        }

        private async Task LoadHardwareTypesAsync()
        {
            HardwareTypes.Clear();

            var types = await _hardwareTypeRepository.GetAllAsync();
            if (types != null)
            {
                foreach (var type in types)
                {
                    HardwareTypes.Add(type);
                }
            }
        }

        private async Task LoadOrderStatusesAsync()
        {
            OrderStatuses.Clear();
            var statuses = await _orderStatusRepository.GetAllAsync();
            foreach (var status in statuses)
            {
                OrderStatuses.Add(status);
            }
        }

        private async Task LoadOrderTypesAsync()
        {
            OrderTypes.Clear();
            var types = await _orderTypeRepository.GetAllAsync();
            foreach (var type in types)
            {
                OrderTypes.Add(type);
            }
        }
    }
}