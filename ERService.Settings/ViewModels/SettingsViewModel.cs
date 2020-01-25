using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.RBAC;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Settings.ViewModels
{
    public class SettingsViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        private IRegionManager _regionManager;
        private readonly IRBACManager _rBACManager;
        private readonly IMessageDialogService _dialogService;

        public bool KeepAlive => false;

        public SettingsViewModel(IRegionManager regionManager, IRBACManager rBACManager, IMessageDialogService dialogService)
        {
            _regionManager = regionManager;
            _rBACManager = rBACManager;
            _dialogService = dialogService;
        }

        #region Navigation        

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (!_rBACManager.LoggedUserHasPermission(AclVerbNames.ApplicationConfiguration))
            {
                await _dialogService.ShowAccessDeniedMessageAsync(this, message: "Nie masz praw dostępu do tego modułu.");
                navigationContext.NavigationService.Journal.GoBack();
            }

            navigationContext.NavigationService.Region.RegionManager.Regions.Remove(RegionNames.SettingsEditorViewRegion);
            navigationContext.NavigationService.Region.RegionManager.Regions.Remove(RegionNames.GeneralSettingsTabControlRegion);

            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.GeneralSettingsView);
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.HardwareTypesView);
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.StatusConfigView);
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.NumerationSettingsView);
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.UserSettingsView);
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.PrintTemplateSettingsView);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        #endregion
    }
}
