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

        public bool EndDateIsChecked
        {
            get { return _endDateIsChecked; }
            set { SetProperty(ref _endDateIsChecked, value); }
        }

        public Order Order { get; }

        public ObservableCollection<OrderStatus> OrderStatuses { get; }

        public ObservableCollection<OrderType> OrderTypes { get; }

        public bool RegisterDateIsChecked
        {
            get { return _registerDateIsChecked; }
            set { SetProperty(ref _registerDateIsChecked, value); }
        }

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
            var query = new QueryBuilder(nameof(Order)).Select($"{nameof(Order)}.{nameof(Order.Id)}");

            query.LeftJoin(nameof(Customer), $"{nameof(Customer)}.{nameof(Customer.Id)}", $"{nameof(Order)}.{nameof(Order.CustomerId)}");

            if (!String.IsNullOrEmpty(Customer.FirstName))
            {
                query.WhereLike(nameof(Customer.FirstName), Customer.FirstName);
            }

            if (!String.IsNullOrEmpty(Customer.LastName))
            {
                query.WhereLike(nameof(Customer.LastName), Customer.LastName);
            }

            if (!String.IsNullOrWhiteSpace(Order.Number))
            {
                query.WhereRaw($"(CAST([{nameof(Order.OrderId)}] AS NVARCHAR)+'/'+[{nameof(Order.Number)}]) = ?", Order.Number);
            }

            if (SelectedOrderStatus != null && SelectedOrderStatus.Id != Guid.Empty)
            {
                query.Where(nameof(Order.OrderStatusId), SelectedOrderStatus.Id);
            }

            if (SelectedOrderType != null && SelectedOrderType.Id != Guid.Empty)
            {
                query.Where(nameof(Order.OrderTypeId), SelectedOrderType.Id);
            }

            if (!String.IsNullOrWhiteSpace(Order.ExternalNumber))
            {
                query.Where(nameof(Order.ExternalNumber), Order.ExternalNumber);
            }

            if (RegisterDateIsChecked && Order.DateRegistered != null)
            {
                query.WhereDate(nameof(Order.DateRegistered), ">=", Order.DateRegistered.Date);
                if (DateRegisteredTo.HasValue)
                {
                    query.WhereDate(nameof(Order.DateRegistered), "<=", DateRegisteredTo.Value.Date);
                }
            }

            if (EndDateIsChecked && Order.DateEnded.HasValue)
            {
                query.WhereDate(nameof(Order.DateEnded), ">=", Order.DateEnded.Value.Date);
                if (DateEndTo.HasValue)
                {
                    query.WhereDate(nameof(Order.DateEnded), "<=", DateEndTo.Value.Date);
                }
            }

            EventAggregator.GetEvent<SearchQueryEvent>().Publish(new SearchQueryEventArgs() { QueryBuilder = query });
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