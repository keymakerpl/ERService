using ERService.Infrastructure.Constants;
using Prism.Ioc;
using Prism.Modularity;
using ERService.Statistics.Views;

namespace ERService.Statistics
{
    public class StatisticsModule : IModule
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<StatisticsTabView>(ViewNames.StatisticsTabView);
            containerRegistry.RegisterForNavigation<BasicStatsView>(ViewNames.BasicStatsView);
            containerRegistry.RegisterForNavigation<OrdersStatsView>(ViewNames.OrdersStatsView);
        }
    }
}
