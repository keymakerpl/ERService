using ERService.HardwareModule.Data.Repository;
using ERService.HardwareModule.ViewModels;
using ERService.HardwareModule.Views;
using ERService.Infrastructure.Constants;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.HardwareModule
{
    public class HardwareModule : IModule
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry   .Register<IHardwareRepository, HardwareRepository>()
                                .Register<IHardwareTypeRepository, HardwareTypeRepository>()
                                .Register<ICustomItemRepository, CustomItemRepository>()
                                .Register<IHwCustomItemRepository, HwCustomItemRepository>();

            containerRegistry.RegisterForNavigation<HardwareView>(ViewNames.HardwareView);
            containerRegistry.RegisterForNavigation<HardwareFlyoutDetailView, HardwareViewModel>(ViewNames.HardwareFlyoutDetailView);
        }
    }
}