using ERService.Business;
using ERService.Infrastructure.Attributes;
using ERService.Infrastructure.Helpers;
using ERService.Infrastructure.Wrapper;
using System;

namespace ERService.OrderModule.Wrapper
{
    public class OrderWrapper : ModelWrapper<Order>
    {
        private string _comment;

        private string _cost;

        private DateTime? _dateEnded;

        private DateTime _dateRegistered;

        private string _externalNumber;

        private string _fault;

        private string _number;

        private OrderStatus _orderStatus;

        private OrderType _orderType;

        private int _progress;

        private string _solution;
        private Guid? _orderStatusId;
        private Guid? _orderTypeId;

        public OrderWrapper(Order model) : base(model)
        {
        }

        public Guid Id { get { return Model.Id; } }

        [Interpreter(Name = "Komentarz", Pattern = "[%o_comment%]")]
        public string Comment
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _comment, value); }
        }

        [Interpreter(Name = "Koszt zlecenia", Pattern = "[%o_cost%]")]
        public string Cost
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _cost, value); }
        }        

        [Interpreter(Name = "Termin zakończenia", Pattern = "[%o_DateEnded%]")]
        public DateTime? DateEnded
        {
            get { return GetValue<DateTime?>(); }
            set { SetProperty(ref _dateEnded, value.Value.Date.AddDays(1).AddMilliseconds(-100)); }
        }

        [Interpreter(Name = "Data rejestracji", Pattern = "[%o_DateRegistered%]")]
        public DateTime DateRegistered
        {
            get { return GetValue<DateTime>(); }
            set { SetProperty(ref _dateRegistered, value); }
        }

        /// <summary>
        /// Właściwość pomocnicza do ustawiania czasu w dacie rejestracji przez kontrolke DateTimePicker
        /// </summary>
        public TimeSpan TimeRegistered
        {
            get { return DateRegistered.TimeOfDay; }
            set { DateRegistered = DateRegistered.Date.Add(value); RaisePropertyChanged(); }
        }

        [Interpreter(Name = "Numer zewnętrzny", Pattern = "[%o_externalNumber%]")]
        public string ExternalNumber
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _externalNumber, value); }
        }

        [Interpreter(Name = "Opis usterki", Pattern = "[%o_fault%]")]
        public string Fault
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _fault, value); }
        }        

        [Interpreter(Name = "Numer zlecenia", Pattern = "[%o_number%]")]
        public string Number
        {
            get
            {
                var slash = !String.IsNullOrWhiteSpace(Model.Number) ? "/" : String.Empty;
                return $"{Model.OrderId}{slash}{Model.Number}";
            }
            set { SetProperty(ref _number, value); }
        }

        [Interpreter(Name = "Status zlecenia", Pattern = "[%o_OrderStatus%]")]
        public OrderStatus OrderStatus
        {
            get { return GetValue<OrderStatus>(); }
            set { SetProperty(ref _orderStatus, value); }
        }

        [Interpreter(Name = "Typ zlecenia", Pattern = "[%o_OrderType%]")]
        public OrderType OrderType
        {
            get { return GetValue<OrderType>(); }
            set { SetProperty(ref _orderType, value); }
        }

        [Interpreter(Name = "Postęp zlecenia", Pattern = "[%o_progress%]")]
        public int Progress
        {
            get { return GetValue<int>(); }
            set { SetProperty(ref _progress, value); }
        }

        [Interpreter(Name = "Opis naprawy", Pattern = "[%o_solution%]")]
        public string Solution
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _solution, value); }
        }

        public DateTime DateAdded
        {
            get { return GetValue<DateTime>(); }
        }

        public DateTime DateModified
        {
            get { return GetValue<DateTime>(); }
        }

        /// <summary>
        /// Returns Barcode in Base64String format for Html usage
        /// </summary>
        [Interpreter(Name = "Barcode", Pattern = "[%o_barcode%]")]
        public string BarcodeBase64
        {
            get
            {
                var barcode = BarcodeGenerator.GenerateBase64String(Number ?? "");
                var result = $"<img src=\"data:image/gif;base64,{barcode}\" alt=\"logo\" />";
                return result;
            }
        }

        public Guid? OrderStatusId
        {
            get { return GetValue<Guid?>(); }
            internal set { SetProperty(ref _orderStatusId, value); }
        }

        public Guid? OrderTypeId
        {
            get { return GetValue<Guid?>(); }
            internal set { SetProperty(ref _orderTypeId, value); }
        }
    }
}