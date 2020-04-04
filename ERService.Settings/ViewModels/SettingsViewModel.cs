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

        public bool KeepAlive => true;

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

            if (navigationContext.NavigationService.Region.RegionManager.Regions.ContainsRegionWithName(RegionNames.SettingsEditorViewRegion))
            {
                navigationContext.NavigationService.Region.RegionManager.Regions.Remove(RegionNames.SettingsEditorViewRegion);
            }

            if (navigationContext.NavigationService.Region.RegionManager.Regions.ContainsRegionWithName(RegionNames.GeneralSettingsTabControlRegion))
            {
                navigationContext.NavigationService.Region.RegionManager.Regions.Remove(RegionNames.GeneralSettingsTabControlRegion);
            }

            if (navigationContext.NavigationService.Region.RegionManager.Regions.ContainsRegionWithName(RegionNames.SettingsTabControlRegion))
            {
                _regionManager.Regions[RegionNames.SettingsTabControlRegion].RemoveAll();
            }

            var tabViews = new string[6] 
            {
                ViewNames.GeneralSettingsView,
                ViewNames.HardwareTypesView,
                ViewNames.StatusConfigView,
                ViewNames.NumerationSettingsView,
                ViewNames.UserSettingsView,
                ViewNames.PrintTemplateSettingsView
            };

            foreach (var view in tabViews)
            {
                if (!_regionManager.Regions[RegionNames.SettingsTabControlRegion].Views.Contains(view))
                {
                    _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, view);
                }
            }
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
