using CommonServiceLocator;
using ERService.Infrastructure.Base.Common;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Helpers;
using ERService.MSSQLDataAccess;
using ERService.RBAC;
using MySql.Data.MySqlClient;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERService.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private string _dbPassword;
        private string _dbServer;
        private string _dbUser;
        private bool _isExpanded;
        private string _login;
        private readonly IConfig _config;
        private IRBACManager _rBACManager;
        private IEventAggregator _eventAggregator;
        private DatabaseProviders _databaseProvider;
        private IMessageDialogService _messageDialogService;

        public LoginViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService, IConfig config)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _config = config;

            LoginCommand = new DelegateCommand<object>(OnLoginCommandExecute);
            ConnectCommand = new DelegateCommand<object>(OnConnectExecute);

            Providers = new List<KeyValuePair<DatabaseProviders, string>>();

            Initialize();
        }

        public ICommand ConnectCommand { get; private set; }

        public DatabaseProviders DatabaseProvider
        {
            get { return _databaseProvider; }
            set { SetProperty(ref _databaseProvider, value); }
        }

        public string DbPassword
        {
            get { return _dbPassword; }
            set { SetProperty(ref _dbPassword, value); }
        }

        public string DbServer { get => _dbServer; set => SetProperty(ref _dbServer, value); }

        public string DbUser
        {
            get { return _dbUser; }
            set { SetProperty(ref _dbUser, value); }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }

        public string Login
        {
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        public ICommand LoginCommand { get; private set; }

        public List<KeyValuePair<DatabaseProviders, string>> Providers { get; set; }

        public void ToggleIsExpanded()
        {
            IsExpanded = !IsExpanded;
        }

        private void FillProvidersCombo()
        {
            Providers.Clear();
            foreach (var provider in Enum.GetValues(typeof(DatabaseProviders)))
            {
                var desc = ((DatabaseProviders)provider).GetAttribute<DescriptionAttribute>();
                Providers.Add(new KeyValuePair<DatabaseProviders, string>((DatabaseProviders)provider, desc.Description));
            }
        }

        private void Initialize()
        {
            FillProvidersCombo();


            try
            {
                Login = _config.LastLogin;
                DbServer = _config.Server;
                DbUser = Cryptography.StringCipher.Decrypt(_config.User, Cryptography.StringCipher.DbPassPhrase);
                DbPassword = Cryptography.StringCipher.Decrypt(_config.Password, Cryptography.StringCipher.DbPassPhrase);
                DatabaseProvider = _config.DatabaseProvider;
            }
            catch (Exception ex)
            {
                Login = "";
                DbServer = "";
                DbUser = "";
                DbPassword = "";
                DatabaseProvider = DatabaseProviders.MSSQLServerLocalDb;
                //TODO: log

                Console.WriteLine($"[DEBUG] LoginViewModel error: {ex.Message}");

            }
        }

        private async void OnConnectExecute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (passwordBox != null)
            {
                DbPassword = passwordBox.Password;
            }

            var connectionString = ConnectionStringBuilder.Construct(DatabaseProvider, DbServer, DbUser, DbPassword);
            if (ServerHeartBeat(connectionString))
            {                
                var result = await _messageDialogService.ShowConfirmationMessageAsync(this, "Połączenie poprawne...", "Połączono z bazą danych. " +
                    "Zmiana parametrów połączenia wymaga ponownego uruchomienia aplikacji. Czy zapisać zmiany i uruchomić ponownie teraz?");

                if (result == DialogResult.OK)
                {
                    _config.Server = DbServer;
                    _config.User = Cryptography.StringCipher.Encrypt(DbUser, Cryptography.StringCipher.DbPassPhrase);
                    _config.Password = Cryptography.StringCipher.Encrypt(DbPassword, Cryptography.StringCipher.DbPassPhrase);
                    _config.DatabaseProvider = DatabaseProvider;
                    _config.SaveConfig();

                    System.Windows.Application.Current.Shutdown();
                }
            }
        }

        private void OnLoginCommandExecute(object parameter)
        {
            if (String.IsNullOrWhiteSpace(Login))
                ShowWrongLoginDataMessage();

            if (_config.LastLogin != Login)
            {
                _config.LastLogin = Login;
                _config.SaveConfig();
            }

            var connectionString = ConnectionStringProvider.Current;
            if (!ServerHeartBeat(connectionString)) return;

            try
            {
                _rBACManager = ServiceLocator.Current.GetInstance<IRBACManager>();
                _rBACManager.Load();
            }
            catch (Exception ex)
            {
                //TODO: error handling
                _messageDialogService.ShowInformationMessageAsync(this, "Błąd połączenia z bazą danych...",
                    $"Error: {ex.Message} {Environment.NewLine} {ex.InnerException?.Message ?? ""}");
            }

            var passwordBox = parameter as PasswordBox;
            if (passwordBox != null && _rBACManager != null)
            {
                if (!_rBACManager.Login(Login, passwordBox.Password))
                {
                    ShowWrongLoginDataMessage();
                }
            }
        }

        //TODO: move to helpers
        private bool ServerHeartBeat(string connectionString)
        {
            if (DatabaseProvider == DatabaseProviders.MSSQLServer)
            {
                using (SqlConnection connection = new SqlConnection(connectionString.Replace(";Initial Catalog=ERService", String.Empty)))
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

                        Console.WriteLine($"[DEBUG] Connection error: {ex.Message}");

                        return false;
                    }
                }
            }
            else if (DatabaseProvider == DatabaseProviders.MySQLServer)
            {
                using (var connection = new MySqlConnection(connectionString.Replace(";database=ERService", String.Empty)))
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
            else if (DatabaseProvider == DatabaseProviders.MSSQLServerLocalDb)
            {
                return true;
            }

            return false;
        }

        private void ShowWrongLoginDataMessage()
        {
            _messageDialogService.ShowInformationMessageAsync(this, "Nieprawidłowe dane logowania...", "Podałeś nieprawidłowy login lub hasło.");
        }
    }
}