using ERService.Infrastructure.Constants;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Settings.ViewModels
{
    public class SettingsViewModel : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        private IRegionManager _regionManager;

        public bool KeepAlive => true;

        public SettingsViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        #region Navigation        

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.GeneralSettingsView);
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.HardwareTypesView);
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.StatusConfigView);
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.NumerationSettingsView);
            _regionManager.RequestNavigate(RegionNames.SettingsTabControlRegion, ViewNames.UserSettingsView);
            
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
