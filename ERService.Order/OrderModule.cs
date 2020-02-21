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
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IRegionManager _regionManager;
        private readonly IBackgroundTaskRegistration _taskRegistration;

        public OrderModule(IRegionManager regionManager, IBackgroundTaskRegistration taskRegistration)
        {
            _regionManager = regionManager;
            _taskRegistration = taskRegistration;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {            
            _taskRegistration.Register(new BackgroundTask<NewOrdersNotificationTask>(CronExpressions.EveryOneMinute));            
            _logger.Info("Initialized");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(NewOrdersNotificationTask));
            containerRegistry.RegisterForNavigation(typeof(OrderSearchView), ViewNames.OrderSearchView);
        }
    }
}