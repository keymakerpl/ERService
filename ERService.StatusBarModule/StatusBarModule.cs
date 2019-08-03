using System;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Events;
using ERService.StatusBar.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.StatusBar
{
    public class StatusBarModule : IModule
    {
        private IRegionManager _regionManager;

        public StatusBarModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.StatusbarRegion, typeof(StatusBarView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}