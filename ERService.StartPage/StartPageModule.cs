using ERService.Infrastructure.Constants;
using ERService.StartPage.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.StartPage
{
    public class StartPageModule : IModule
    {
        private IContainerProvider _container;
        private IRegionManager _regionManager;

        public StartPageModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(StartPageView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _container = containerProvider;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}