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

        public Order Order
        {
            get { return GetValue<Order>(); }
            set { SetValue(value); }
        }

        public string Number
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public DateTime DateAdded
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime DateEnded
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public OrderStatus OrderStatus
        {
            get { return GetValue<OrderStatus>(); }
            set { SetValue(value); }
        }

        public OrderType OrderType
        {
            get { return GetValue<OrderType>(); }
            set { SetValue(value); }
        }

        public string Cost
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Fault
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Solution
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Comment
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ExternalNumber
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public int Progress
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
    }
}
