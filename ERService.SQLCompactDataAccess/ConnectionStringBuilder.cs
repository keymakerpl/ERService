using ERService.Infrastructure.Base.Common;
using ERService.Infrastructure.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;

namespace ERService.MSSQLDataAccess
{
    public static class ConnectionStringBuilder
    {
        public static string Construct()
        {
            IConfig _config = new Config();

            switch (_config.SelectedDatabaseProvider)
            {
                case DatabaseProvidersEnum.MSSQLServer:
                    var mssqlConnStringBuilder = new SqlConnectionStringBuilder();
                    mssqlConnStringBuilder.DataSource = _config.Server;
                    mssqlConnStringBuilder.UserID = Cryptography.StringCipher.Decrypt(_config.User, Cryptography.StringCipher.DbPassPhrase);
                    mssqlConnStringBuilder.Password = Cryptography.StringCipher.Decrypt(_config.Password, Cryptography.StringCipher.DbPassPhrase);
                    mssqlConnStringBuilder.MultipleActiveResultSets = true;
                    mssqlConnStringBuilder.InitialCatalog = "ERService";
                    return mssqlConnStringBuilder.ToString();

                case DatabaseProvidersEnum.MSSQLServerLocalDb:
                    var path = AppDomain.CurrentDomain.BaseDirectory;
                    return $@"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=SSPI;AttachDBFilename={path}localdb.mdf";

                case DatabaseProvidersEnum.MySQLServer:
                    var mysqlConnStringBuilder = new MySqlConnectionStringBuilder();
                    mysqlConnStringBuilder.Server = _config.Server;
                    mysqlConnStringBuilder.UserID = Cryptography.StringCipher.Decrypt(_config.User, Cryptography.StringCipher.DbPassPhrase);
                    mysqlConnStringBuilder.Password = Cryptography.StringCipher.Decrypt(_config.Password, Cryptography.StringCipher.DbPassPhrase);
                    return mysqlConnStringBuilder.ToString();

                default:
                    return "";
            }
        }
    }
}
