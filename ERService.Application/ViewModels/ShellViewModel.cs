using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Events;
using ERService.Startup;
using ERService.Views;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Application.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private readonly IERBootstrap _bootstrap;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        public string ApplicationName { get; }
        public string ApplicationVersion { get; }

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

        private bool _isProgressBarVisible;
        public bool IsProgressBarVisible
        {
            get { return _isProgressBarVisible; }
            set { SetProperty(ref _isProgressBarVisible, value); }
        }

        private bool _isCenterLogoVisible = true;
        public bool IsCenterLogoVisible
        {
            get { return _isCenterLogoVisible; }
            private set { SetProperty(ref _isCenterLogoVisible, value); }
        }

        public ShellViewModel(IERBootstrap bootstrap, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _bootstrap = bootstrap;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Subscribe(OnUserLogedin, true);
            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>().Subscribe(OnUserLogedout, true);
            _eventAggregator.GetEvent<AfterSideMenuExpandToggled>().Subscribe(OnSideMenuExpandToggled);
            _eventAggregator.GetEvent<ShowProgressBarEvent>().Subscribe(OnShowProgressBarChanged, true);

            var assembly = Assembly.GetEntryAssembly();
            var major = assembly.GetName().Version.Major;
            var minor = assembly.GetName().Version.Minor;
            var build = assembly.GetName().Version.Build;

            ApplicationName = $"{assembly.GetName().Name}";
            ApplicationVersion = $"v{major}.{minor}.{build}";

            Initialize();
        }

        private void OnShowProgressBarChanged(ShowProgressBarEventArgs args)
        {
            IsProgressBarVisible = args.IsShowing;
        }

        private void Initialize()
        {
            var t = Task.Run(() => 
            {
                _eventAggregator.GetEvent<ShowProgressBarEvent>().Publish(new ShowProgressBarEventArgs { IsShowing = true });
                _bootstrap.ColdStart();
                _eventAggregator.GetEvent<ShowProgressBarEvent>().Publish(new ShowProgressBarEventArgs { IsShowing = false });
            });

            t.ContinueWith((task) => 
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    _regionManager.RegisterViewWithRegion(RegionNames.DetailHeaderRegion, typeof(DetailHeaderView));

                    IsCenterLogoVisible = false;
                    ShowLoginWindow();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnUserLogedin(UserAuthorizationEventArgs args)
        {
            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Unsubscribe(OnUserLogedin);
            _regionManager.Regions[RegionNames.ContentRegion].NavigationService.Navigating += NavigationService_Navigating;
        }

        private void OnUserLogedout(UserAuthorizationEventArgs args)
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();

            RightFlyoutIsExpanded = false;
            NotificationFlyoutIsExpanded = false;
            ShowLoginWindow();
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

        private void ShowLoginWindow()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RequestNavigate(ViewNames.LoginView);
        }
    }
}
