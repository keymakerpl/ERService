using CommonServiceLocator;
using ERService.Infrastructure.Base.Common;
using ERService.Infrastructure.Helpers;

namespace ERService.MSSQLDataAccess
{
    public static class ConnectionStringProvider
    {
        private static IConfig _config
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<IConfig>();
                }
                catch (System.Exception)
                {
#if DEBUG
                    //TODO: Log
                    return new Config();
#endif
                }
            }
        }

        public static string Current
        {
            get
            {
                var provider = _config.DatabaseProvider;
                var dbServer = _config.Server;
                var dbUser = Cryptography.StringCipher.Decrypt(_config.User, Cryptography.StringCipher.DbPassPhrase);
                var dbPassword = Cryptography.StringCipher.Decrypt(_config.Password, Cryptography.StringCipher.DbPassPhrase);

                return ConnectionStringBuilder.Construct(provider, dbServer, dbUser, dbPassword);
            }
        }
    }
}
