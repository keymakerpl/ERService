using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.RBAC;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;

namespace ERService.Notification.ViewModels
{
    public class LoggedUserViewModel : DetailViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IRBACManager _rBACManager;
        private string _userName;

        public LoggedUserViewModel(
            IRBACManager rBACManager,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            UserLogoutCommand = new DelegateCommand(OnUserLogoutExecute);
            UserSettingsCommand = new DelegateCommand(OnUserSettingsExecute);

            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Subscribe(OnUserLogged, true);
            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>().Subscribe(OnUserLoggedout, true);            

            _regionManager = regionManager;
            _rBACManager = rBACManager;
        }

        public DelegateCommand UserLogoutCommand { get; }

        public DelegateCommand UserSettingsCommand { get; }

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private void OnUserLogged(UserAuthorizationEventArgs args)
        {
            UserName = !String.IsNullOrEmpty(args.UserLastName) ? $"{args.UserName} {args.UserLastName}" : args.UserLogin;
        }

        private void OnUserLoggedout(UserAuthorizationEventArgs args)
        {
            UserName = String.Empty;
        }

        private void OnUserLogoutExecute()
        {
            _rBACManager.Logout();
        }

        private void OnUserSettingsExecute()
        {
            if (_rBACManager.LoggedUser == null) return;

            _eventAggregator.GetEvent<AfterSideMenuExpandToggled>().Publish(new AfterSideMenuExpandToggledArgs
            {
                DetailID = _rBACManager.LoggedUser.Id,
                Flyout = SideFlyouts.DetailFlyout,
                ViewName = ViewNames.UserDetailView
            });
        }

        #region NAVIGATION
        public override bool KeepAlive => true;

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            var name = navigationContext.Parameters.GetValue<string>("UserName");
            if (!String.IsNullOrWhiteSpace(name)) UserName = name;
        }
        #endregion
    }
}