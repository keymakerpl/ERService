using ERService.Infrastructure.Constants;
using ERService.StartPage.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.StartPage
{
    public class StartPageModule : IModule
    {
        private IRegionManager _regionManager;

        public StartPageModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, typeof(StartPageView).FullName);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<StartPageView>(typeof(StartPageView).FullName);
        }
    }
}