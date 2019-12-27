using ERService.Infrastructure.Base.Common;
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
                    var newConnStringBuilder = new SqlConnectionStringBuilder();
                    newConnStringBuilder.DataSource = _config.Server;
                    newConnStringBuilder.UserID = _config.User;
                    newConnStringBuilder.Password = _config.Password;
                    newConnStringBuilder.MultipleActiveResultSets = true;
                    newConnStringBuilder.InitialCatalog = "ERService";
                    return newConnStringBuilder.ToString();

                case DatabaseProvidersEnum.MSSQLServerLocalDb:
                    var path = AppDomain.CurrentDomain.BaseDirectory;
                    return $@"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=SSPI;AttachDBFilename={path}localdb.mdf";

                case DatabaseProvidersEnum.MySQLServer:
                    return "";

                default:
                    return "";
            }
        }
    }
}
