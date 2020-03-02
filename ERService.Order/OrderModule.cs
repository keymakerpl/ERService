using ERService.Infrastructure.Constants;
using ERService.OrderModule.Data.Repository;
using ERService.OrderModule.Repository;
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
            containerRegistry.Register(typeof(NewOrdersNotificationTask))
                             .Register<IOrderRepository, OrderRepository>()
                             .Register<IOrderStatusRepository, OrderStatusRepository>()
                             .Register<IOrderTypeRepository, OrderTypeRepository>()
                             .Register<IBlobRepository, BlobRepository>()
                             .Register<INumerationRepository, NumerationRepository>();

            containerRegistry.RegisterForNavigation<OrderSearchView>(ViewNames.OrderSearchView);
            containerRegistry.RegisterForNavigation<OrderView>(ViewNames.OrderView);
            containerRegistry.RegisterForNavigation<OrderListView>(ViewNames.OrderListView);
        }
    }
}