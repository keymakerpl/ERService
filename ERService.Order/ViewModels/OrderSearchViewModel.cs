using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using ERService.OrderModule.Repository;
using Prism.Events;
using System;
using System.Collections.ObjectModel;

namespace ERService.OrderModule.ViewModels
{
    public class OrderSearchViewModel : SearchViewModelBase
    {
        private readonly IOrderStatusRepository _statusRepository;
        private readonly IOrderTypeRepository _typeRepository;
        private DateTime? _dateEndTo;
        private DateTime? _dateRegisteredTo;
        private bool _endDateIsChecked;
        private bool _registerDateIsChecked;
        private OrderStatus _selectedOrderStatus;
        private OrderType _selectedOrderType;

        public OrderSearchViewModel(
                            IOrderStatusRepository statusRepository,
            IOrderTypeRepository typeRepository,
            IEventAggregator eventAggregator) : base(eventAggregator)
        {
            Order = new Order() { DateRegistered = DateTime.Now.AddDays(-14).Date, DateEnded = DateTime.Now };
            Customer = new Customer();

            OrderStatuses = new ObservableCollection<OrderStatus>();
            OrderTypes = new ObservableCollection<OrderType>();

            _statusRepository = statusRepository;
            _typeRepository = typeRepository;

            RegisterDateIsChecked = true;            

            LoadOrderStatusesAsync();
            LoadOrderTypesAsync();
        }

        public Customer Customer { get; }

        public DateTime? DateEndTo
        {
            get { return _dateEndTo; }
            set { SetProperty(ref _dateEndTo, value); }
        }

        public DateTime? DateRegisteredTo
        {
            get { return _dateRegisteredTo; }
            set { SetProperty(ref _dateRegisteredTo, value); }
        }

        public bool RegisterDateIsChecked
        {
            get { return _registerDateIsChecked; }
            set { SetProperty(ref _registerDateIsChecked, value); }
        }

        public bool EndDateIsChecked
        {
            get { return _endDateIsChecked; }
            set { SetProperty(ref _endDateIsChecked, value); }
        }

        public Order Order { get; }

        public ObservableCollection<OrderStatus> OrderStatuses { get; }

        public ObservableCollection<OrderType> OrderTypes { get; }        

        public OrderStatus SelectedOrderStatus
        {
            get { return _selectedOrderStatus; }
            set
            {
                SetProperty(ref _selectedOrderStatus, value);
            }
        }

        public OrderType SelectedOrderType
        {
            get { return _selectedOrderType; }
            set
            {
                SetProperty(ref _selectedOrderType, value);
            }
        }

        protected override void OnSearchExecute()
        {
            var predicate = PredicateBuilder.True<Order>();

            if (!String.IsNullOrEmpty(Customer.FirstName))
            {
                predicate = predicate.And(o => o.Customer.FirstName.Contains(Customer.FirstName));
            }

            if (!String.IsNullOrEmpty(Customer.LastName))
            {
                predicate = predicate.And(o => o.Customer.LastName.Contains(Customer.LastName));
            }

            if (!String.IsNullOrWhiteSpace(Order.Number))
            {
                predicate = predicate.And(o => string.Concat(o.OrderId, "/", o.Number) == Order.Number);
            }

            if (SelectedOrderStatus != null && SelectedOrderStatus.Id != Guid.Empty)
            {
                predicate = predicate.And(o => o.OrderStatusId == SelectedOrderStatus.Id);
            }

            if (SelectedOrderType != null && SelectedOrderType.Id != Guid.Empty)
            {
                predicate = predicate.And(o => o.OrderTypeId == SelectedOrderType.Id);
            }

            if (!String.IsNullOrWhiteSpace(Order.ExternalNumber))
            {
                predicate = predicate.And(o => o.ExternalNumber == Order.ExternalNumber);
            }

            if (RegisterDateIsChecked && Order.DateRegistered != null)
            {
                predicate = predicate.And(o => o.DateRegistered >= Order.DateRegistered.Date);
                if (DateRegisteredTo.HasValue)
                {
                    var dateTo = DateRegisteredTo.Value.Date.AddDays(1).AddMilliseconds(-1);
                    predicate = predicate.And(o => o.DateRegistered <= dateTo);
                }
            }

            if (EndDateIsChecked && Order.DateEnded.HasValue)
            {
                var dateEndFrom = Order.DateEnded.Value.Date;
                predicate = predicate.And(o => o.DateEnded >= dateEndFrom);
                if (DateEndTo.HasValue)
                {
                    var dateEndTo = DateEndTo.Value.Date.AddDays(1).AddMilliseconds(-1);
                    predicate = predicate.And(o => o.DateEnded <= dateEndTo);
                }
            }

            EventAggregator.GetEvent<SearchEvent<Order>>().Publish(new SearchEventArgs<Order>() { Predicate = predicate });
        }

        private async void LoadOrderStatusesAsync()
        {
            OrderStatuses.Clear();
            OrderStatuses.Add(new OrderStatus() { Id = Guid.Empty, Name = "" });
            var statuses = await _statusRepository.GetAllAsync();
            foreach (var status in statuses)
            {
                OrderStatuses.Add(status);
            }
        }

        private async void LoadOrderTypesAsync()
        {
            OrderTypes.Clear();
            OrderTypes.Add(new OrderType() { Id = Guid.Empty, Name = "" });
            var types = await _typeRepository.GetAllAsync();
            foreach (var type in types)
            {
                OrderTypes.Add(type);
            }
        }
    }
}