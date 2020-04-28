using System;
using System.Reflection;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Events;
using ERService.Views;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Application.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        public string ApplicationName { get; }

        private bool _notificationFlyoutIsExpanded;
        public bool NotificationFlyoutIsExpanded
        {
            get { return _notificationFlyoutIsExpanded; }
            set { SetProperty(ref _notificationFlyoutIsExpanded, value); }
        }

        private bool _rightFlyoutIsExpanded;
        public bool RightFlyoutIsExpanded
        {
            get { return _rightFlyoutIsExpanded; }
            set
            {
                SetProperty(ref _rightFlyoutIsExpanded, value);
            }
        }

        public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Subscribe(OnUserLogedin, true);
            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>().Subscribe(OnUserLogedout, true);
            _eventAggregator.GetEvent<AfterSideMenuExpandToggled>().Subscribe(OnSideMenuExpandToggled);

            var assembly = Assembly.GetEntryAssembly();
            ApplicationName = $"{assembly.GetName().Name} {assembly.GetName().Version}";            

            ShowLoginWindow();
        }

        private void OnUserLogedin(UserAuthorizationEventArgs args)
        {
            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Unsubscribe(OnUserLogedin);
            _regionManager.Regions[RegionNames.ContentRegion].NavigationService.Navigating += NavigationService_Navigating;
        }

        private void NavigationService_Navigating(object sender, RegionNavigationEventArgs e)
        {
            NotificationFlyoutIsExpanded = false;
            RightFlyoutIsExpanded = false;
        }

        private void OnSideMenuExpandToggled(AfterSideMenuExpandToggledArgs args)
        {
            switch (args.Flyout)
            {
                case SideFlyouts.NotificationFlyout:
                    ToggleNotificationFlyout();
                    break;
                case SideFlyouts.DetailFlyout:
                    ToggleDetailFlyout(args.DetailID, args.ViewName, args.IsReadOnly);
                    break;
            }
        }

        private void ToggleNotificationFlyout()
        {
            NotificationFlyoutIsExpanded = !NotificationFlyoutIsExpanded;
        }

        private void ToggleDetailFlyout(Guid detailID, string viewName, bool isReadOnly)
        {
            RightFlyoutIsExpanded = !RightFlyoutIsExpanded;

            if (String.IsNullOrWhiteSpace(viewName)) return;

            if (RightFlyoutIsExpanded)
            {
                var parameters = new NavigationParameters();
                parameters.Add("ID", detailID);
                parameters.Add("IsReadOnly", isReadOnly);

                _regionManager.Regions[RegionNames.DetailFlyoutRegion].RemoveAll();
                _regionManager.RequestNavigate(RegionNames.DetailFlyoutRegion, viewName, parameters);
            }
            else
            {
                _regionManager.Regions[RegionNames.DetailFlyoutRegion].RemoveAll();
            }
        }

        private void OnUserLogedout(UserAuthorizationEventArgs args)
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();

            RightFlyoutIsExpanded = false;
            NotificationFlyoutIsExpanded = false;
            ShowLoginWindow();
        }

        private void ShowLoginWindow()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(LoginView));
        }
    }
}
