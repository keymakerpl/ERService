using ERService.Infrastructure.Constants;
using ERService.OrderModule.Tasks;
using ERService.OrderModule.Views;
using ERService.Services.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.OrderModule
{
    public class OrderModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IBackgroundTaskRegistration _taskRegistration;

        public OrderModule(IRegionManager regionManager, IBackgroundTaskRegistration taskRegistration)
        {
            _regionManager = regionManager;
            _taskRegistration = taskRegistration;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.OrderSearchRegion, typeof(OrderSearchView));

            var newOrderTask = new NewOrdersNotificationBackgroundTask();
            _taskRegistration.Register(newOrderTask);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}