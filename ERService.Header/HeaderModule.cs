using ERService.Header.Views;
using ERService.Infrastructure.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.Header
{
    public class HeaderModule : IModule
    {
        private IContainerProvider _container;
        private IRegionManager _regionManager;

        public HeaderModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _container = containerProvider;
            _regionManager.RegisterViewWithRegion(RegionNames.HeaderRegion, typeof(HeaderView));
            _container.Resolve<HeaderView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}