using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.CustomerModule
{
    public class CustomerModule : IModule
    {
        protected IContainerProvider _container { get; private set; }
        protected IRegionManager _regionManager { get; }
        public IModuleManager _moduleManager { get; }

        public CustomerModule(IRegionManager regionManager, IModuleManager moduleManager)
        {
            _regionManager = regionManager;
            _moduleManager = moduleManager;
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