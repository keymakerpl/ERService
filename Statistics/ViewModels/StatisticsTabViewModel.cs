using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using Prism.Events;
using Prism.Regions;

namespace ERService.Statistics.ViewModels
{
    public class StatisticsTabViewModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IRegionManager _regionManager;

        public StatisticsTabViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IMessageDialogService messageDialogService) 
            : base(eventAggregator, messageDialogService)
        {
            _regionManager = regionManager;
        }

        public override bool KeepAlive
        {
            get
            {
                return true;
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _regionManager.RequestNavigate(RegionNames.StatsTabControlRegion, ViewNames.BasicStatsView, OnNavigatedResult);
            _regionManager.RequestNavigate(RegionNames.StatsTabControlRegion, ViewNames.OrdersStatsView, OnNavigatedResult);
        }
    }
}
