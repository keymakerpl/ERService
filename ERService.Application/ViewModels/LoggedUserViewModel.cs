using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.RBAC;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;

namespace ERService.ViewModels
{
    public class LoggedUserViewModel : DetailViewModelBase
    {
        private readonly IRegionManager _regionManager;

        private string _userName;
        private readonly IRBACManager _rBACManager;

        public LoggedUserViewModel(IRBACManager rBACManager, IRegionManager regionManager, IEventAggregator eventAggregator, IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
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

        private void OnUserLoggedout(UserAuthorizationEventArgs obj)
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

            var parameters = new NavigationParameters();
            parameters.Add("ID", _rBACManager.LoggedUser.Id);

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.UserDetailView, parameters);
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