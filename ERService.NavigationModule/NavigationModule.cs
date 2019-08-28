﻿using System;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Events;
using ERService.Navigation.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.Navigation
{
    public class NavigationModule : IModule
    {
        private IRegionManager _regionManager;

        public NavigationModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.NavigationRegion, typeof(NavigationView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {                                     
                     
        }
    }
}