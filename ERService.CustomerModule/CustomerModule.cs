using ERService.CustomerModule.Views;
using ERService.CustomerModule.Repository;
using ERService.CustomerModule.ViewModels;
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

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ICustomerRepository, CustomerRepository>();

            containerRegistry.RegisterForNavigation<CustomerView>(ViewNames.CustomerView);
            containerRegistry.RegisterForNavigation<CustomerListView>(ViewNames.CustomerListView);
            containerRegistry.RegisterForNavigation<CustomerSearchView>(ViewNames.CustomerSearchView);
            containerRegistry.RegisterForNavigation<CustomerFlyoutDetailView, CustomerViewModel>(ViewNames.CustomerFlyoutDetailView);            
        }
    }
}