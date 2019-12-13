using ERService.Header.Views;
using ERService.Infrastructure.Constants;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.Header
{
    public class HeaderModule : IModule
    {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;

        public HeaderModule(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.HeaderRegion, typeof(HeaderView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}