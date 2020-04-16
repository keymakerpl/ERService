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
            _regionManager = regionManager;
            _settingsRepository = settingsRepository;
            
            Title = "Ustawienia ogólne";
        }

        private int _tabIndex;

        public int TabIndex
        {
            get { return _tabIndex; }
            set { SetProperty(ref _tabIndex, value); }
        }

        #region Navigation

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _regionManager.RequestNavigate(RegionNames.GeneralSettingsTabControlRegion, ViewNames.CompanySettingsView);
            _regionManager.RequestNavigate(RegionNames.GeneralSettingsTabControlRegion, ViewNames.LicenseSettingsView);

            TabIndex = 0;
        }

        public override bool KeepAlive => true;
        
        #endregion
    }
}
