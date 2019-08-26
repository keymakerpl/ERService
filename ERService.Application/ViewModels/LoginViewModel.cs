using ERService.Infrastructure.Dialogs;
using ERService.RBAC;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERService.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private string _login = "administrator";

        public string Login
        {
            get { return _login; } 
            set { SetProperty(ref _login, value); } 
        }

        private string _password;

        [Obsolete]
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private IEventAggregator _eventAggregator;
        private IRBACManager _rbacManager;
        private IMessageDialogService _messageDialogService;

        public ICommand LoginCommand { get; private set; }

        public LoginViewModel(IRBACManager iRBACManager, IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
            _rbacManager = iRBACManager;
            _messageDialogService = messageDialogService;

            LoginCommand = new DelegateCommand<object>(OnLoginCommandExecute);
        }

        private void OnLoginCommandExecute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (passwordBox != null)
            {
                if (!_rbacManager.Authorize(Login, passwordBox.Password))
                {
                    _messageDialogService.ShowInformationMessageAsync(this, "Nieprawidłowe dane logowania...", "Podałeś nieprawidłowy login lub hasło.");
                }
            }            
        }
    }
}
