using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.StatusBar
{
    public class StatusBarModule : IModule
    {
        private IRegionManager _regionManager;

        public StatusBarModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}