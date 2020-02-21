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
        private readonly string _applicationName;
        public string ApplicationName { get { return _applicationName; } }

        private bool _isExpanded;

        public bool RightFlyoutIsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }

        private bool _searchIsExpanded;

        public bool SearchIsExpanded
        {
            get { return _searchIsExpanded; }
            set { SetProperty(ref _searchIsExpanded, value); }
        }

        public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>().Subscribe(OnUserLogedout, true);
            _eventAggregator.GetEvent<AfterSideMenuButtonToggled>().Subscribe(OnSideMenuButtonToggled, true);

            var assembly = Assembly.GetEntryAssembly();
            _applicationName = $"{assembly.GetName().Name}";

            ShowLoginWindow();
        }

        private void OnSideMenuButtonToggled(AfterSideMenuButtonToggledArgs args)
        {
            switch (args.FlyoutSide)
            {
                case SideFlyouts.RightSide:
                    RightFlyoutIsExpanded = !RightFlyoutIsExpanded;
                    break;
                case SideFlyouts.BottomSearch:
                    SearchIsExpanded = !SearchIsExpanded;
                    break;
            }
        }

        private void OnUserLogedout(UserAuthorizationEventArgs obj)
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            ShowLoginWindow();
        }

        private void ShowLoginWindow()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(LoginView));
        }
    }
}
