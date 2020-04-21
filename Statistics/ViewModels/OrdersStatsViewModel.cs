using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.OrderModule.Repository;
using LiveCharts;
using LiveCharts.Wpf;
using Prism.Events;
using Prism.Regions;
using System;
using System.Linq;

namespace ERService.Statistics.ViewModels
{
    public class OrdersStatsViewModel : DetailViewModelBase
    {
        private readonly IOrderRepository _orderRepository;

        private ChartValues<int> _finishedCount;
        private ChartValues<int> _inProgressCount;
        private ChartValues<int> _openCount;

        public OrdersStatsViewModel(IOrderRepository orderRepository, IEventAggregator eventaggregator, IMessageDialogService messageDialogService)
            : base(eventaggregator, messageDialogService)
        {
            _orderRepository = orderRepository;

            PointLabel = point => $"{point.Y} ({point.Participation:P})";

            _finishedCount = new ChartValues<int>();
            _inProgressCount = new ChartValues<int>();
            _openCount = new ChartValues<int>();

            IsClosedVisible = true;
            IsInProgressVisible = true;
            IsOpenVisible = true;

            _dateFrom = DateTime.Now.AddDays(-14).Date;
            _dateTo = DateTime.Now.Date;

            Title = "Naprawy";
        }

        public ChartValues<int> FinishedCount
        {
            get { return _finishedCount; }
            private set { SetProperty(ref _finishedCount, value); }
        }

        public ChartValues<int> InProgressCount
        {
            get { return _inProgressCount; }
            private set { SetProperty(ref _inProgressCount, value); }
        }

        public ChartValues<int> OpenCount
        {
            get { return _openCount; }
            private set { SetProperty(ref _openCount, value); }
        }

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

        private bool _isClosedVisible;
        public bool IsClosedVisible
        {
            get { return _isClosedVisible; }
            set { SetProperty(ref _isClosedVisible, value); }
        }

        private bool _isInProgressVisible;
        public bool IsInProgressVisible
        {
            get { return _isInProgressVisible; }
            set { SetProperty(ref _isInProgressVisible, value); }
        }

        private bool _isOpenVisible;
        public bool IsOpenVisible
        {
            get { return _isOpenVisible; }
            set { SetProperty(ref _isOpenVisible, value); }
        }

        public override bool KeepAlive => true;

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public Func<ChartPoint, string> PointLabel { get; set; }

        public override void Load()
        {
            OpenCount.Clear();
            InProgressCount.Clear();
            FinishedCount.Clear();

            var dateTo = _dateTo.AddDays(1);

            var openCount = _orderRepository.FindByInclude(o => o.OrderStatus.Group == Business.StatusGroup.Open && (o.DateRegistered >= _dateFrom && o.DateRegistered <= dateTo), s => s.OrderStatus).Count();            
            OpenCount.Add(openCount);

            var inProgressCount = _orderRepository.FindByInclude(o => o.OrderStatus.Group == Business.StatusGroup.InProgress && (o.DateRegistered >= _dateFrom && o.DateRegistered <= dateTo), s => s.OrderStatus).Count();
            InProgressCount.Add(inProgressCount);

            var finishedCount = _orderRepository.FindByInclude(o => o.OrderStatus.Group == Business.StatusGroup.Finished && (o.DateRegistered >= _dateFrom && o.DateRegistered <= dateTo), s => s.OrderStatus).Count();
            FinishedCount.Add(finishedCount);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Load();
        }

        private void Chart_OnDataClick(ChartPoint chartpoint)
        {
            if (chartpoint == null)
            {
                throw new ArgumentNullException(nameof(chartpoint));
            }

            var chart = chartpoint.ChartView as PieChart;
            if (chart != null)
            {
                foreach (PieSeries series in chart.Series)
                    series.PushOut = 0;
            }

            var selectedSeries = chartpoint.SeriesView as PieSeries;
            if (selectedSeries != null)
            {
                selectedSeries.PushOut = 8;
            }
        }
    }
}