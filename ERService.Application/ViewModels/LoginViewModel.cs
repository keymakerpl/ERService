using ERService.Infrastructure.Base.Common;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Helpers;
using ERService.Infrastructure.Helpers.Data;
using ERService.MSSQLDataAccess;
using ERService.RBAC;
using ERService.Startup;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERService.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IConfig _config;
        private readonly IERBootstrap _bootstrap;
        private readonly IRegionManager _regionManager;
        private readonly IRBACManager _rBACManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDialogService _messageDialogService;
        private DatabaseProviders _databaseProvider;
        private string _dbPassword;
        private string _dbServer;
        private string _dbUser;
        private string _login;
        private bool _isExpanded;

        public LoginViewModel(
            IERBootstrap bootstrap,
            IRegionManager regionManager,
            IRBACManager rBACManager,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IConfig config)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _bootstrap = bootstrap;
            _regionManager = regionManager;
            _rBACManager = rBACManager;
            _config = config;

            LoginCommand = new DelegateCommand<object>(OnLoginCommandExecute);
            ConnectCommand = new DelegateCommand<object>(OnConnectExecute);            

            Providers = new List<KeyValuePair<DatabaseProviders, string>>();

            Initialize();
        }

        public ICommand ConnectCommand { get; }

        public ICommand LoginCommand { get; }

        public DatabaseProviders DatabaseProvider
        {
            get { return _databaseProvider; }
            set { SetProperty(ref _databaseProvider, value); }
        }

        public string DbUser
        {
            get { return _dbUser; }
            set { SetProperty(ref _dbUser, value); }
        }

        public string DbPassword
        {
            get { return _dbPassword; }
            set { SetProperty(ref _dbPassword, value); }
        }

        public string DbServer { get => _dbServer; set => SetProperty(ref _dbServer, value); }        

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

                _logger.Error(ex);
                _logger.Debug(ex);
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
            if (DbHelper.ServerHeartBeat(connectionString, DatabaseProvider))
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
            else
            {
                await _messageDialogService.ShowInformationMessageAsync(this, "Błąd połączenia z bazą danych...", "Nie można ustanowić połączenia dla podanych ustawień.");
            }
        }

        private async void OnLoginCommandExecute(object parameter)
        {
            if (!String.IsNullOrWhiteSpace(Login) && _config.LastLogin != Login)
            {
                _config.LastLogin = Login;
                _config.SaveConfig();
            }

            var connectionString = ConnectionStringProvider.Current;
            if (!DbHelper.ServerHeartBeat(connectionString, DatabaseProvider))
            {
                await _messageDialogService.ShowInformationMessageAsync(this, "Błąd połączenia z bazą danych...", "Więcej informacji znajduje się w pliku log.");
                return;
            }

            var passwordBox = parameter as PasswordBox;
            if (passwordBox != null)
            {
                if (!_rBACManager.Login(Login, passwordBox.Password))
                {
                    ShowWrongLoginDataMessage();
                    _logger.Info($"Login failed for user: {Login}");

                    return;
                }
                ContinueInitialization();
            }
        }

        private void ContinueInitialization()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();

            var t = Task.Run(() =>
            {
                
                _eventAggregator.GetEvent<ShowProgressBarEvent>().Publish(new ShowProgressBarEventArgs { IsShowing = true });                
                _bootstrap.HotStart();
                _eventAggregator.GetEvent<ShowProgressBarEvent>().Publish(new ShowProgressBarEventArgs { IsShowing = false });
            });

            t.ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    _regionManager.RequestNavigate(RegionNames.HeaderRegion, ViewNames.HeaderView);
                    _regionManager.RequestNavigate(RegionNames.NotificationRegion, ViewNames.NotificationListView);                    
                    _regionManager.RequestNavigate(RegionNames.NavigationRegion, ViewNames.NavigationView);
                    _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.StartPageView);

                    _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Publish(new UserAuthorizationEventArgs
                    {
                        UserID = _rBACManager.LoggedUser.Id,
                        UserName = _rBACManager.LoggedUser.FirstName,
                        UserLastName = _rBACManager.LoggedUser.LastName,
                        UserLogin = _rBACManager.LoggedUser.Login,
                    });
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

            t.ContinueWith((task) => 
            {
                if (task.Status == TaskStatus.Faulted)
                {
                    _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
                    _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.LoginView);
                    _eventAggregator.GetEvent<ShowProgressBarEvent>().Publish(new ShowProgressBarEventArgs { IsShowing = false });
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private void ShowWrongLoginDataMessage()
        {
            _messageDialogService.ShowInformationMessageAsync(this, "Nieprawidłowe dane logowania...", "Podałeś nieprawidłowy login lub hasło.");            
        }
    }
}