using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.SQLLiteDataAccess
{
    public class SQLLiteDataAccessModule : IModule
    {
        protected IContainerProvider _container { get; private set; }
        protected IRegionManager _regionManager { get; private set; }

        public SQLLiteDataAccessModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
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