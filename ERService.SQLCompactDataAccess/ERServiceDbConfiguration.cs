using ERService.Infrastructure.Base.Common;
using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ERService.MSSQLDataAccess
{
    public class ERServiceDbConfiguration : DbConfiguration
    {
        private object _config { get; } = CommonServiceLocator.ServiceLocator.Current.GetInstance(typeof(IConfig)) as IConfig;

        public ERServiceDbConfiguration()
        {
            if (((IConfig)_config).SelectedDatabaseProvider == DatabaseProvidersEnum.MySQLServer)
            {
                try
                {
                    var dataSet = (DataSet)ConfigurationManager.GetSection("system.data");
                    dataSet.Tables[0].Rows.Add(
                        "MySQL Data Provider",
                        ".Net Framework Data Provider for MySQL",
                        "MySql.Data.MySqlClient",
                        typeof(MySqlClientFactory).AssemblyQualifiedName
                    );
                }
                catch (System.Exception ex)
                {
                    //TODO: log
                }

                SetDefaultConnectionFactory(new MySqlConnectionFactory());
                SetProviderServices("MySql.Data.MySqlClient", new MySqlProviderServices());
            }
            else
            {
                SetDefaultConnectionFactory(new SqlConnectionFactory());
                SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
            }
        }        
    }
}
