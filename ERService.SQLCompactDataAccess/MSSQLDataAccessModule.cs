using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.MSSQLDataAccess
{
    public class MSSQLDataAccessModule : IModule
    {
        protected IContainerProvider _container { get; private set; }
        protected IRegionManager _regionManager { get; private set; }

        public MSSQLDataAccessModule(IRegionManager regionManager)
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