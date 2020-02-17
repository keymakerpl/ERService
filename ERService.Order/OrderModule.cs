using ERService.Infrastructure.Constants;
using ERService.OrderModule.Tasks;
using ERService.OrderModule.Views;
using ERService.Services.Tasks;
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
            
            _taskRegistration.Register(new BackgroundTask<NewOrdersNotificationTask>("*/1 * * * *"));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(NewOrdersNotificationTask));
        }
    }
}