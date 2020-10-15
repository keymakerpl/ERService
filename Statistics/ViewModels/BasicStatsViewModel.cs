using ERService.CustomerModule.Repository;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.OrderModule.Repository;
using LiveCharts;
using LiveCharts.Configurations;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.Statistics.ViewModels
{
    public class DateModel
    {
        public DateTime DateTime { get; set; }
        public int Value { get; set; }
    }

    public class BasicStatsViewModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public BasicStatsViewModel(
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            Initialize();

            _customerRepository = customerRepository;
            _orderRepository = orderRepository;

            Title = "Ogólne";
        }

        private void Initialize()
        {
            var mapper = Mappers.Xy<DateModel>()
                                                .X(dt => dt.DateTime.Ticks / TimeSpan.FromDays(1).Ticks)
                                                .Y(v => v.Value);

            Charting.For<DateModel>(mapper, SeriesOrientation.Horizontal);

            OrderValues = new ChartValues<DateModel>();
            CustomerValues = new ChartValues<DateModel>();

            IsCustomersVisible = true;
            IsOrdersVisible = true;

            _dateFrom = DateTime.Now.AddDays(-14).Date;
            _dateTo = DateTime.Now.Date;

            Formatter = (value) =>
            {
                try
                {
                    var ticks = (long)(value * TimeSpan.FromDays(1).Ticks);
                    return new DateTime(ticks).ToString("d");
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex);
                    return "Error";
                }
            };

            PropertyChanged += async (s, a) =>
            {
                if (a.PropertyName == nameof(DateFrom) || a.PropertyName == nameof(DateTo))
                {
                    await LoadAsync();
                }
            };
        }

        public Func<double, string> Formatter { get; private set; }

        private DateTime _dateFrom;
        public DateTime DateFrom
        {
            get { return _dateFrom; }
            set { SetProperty(ref _dateFrom, value); }
        }

        private DateTime _dateTo;
        public DateTime DateTo
        {
            get { return _dateTo; }
            set { SetProperty(ref _dateTo, value); }
        }

        private bool _isCustomersVisible;
        public bool IsCustomersVisible
        {
            get { return _isCustomersVisible; }
            set { SetProperty(ref _isCustomersVisible, value); }
        }

        private bool _isOrdersVisible;

        public bool IsOrdersVisible
        {
            get { return _isOrdersVisible; }
            set { SetProperty(ref _isOrdersVisible, value); }
        }

        //TODO: Refactor
        public override async Task LoadAsync()
        {
            _logger.Info("BasicStats LoadAsync Entered");

            try
            {
                var dateTo = _dateTo.AddDays(1);
                var customers = await _customerRepository.FindByAsync(c => c.DateAdded >= _dateFrom && c.DateAdded <= dateTo);
                var orders = await _orderRepository.FindByAsync(o => o.DateRegistered >= _dateFrom && o.DateRegistered <= dateTo);
                var customerDates = GetEmptyDateModels(_dateFrom, _dateTo)
                                                                          .Cast<DateModel>()
                                                                          .ToDictionary(dm => dm.DateTime);

                var customerQuery = from c in customers
                                    group c by c.DateAdded.Date into g
                                    orderby g.Key
                                    select new DateModel { DateTime = g.Key, Value = g.Count() };

                foreach (var dateModel in customerQuery)
                {
                    customerDates[dateModel.DateTime].Value = dateModel.Value;
                }

                CustomerValues.Clear();
                CustomerValues.AddRange(customerDates.Values);

                var orderDates = GetEmptyDateModels(_dateFrom, _dateTo)
                                                                          .Cast<DateModel>()
                                                                          .ToDictionary(dm => dm.DateTime);

                var orderQuery = from o in orders
                                 group o by o.DateRegistered.Date into g
                                 orderby g.Key
                                 select new DateModel { DateTime = g.Key, Value = g.Count() };

                foreach (var dateModel in orderQuery)
                {
                    orderDates[dateModel.DateTime].Value = dateModel.Value;
                }

                OrderValues.Clear();
                OrderValues.AddRange(orderDates.Values);

                RaisePropertyChanged(nameof(CustomersCount));
                RaisePropertyChanged(nameof(OrdersCount));
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                _logger.Error(ex);
            }
        }

        private IEnumerable GetEmptyDateModels(DateTime dateFrom, DateTime dateTo)
        {
            var daysCount = (dateTo - dateFrom).TotalDays;

            for (int i = 0; i <= daysCount; i++)
            {
                yield return new DateModel() { DateTime = dateFrom.AddDays(i) };
            }
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override bool KeepAlive => true;

        public int CustomersCount => CustomerValues.Sum(dm => dm.Value);

        public int OrdersCount => OrderValues.Sum(dm => dm.Value);

        public ChartValues<DateModel> CustomerValues { get; set; }
        public ChartValues<DateModel> OrderValues { get; set; }
    }
}