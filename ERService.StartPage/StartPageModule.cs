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
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _container = containerProvider;
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(StartPageView));
            _container.Resolve<StartPageView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}