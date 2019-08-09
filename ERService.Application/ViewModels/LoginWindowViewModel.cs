using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Helpers;
using ERService.RBAC;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Windows.Input;

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
        private IMessageDialogService _messageDialogService;

        public ICommand LoginCommand { get; private set; }

        public LoginWindowViewModel(IRBACManager iRBACManager, IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
            _rbacManager = iRBACManager;
            _messageDialogService = messageDialogService;

            LoginCommand = new DelegateCommand(OnLoginCommandExecute);
        }

        private void OnLoginCommandExecute()
        {
            if (!_rbacManager.Authorize(Login, Password))
            {
                _messageDialogService.ShowInformationMessageAsync(this, "Nieprawidłowe dane logowania...", "Podałeś nieprawidłowy login lub hasło.");
            }
        }
    }
}
