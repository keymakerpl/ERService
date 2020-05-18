using ERService.Infrastructure.Constants;
using ERService.Navigation.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.Navigation
{
    public class NavigationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationView>(ViewNames.NavigationView);
        }
    }
}