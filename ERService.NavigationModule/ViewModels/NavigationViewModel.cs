﻿using ERService.Infrastructure.Constants;
using Prism.Commands;
using Prism.Regions;
using System;

namespace ERService.Navigation.ViewModels
{
    public class NavigationViewModel
    {
        public DelegateCommand<object> OpenDetailViewCommand { get; }

        public IRegionManager _regionManager { get; private set; }

        public NavigationViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            OpenDetailViewCommand = new DelegateCommand<object>(OnOpenDetailViewExecute);
        }

        private void OnOpenDetailViewExecute(object viewType)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, new Uri(viewType.ToString(), UriKind.Relative));
        }
    }
}
