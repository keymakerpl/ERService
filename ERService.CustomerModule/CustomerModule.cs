using ERService.CustomerModule.Views;
using ERService.Infrastructure.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.CustomerModule
{
    public class CustomerModule : IModule
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IRegionManager _regionManager;

        public CustomerModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {            
            _logger.Info("Initialized.");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation(typeof(CustomerSearchView), ViewNames.CustomerSearchView);
        }
    }
}