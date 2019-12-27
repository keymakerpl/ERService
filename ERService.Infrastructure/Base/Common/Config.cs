using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;

namespace ERService.Infrastructure.Base.Common
{
    public class Config : IConfig
    {
        public Config()
        {
            Initialize();
        }

        public DatabaseProvidersEnum SelectedDatabaseProvider { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string LastLogin { get; set; }

        public void SaveConfig()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                var dbSettings =
                    ((AppSettingsSection)config.GetSection("applicationSettings/dbSettings")).Settings;

                if (dbSettings != null)
                {
                    dbSettings["Provider"].Value = ((int)SelectedDatabaseProvider).ToString();
                    dbSettings["Server"].Value = Server;
                    dbSettings["User"].Value = User;
                    dbSettings["Password"].Value = Password;
                }

                var loginSettings =
                    ((AppSettingsSection)config.GetSection("applicationSettings/loginSettings")).Settings;

                if (loginSettings != null)
                {
                    loginSettings["LastLogin"].Value = LastLogin;
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("applicationSettings");
                Initialize();
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                //TODO: Logger
            }
        }

        private void Initialize()
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                var dbSettings =
                    ((AppSettingsSection)config.GetSection("applicationSettings/dbSettings")).Settings;

                if (dbSettings != null)
                {
                    var selectedProvider = dbSettings["Provider"].Value;
                    SelectedDatabaseProvider = (DatabaseProvidersEnum)int.Parse(selectedProvider);
                    Server = dbSettings["Server"].Value;
                    User = dbSettings["User"].Value;
                    Password = dbSettings["Password"].Value;
                }

                var loginSettings =
                    ((AppSettingsSection)config.GetSection("applicationSettings/loginSettings")).Settings;

                if (loginSettings != null)
                {
                    LastLogin = loginSettings["LastLogin"].Value;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                //TODO: Logger
            }
        }
    }

    public enum DatabaseProvidersEnum
    {
        [Description("Microsoft SQL Server")]
        MSSQLServer,
        [Description("Microsoft SQL LocalDb")]
        MSSQLServerLocalDb,
        [Description("MySQL Server")]
        MySQLServer
    }
}