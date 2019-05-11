using ERService.StatusBar.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.StatusBar
{
    public class StatusBarModule : IModule
    {
        private IContainerProvider _container;
        private IRegionManager _regionManager;

        public StatusBarModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _container = containerProvider;
            _regionManager.RegisterViewWithRegion("StatusBarRegion", typeof(StatusBarView));
            _container.Resolve<StatusBarView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}