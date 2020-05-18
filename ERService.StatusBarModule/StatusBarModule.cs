using System;
using System.Threading;
using System.Windows.Threading;
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
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}