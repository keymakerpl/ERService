using ERService.Settings.ViewModels;
using ERService.Settings.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.Settings
{
    public class SettingsModule : IModule
    {
        protected IContainerProvider _container { get; private set; }
        protected IRegionManager _regionManager { get; private set; }

        public SettingsModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(SettingsView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _container = containerProvider;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}