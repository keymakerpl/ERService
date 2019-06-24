﻿using System;
using ERService.Infrastructure.Constants;
using ERService.Settings.Views;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Settings.ViewModels
{
    public class SettingsViewModel : BindableBase, INavigationAware
    {
        private IRegionManager _regionManager;

        public SettingsViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        #region Navigation        

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.GeneralSettingsView);
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.HardwareTypesView);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        #endregion
    }
}
