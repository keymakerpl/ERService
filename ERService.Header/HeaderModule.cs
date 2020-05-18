using ERService.Header.Views;
using ERService.Infrastructure.Constants;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.Header
{
    public class HeaderModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HeaderView>(ViewNames.HeaderView);
        }
    }
}