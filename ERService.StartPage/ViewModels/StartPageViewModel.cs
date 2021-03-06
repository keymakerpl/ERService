﻿using System;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.RBAC;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.StartPage.ViewModels
{
    public class StartPageViewModel : BindableBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        public bool KeepAlive => false;

        private IRegionManager _regionManager;
        private readonly IRBACManager _rBACManager;
        private readonly IMessageDialogService _dialogService;

        public DelegateCommand OrdersCommand { get; private set; }
        public DelegateCommand CustomersCommand { get; private set; }
        public DelegateCommand SettingsCommand { get; private set; }
        public DelegateCommand AddOrderCommand { get; private set; }
        public DelegateCommand StatsCommand { get; }
        public DelegateCommand<object> SearchCommand { get; }

        public StartPageViewModel(IRegionManager regionManager, IRBACManager rBACManager, IMessageDialogService dialogService)
        {
            _regionManager = regionManager;
            _rBACManager = rBACManager;
            _dialogService = dialogService;

            OrdersCommand = new DelegateCommand(OnOrdersCommandExecute);
            CustomersCommand = new DelegateCommand(OnCustomersCommandExecute);
            SettingsCommand = new DelegateCommand(OnSettingsCommandExecute);
            AddOrderCommand = new DelegateCommand(OnAddOrderExecute);
            StatsCommand = new DelegateCommand(OnStatsCommandExecute);
            SearchCommand = new DelegateCommand<object>(OnSearchExecute);
        }

        private void OnSearchExecute(object args)
        {
            var parameters = new NavigationParameters();
            parameters.Add("OrderNumber", args);

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.OrderListView, parameters);
        }

        private void OnStatsCommandExecute()
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.StatisticsTabView);
        }

        private async void OnAddOrderExecute()
        {
            if (!_rBACManager.LoggedUserHasPermission(AclVerbNames.CanAddOrder))
            {
                await _dialogService.ShowAccessDeniedMessageAsync(this);
                return;
            }

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.OrderWizardView);
        }

        private void OnSettingsCommandExecute()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll(); 
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.SettingsView);
        }

        private void OnCustomersCommandExecute()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll(); 
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.CustomerListView);
        }

        private void OnOrdersCommandExecute()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll(); 
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.OrderListView);
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
