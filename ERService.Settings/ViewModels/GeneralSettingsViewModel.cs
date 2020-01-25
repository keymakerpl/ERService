using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Settings.Data.Repository;
using Prism.Events;
using Prism.Regions;

namespace ERService.Settings.ViewModels
{
    public class GeneralSettingsViewModel : DetailViewModelBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IRegionManager _regionManager;

        public GeneralSettingsViewModel(
            IEventAggregator eventAggregator,
            ISettingsRepository settingsRepository,
            IMessageDialogService messageDialogService,
            IRegionManager regionManager) : base(eventAggregator, messageDialogService)
        {
            _settingsRepository = settingsRepository;
            
            Title = "Ustawienia ogólne";
            _regionManager = regionManager;
        }

        #region Navigation

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _regionManager.RequestNavigate(RegionNames.GeneralSettingsTabControlRegion, ViewNames.CompanySettingsView);
            _regionManager.RequestNavigate(RegionNames.GeneralSettingsTabControlRegion, ViewNames.LicenseSettingsView);
        }

        public override bool KeepAlive => true;
        #endregion
    }
}
