using ERService.CustomerModule.Views;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Repositories;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.CustomerModule
{
    public class CustomerModule : IModule
    {
        protected IContainerProvider _container { get; private set; }
        protected IRegionManager _regionManager { get; private set; }

        public CustomerModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(CustomerView));
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(CustomerListView));
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