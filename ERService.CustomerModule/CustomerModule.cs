using ERService.CustomerModule.Repository;
using ERService.CustomerModule.Views;
using ERService.HardwareModule.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.CustomerModule
{
    public class CustomerModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ICustomerRepository, CustomerRepository>();
            containerRegistry.RegisterForNavigation<CustomerView>(typeof(CustomerView).FullName);
            containerRegistry.RegisterForNavigation<CustomerListView>(typeof(CustomerListView).FullName);
            containerRegistry.RegisterForNavigation<HardwareView>(typeof(HardwareView).FullName);
        }
    }
}