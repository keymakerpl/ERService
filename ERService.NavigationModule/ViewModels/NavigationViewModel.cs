using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace ERService.Navigation.ViewModels
{
    public class NavigationViewModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        private string _currentContentName;
        private bool _isEnabled;

        public NavigationViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) 
            : base(eventAggregator, messageDialogService)
        {
            _regionManager = regionManager;

            _regionManager.Regions[RegionNames.ContentRegion].NavigationService.Navigated += NavigationService_Navigated;
            _regionManager.Regions[RegionNames.ContentRegion].NavigationService.NavigationFailed += NavigationService_NavigationFailed;

            IsEnabled = false;

            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Subscribe((o) => { IsEnabled = true; }, true);
            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>().Subscribe((o) => { IsEnabled = false; }, true);            

            OpenDetailViewCommand = new DelegateCommand<string>(OnOpenDetailViewExecute);
        }

        private void NavigationService_NavigationFailed(object sender, RegionNavigationFailedEventArgs e)
        {
            var message = $"Navigation failed: {e.Error}";
            _logger.Debug(message);
            _logger.Error(message);
        }

        private void NavigationService_Navigated(object sender, RegionNavigationEventArgs e)
        {
            _logger.Debug($"Navigated to: {e.Uri}");
        }

        public IRegionManager _regionManager { get; }

        public string CurrentContentName
        {
            get { return _currentContentName; }
            set { SetProperty(ref _currentContentName, value); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public DelegateCommand<string> OpenDetailViewCommand { get; }

        private void OnOpenDetailViewExecute(string viewName)
        {
            _regionManager.Regions[RegionNames.ContentRegion].RequestNavigate(viewName);
        }
    }
}