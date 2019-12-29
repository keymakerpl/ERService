using ERService.Infrastructure.Base.Common;
using ERService.Infrastructure.Dialogs;
using ERService.MSSQLDataAccess;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;
using ERService.Infrastructure.Helpers;
using System.ComponentModel;
using ERService.RBAC;
using CommonServiceLocator;
using MySql.Data.MySqlClient;

namespace ERService.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private string _login;

        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }

        public void ToggleIsExpanded()
        {
            IsExpanded = !IsExpanded;
        }

        public string Login
        {
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        private readonly IRBACManager _rBACManager;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private readonly IConfig _config;

        private string _dbUser;

        public string DbUser
        {
            get { return _dbUser; }
            set { SetProperty(ref _dbUser, value); }
        }

        private string _dbPassword;
        private string _dbServer;

        public string DbPassword
        {
            get { return _dbPassword; }
            set { SetProperty(ref _dbPassword, value); }
        }

        private DatabaseProvidersEnum _selectedProvider;

        public DatabaseProvidersEnum SelectedProvider
        {
            get { return _selectedProvider; }
            set { _selectedProvider = value; }
        }

        public List<KeyValuePair<DatabaseProvidersEnum, string>> Providers { get; set; }

        public ICommand LoginCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public string DbServer { get => _dbServer; set => SetProperty(ref _dbServer, value); }

        public LoginViewModel(IRBACManager rBACManager, IEventAggregator eventAggregator, IMessageDialogService messageDialogService, IConfig config)
        {
            _rBACManager = rBACManager;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _config = config;

            LoginCommand = new DelegateCommand<object>(OnLoginCommandExecute);
            ConnectCommand = new DelegateCommand<object>(OnConnectExecute);

            Providers = new List<KeyValuePair<DatabaseProvidersEnum, string>>();

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                Login = _config.LastLogin;
                DbServer = _config.Server;
                DbUser = Cryptography.StringCipher.Decrypt(_config.User, Cryptography.StringCipher.DbPassPhrase);
                DbPassword = Cryptography.StringCipher.Decrypt(_config.Password, Cryptography.StringCipher.DbPassPhrase);
                SelectedProvider = _config.SelectedDatabaseProvider;
            }
            catch (Exception ex)
            {
                Login = "";
                DbServer = "";
                DbUser = "";
                DbPassword = "";
                SelectedProvider = DatabaseProvidersEnum.MSSQLServerLocalDb;
                //TODO: log
            }

            FillProvidersCombo();
        }

        private void FillProvidersCombo()
        {
            Providers.Clear();
            foreach (var provider in Enum.GetValues(typeof(DatabaseProvidersEnum)))
            {
                var desc = ((DatabaseProvidersEnum)provider).GetAttribute<DescriptionAttribute>();
                Providers.Add(new KeyValuePair<DatabaseProvidersEnum, string>((DatabaseProvidersEnum)provider, desc.Description));
            }
        }

        private void OnConnectExecute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (passwordBox != null)
            {
                DbPassword = passwordBox.Password;
            }

            if (ServerHeartBeat())
            {
                _config.SaveConfig();
                _messageDialogService.ShowInformationMessageAsync(this, "Połączenie poprawne...", "Połączono z bazą danych.");
                return;
            }            
        }

        //TODO: move to helpers
        private bool ServerHeartBeat()
        {
            _config.Server = DbServer;
            _config.User = Cryptography.StringCipher.Encrypt(DbUser, Cryptography.StringCipher.DbPassPhrase);
            _config.Password = Cryptography.StringCipher.Encrypt(DbPassword, Cryptography.StringCipher.DbPassPhrase);
            _config.SelectedDatabaseProvider = SelectedProvider;
            _config.SaveConfig();

            if (SelectedProvider != DatabaseProvidersEnum.MySQLServer)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStringBuilder.Construct()))
                {
                    try
                    {
                        connection.Open();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        //TODO: logger
                        _messageDialogService.ShowInformationMessageAsync(this, "Brak połączenia...", "Nie udało się połączyć z bazą danych, więcej informacji znajduje się w pliku dziennika.");
                        return false;
                    }
                }
            }
            else
            {
                using (var connection = new MySqlConnection(ConnectionStringBuilder.Construct()))
                {
                    try
                    {
                        connection.Open();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _messageDialogService.ShowInformationMessageAsync(this, "Brak połączenia...", ex.Message);
                        return false;
                    }
                }
            }
        }

        private void OnLoginCommandExecute(object parameter)
        {
            if (String.IsNullOrWhiteSpace(Login))
                ShowWrongLoginDataMessage();

            _config.LastLogin = Login;
            _config.SaveConfig();

            if (!ServerHeartBeat()) return;

            _rBACManager.Load();

            var passwordBox = parameter as PasswordBox;
            if (passwordBox != null)
            {
                if (!_rBACManager.Login(Login, passwordBox.Password))
                {
                    ShowWrongLoginDataMessage();
                }
            }
        }

        private void ShowWrongLoginDataMessage()
        {
            _messageDialogService.ShowInformationMessageAsync(this, "Nieprawidłowe dane logowania...", "Podałeś nieprawidłowy login lub hasło.");
        }
    }

}
