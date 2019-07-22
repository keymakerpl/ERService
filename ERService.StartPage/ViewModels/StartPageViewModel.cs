using System;
using ERService.Infrastructure.Constants;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.StartPage.ViewModels
{
    public class StartPageViewModel : BindableBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        public bool KeepAlive => false;

        private IRegionManager _regionManager;

        public DelegateCommand OrdersCommand { get; private set; }
        public DelegateCommand CustomersCommand { get; private set; }
        public DelegateCommand SettingsCommand { get; private set; }
        public DelegateCommand AddOrderCommand { get; private set; }

        public StartPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            OrdersCommand = new DelegateCommand(OnOrdersCommandExecute);
            CustomersCommand = new DelegateCommand(OnCustomersCommandExecute);
            SettingsCommand = new DelegateCommand(OnSettingsCommandExecute);
            AddOrderCommand = new DelegateCommand(OnAddOrderExecute);
        }

        private void OnAddOrderExecute()
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("Wizard", true);
            parameters.Add("ViewFullName", ViewNames.CustomerView);

            _regionManager.RequestNavigate(RegionNames.ContentRegion, new Uri(ViewNames.CustomerView + parameters, UriKind.Relative));
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
