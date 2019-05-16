using ERService.CustomerModule.Repository;
using ERService.CustomerModule.Views;
using ERService.Infrastructure.Constants;
using ERService.Navigation.Views;
using ERService.Settings.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.Navigation
{
    public class NavigationModule : IModule
    {
        private IContainerProvider _container;
        private IRegionManager _regionManager;

        public NavigationModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _container = containerProvider;
            _regionManager.RegisterViewWithRegion(RegionNames.NavigationRegion, typeof(NavigationView));
            _container.Resolve<NavigationView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        { 
            containerRegistry.Register<ICustomerRepository, CustomerRepository>();
            containerRegistry.Register<object, CustomerView>(typeof(CustomerView).FullName);
            containerRegistry.Register<object, SettingsView>(typeof(SettingsView).FullName);
            containerRegistry.Register<object, CustomerListView>(typeof(CustomerListView).FullName);
        }
    }
}