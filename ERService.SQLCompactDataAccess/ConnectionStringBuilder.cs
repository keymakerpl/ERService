using ERService.Infrastructure.Base.Common;
using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;

namespace ERService.MSSQLDataAccess
{
    public static class ConnectionStringBuilder
    {
        public static string Construct(DatabaseProviders provider, string server, string user, string password)
        {
            var connectionstring = String.Empty;

            switch (provider)
            {
                case DatabaseProviders.MSSQLServer:
                    var mssqlConnStringBuilder = new SqlConnectionStringBuilder();
                    mssqlConnStringBuilder.DataSource = server;
                    mssqlConnStringBuilder.UserID = user;
                    mssqlConnStringBuilder.Password = password;
                    mssqlConnStringBuilder.MultipleActiveResultSets = true;
                    mssqlConnStringBuilder.InitialCatalog = "ERService";
                    mssqlConnStringBuilder.ApplicationName = AppDomain.CurrentDomain.FriendlyName;
                    connectionstring = mssqlConnStringBuilder.ToString();
                    break;

                case DatabaseProviders.MSSQLServerLocalDb:
                    var path = AppDomain.CurrentDomain.BaseDirectory;
                    connectionstring = $@"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=SSPI;AttachDBFilename={path}localdb.mdf";
                    break;

                case DatabaseProviders.MySQLServer:
                    var mysqlConnStringBuilder = new MySqlConnectionStringBuilder();
                    mysqlConnStringBuilder.Server = server;
                    mysqlConnStringBuilder.UserID = user;
                    mysqlConnStringBuilder.Password = password;
                    mysqlConnStringBuilder.Database = "ERService";
                    connectionstring = mysqlConnStringBuilder.ToString();
                    break;
            }

            return connectionstring;
        }
    }
}
