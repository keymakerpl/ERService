using ERService.OrderModule.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.OrderModule
{
    public class OrderModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<OrderView>(typeof(OrderView).FullName);
            containerRegistry.RegisterForNavigation<OrderListView>(typeof(OrderListView).FullName);
        }
    }
}