using ERService.Business;
using ERService.Infrastructure.Attributes;
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

        private string _number;

        [Interpreter(Name = "Numer zlecenia", Pattern = "[%o_number%]")]
        public string Number
        {
            get { return $"{Model.OrderId}/{Model.Number}"; }
            set { SetProperty(ref _number, value); }
        }

        private DateTime _dateAdded;

        [Interpreter(Name = "Data rejestracji", Pattern = "[%o_DateAdded%]")]
        public DateTime DateAdded
        {
            get { return GetValue<DateTime>(); }
            set { SetProperty(ref _dateAdded, value); }
        }

        private DateTime _datEnded;

        [Interpreter(Name = "Data zakończenia", Pattern = "[%o_DateEnded%]")]
        public DateTime DateEnded
        {
            get { return GetValue<DateTime>(); }
            set { SetProperty(ref _datEnded, value); }
        }

        private OrderStatus _orderStatus;

        [Interpreter(Name = "Status zlecenia", Pattern = "[%o_OrderStatus%]")]
        public OrderStatus OrderStatus
        {
            get { return GetValue<OrderStatus>(); }
            set { SetProperty(ref _orderStatus, value); }
        }

        private OrderType _orderType;

        [Interpreter(Name = "Typ zlecenia", Pattern = "[%o_OrderType%]")]
        public OrderType OrderType
        {
            get { return GetValue<OrderType>(); }
            set { SetProperty(ref _orderType, value); }
        }

        private string _cost;

        [Interpreter(Name = "Koszt zlecenia", Pattern = "[%o_cost%]")]
        public string Cost
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _cost, value); }
        }

        private string _fault;

        [Interpreter(Name = "Opis usterki", Pattern = "[%o_fault%]")]
        public string Fault
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _fault, value); }
        }

        private string _solution;

        [Interpreter(Name = "Opis naprawy", Pattern = "[%o_solution%]")]
        public string Solution
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _solution, value); }
        }

        private string _comment;

        [Interpreter(Name = "Komentarz", Pattern = "[%o_comment%]")]
        public string Comment
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _comment, value); }
        }

        private string _externalNumber;

        [Interpreter(Name = "Numer zewnętrzny", Pattern = "[%o_externalNumber%]")]
        public string ExternalNumber
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _externalNumber, value); }
        }

        private int _progress;

        [Interpreter(Name = "Postęp zlecenia", Pattern = "[%o_progress%]")]
        public int Progress
        {
            get { return GetValue<int>(); }
            set { SetProperty(ref _progress, value); }
        }
    }
}
