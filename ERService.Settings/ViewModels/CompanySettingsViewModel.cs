using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Settings.Manager;
using Prism.Events;
using Prism.Regions;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class CompanySettingsViewModel : DetailViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ISettingsManager _settingsManager;
        private object _companySetting;

        public CompanySettingsViewModel(
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IRegionManager regionManager,
            ISettingsManager settingsManager) : base(eventAggregator, messageDialogService)
        {
            Title = "Dane firmy";

            _regionManager = regionManager;
            _settingsManager = settingsManager;
        }

        public override bool KeepAlive => true;

        public object CompanySetting
        {
            get { return _companySetting; }
            private set { SetProperty(ref _companySetting, value); }
        }

        protected override void OnSaveExecute()
        {
            _settingsManager.SaveAsync();
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.SettingsView);
        }

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }

        public override async Task LoadAsync()
        {
            CompanySetting = await _settingsManager.GetConfigAsync("CompanyInfo");
        }
    }
}
