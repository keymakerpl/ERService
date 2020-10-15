using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Helpers;
using ERService.OrderModule.Repository;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ERService.Infrastructure.Notifications.ToastNotifications;
using System.Diagnostics;
using LiveCharts.Wpf.Charts.Base;
using ERService.Infrastructure.Extensions;

namespace ERService.Statistics.ViewModels
{
    public class OrdersStatsViewModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IOrderRepository _orderRepository;
        private readonly IImagesCollection _imagesCollection;
        private DateTime _dateFrom;
        private DateTime _dateTo;

        public DelegateCommand<Chart> SaveToPDFCommand { get; }

        private ChartValues<int> _finishedCount;
        private ChartValues<int> _inProgressCount;
        private ChartValues<int> _openCount;
        private bool _isClosedVisible;
        private bool _isInProgressVisible;
        private bool _isOpenVisible;

        public OrdersStatsViewModel(
            IOrderRepository orderRepository,
            IEventAggregator eventaggregator,
            IMessageDialogService messageDialogService,
            IImagesCollection imagesCollection)
            : base(eventaggregator, messageDialogService)
        {
            _orderRepository = orderRepository;
            _imagesCollection = imagesCollection;
            
            PointLabel = point => point.Y > 0 ? $"{point.Y} ({point.Participation:P})" : "";

            _finishedCount = new ChartValues<int>();
            _inProgressCount = new ChartValues<int>();
            _openCount = new ChartValues<int>();

            IsClosedVisible = true;
            IsInProgressVisible = true;
            IsOpenVisible = true;

            _dateFrom = DateTime.Now.AddDays(-14).Date;
            _dateTo = DateTime.Now.Date;

            SaveToPDFCommand = new DelegateCommand<Chart>(OnSaveToPDFExecute);

            Title = "Naprawy";

            Initialize();
        }

        private void OnSaveToPDFExecute(Chart arg)
        {
            try
            {
                var pdfChart = arg.CloneChart<PieChart>();
                var dialog = new SaveFileDialog();
                dialog.FileName = $"Orders_{DateTime.Now:dd_MM_yyyy}.pdf";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.Filter = "Plik PDF (*.pdf)|*.pdf";
                var dialogResult = dialog.ShowDialog();

                if (dialogResult.HasValue && dialogResult.Value)
                {
                    using (var imageStream = new MemoryStream())
                    {
                        var header = $"Zlecenia serwisowe w okresie: od {_dateFrom:dd.MM.yyyy} do {_dateTo:dd.MM.yyyy}";
                        var logo = _imagesCollection["logo"].ImageData;
                        var encoder = new PngBitmapEncoder();

                        //TODO: Przerobić na serwisy dla DI
                        ImageHelper.SaveVisualToStream(pdfChart, encoder, imageStream, c => ((PieChart)c).Update(true, true));
                        PDFHelper.SaveImageToPDF(imageStream, dialog.FileName, header, logo);

                        _messageDialogService.ShowInsideContainer(
                            "Zapisano plik PDF...",
                            $"{dialog.FileName}",
                            NotificationTypes.Success,
                            onClick: () => Process.Start(dialog.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                _messageDialogService.ShowInsideContainer("Błąd zapisu pliku PDF...", ex.Message, NotificationTypes.Error);
                _logger.Error(ex);
            }
        }        

        private void Initialize()
        {
            PropertyChanged += async (s, a) =>
            {
                if (a.PropertyName == nameof(DateFrom) || a.PropertyName == nameof(DateTo))
                {
                    await LoadAsync();
                }
            };
        }

        public DateTime DateFrom
        {
            get { return _dateFrom; }
            set { SetProperty(ref _dateFrom, value); }
        }

        public DateTime DateTo
        {
            get { return _dateTo; }
            set { SetProperty(ref _dateTo, value); }
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

        public Func<ChartPoint, string> PointLabel { get; set; }

        public bool IsClosedVisible
        {
            get { return _isClosedVisible; }
            set { SetProperty(ref _isClosedVisible, value); }
        }

        public bool IsInProgressVisible
        {
            get { return _isInProgressVisible; }
            set { SetProperty(ref _isInProgressVisible, value); }
        }

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

        public override async Task LoadAsync()
        {
            _logger.Info("OrderStatsView LoadAsync Entered");

            var dateTo = _dateTo.AddDays(1);

            var openCount = await _orderRepository.FindByIncludeAsync(
                o => o.OrderStatus.Group == StatusGroup.Open && o.DateRegistered >= _dateFrom && o.DateRegistered <= dateTo,
                s => s.OrderStatus);

            OpenCount.Clear();
            OpenCount.Add(openCount.Count());

            var inProgressCount = await _orderRepository.FindByIncludeAsync(
                        o => o.OrderStatus.Group == StatusGroup.InProgress && o.DateRegistered >= _dateFrom && o.DateRegistered <= dateTo,
                        s => s.OrderStatus);

            InProgressCount.Clear();
            InProgressCount.Add(inProgressCount.Count());

            var finishedCount = await _orderRepository.FindByIncludeAsync(
                        o => o.OrderStatus.Group == StatusGroup.Finished && o.DateRegistered >= _dateFrom && o.DateRegistered <= dateTo,
                        s => s.OrderStatus);

            FinishedCount.Clear();
            FinishedCount.Add(finishedCount.Count());
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }
    }
}