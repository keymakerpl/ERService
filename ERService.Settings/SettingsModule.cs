using ERService.Settings.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.Settings
{
    public class SettingsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SettingsView>(typeof(SettingsView).FullName);
        }
    }
}