using ERService.Infrastructure.Base.Common;
using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;

namespace ERService.Infrastructure.Helpers.Data
{
    public static class DbHelper
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool ServerHeartBeat(string connectionString, DatabaseProviders databaseProvider)
        {
            if (databaseProvider == DatabaseProviders.MSSQLServer)
            {
                using (var connection = new SqlConnection(connectionString.Replace(";Initial Catalog=ERService", String.Empty)))
                {
                    try
                    {
                        connection.Open();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        _logger.Error(ex);
                        return false;
                    }
                }
            }
            else if (databaseProvider == DatabaseProviders.MySQLServer)
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
                        _logger.Error(ex);
                        return false;
                    }
                }
            }
            else if (databaseProvider == DatabaseProviders.MSSQLServerLocalDb)
            {
                return true;
            }

            return false;
        }
    }
}
