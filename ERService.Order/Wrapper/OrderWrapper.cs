using ERService.Business;
using ERService.Infrastructure.Wrapper;
using System;

namespace ERService.OrderModule.Wrapper
{
    public class OrderWrapper : ModelWrapper<Order>
    {
        public OrderWrapper(Order model) : base(model)
        {
        }

        public Guid Id { get { return Model.Id; } }

        private Order _order;
        public Order Order
        {
            get { return GetValue<Order>(); }
            set { SetProperty(ref _order, value); }
        }

        private string _number;
        public string Number
        {
            get { return $"{Model.OrderId}/{Model.Number}"; }
            set { SetProperty(ref _number, value); }
        }

        private DateTime _dateAdded;
        public DateTime DateAdded
        {
            get { return GetValue<DateTime>(); }
            set { SetProperty(ref _dateAdded, value); }
        }

        private DateTime _datEnded;
        public DateTime DateEnded
        {
            get { return GetValue<DateTime>(); }
            set { SetProperty(ref _datEnded, value); }
        }

        private OrderStatus _orderStatus;
        public OrderStatus OrderStatus
        {
            get { return GetValue<OrderStatus>(); }
            set { SetProperty(ref _orderStatus, value); }
        }

        private OrderType _orderType;
        public OrderType OrderType
        {
            get { return GetValue<OrderType>(); }
            set { SetProperty(ref _orderType, value); }
        }

        private string _cost;
        public string Cost
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _cost, value); }
        }

        private string _fault;
        public string Fault
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _fault, value); }
        }

        private string _solution;
        public string Solution
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _solution, value); }
        }

        private string _comment;
        public string Comment
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _comment, value); }
        }

        private string _externalNumber;
        public string ExternalNumber
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _externalNumber, value); }
        }

        private int _progress;
        public int Progress
        {
            get { return GetValue<int>(); }
            set { SetProperty(ref _progress, value); }
        }
    }
}
