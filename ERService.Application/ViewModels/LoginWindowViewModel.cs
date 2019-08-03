using ERService.Infrastructure.Events;
using ERService.RBAC;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace ERService.ViewModels
{
    public class LoginWindowViewModel : BindableBase
    {
        private string _login = "administrator";

        public string Login
        {
            get { return _login; } 
            set { SetProperty(ref _login, value); } 
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private IEventAggregator _eventAggregator;
        private IRBACManager _rbacManager;

        public DelegateCommand LoginCommand { get; private set; }

        public LoginWindowViewModel(IRBACManager iRBACManager, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _rbacManager = iRBACManager;

            LoginCommand = new DelegateCommand(OnLoginCommandExecute);
        }

        private void OnLoginCommandExecute()
        {
            if (_rbacManager.Authorize(Login, Password))
            {
                _eventAggregator.GetEvent<AfterAuthorisedEvent>().Publish(new AfterAuthorisedEventArgs { UserLogin = "administrator" });
            }
        }
    }
}
