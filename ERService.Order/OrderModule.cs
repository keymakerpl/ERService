using ERService.Infrastructure.Constants;
using ERService.OrderModule.Data.Repository;
using ERService.OrderModule.Repository;
using ERService.OrderModule.Tasks;
using ERService.OrderModule.ViewModels;
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
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry   .Register(typeof(NewOrdersNotificationTask))
                                .RegisterSingleton<IOrderRepository, OrderRepository>()
                                .Register<IOrderStatusRepository, OrderStatusRepository>()
                                .Register<IOrderTypeRepository, OrderTypeRepository>()
                                .Register<IBlobRepository, BlobRepository>()
                                .Register<INumerationRepository, NumerationRepository>()
                                .Register<IOrderContext, OrderContext>();

            containerRegistry   .RegisterForNavigation<OrderSearchView>(ViewNames.OrderSearchView);
            containerRegistry   .RegisterForNavigation<OrderView>(ViewNames.OrderView);
            containerRegistry   .RegisterForNavigation<OrderListView>(ViewNames.OrderListView);
            containerRegistry   .RegisterForNavigation<OrderWizardView, OrderWizardViewModel>(ViewNames.OrderWizardView);
            containerRegistry   .RegisterForNavigation<OrderWizardCustomerView, OrderWizardCurrentStageModel>(ViewNames.OrderWizardCustomerView);
            containerRegistry   .RegisterForNavigation<OrderWizardHardwareView, OrderWizardCurrentStageModel>(ViewNames.OrderWizardHardwareView);
            containerRegistry   .RegisterForNavigation<OrderWizardOrderView, OrderWizardCurrentStageModel>(ViewNames.OrderWizardOrderView);
        }
    }
}