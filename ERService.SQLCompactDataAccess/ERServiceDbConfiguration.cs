using CommonServiceLocator;
using ERService.Infrastructure.Base.Common;
using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

namespace ERService.MSSQLDataAccess
{
    public class ERServiceDbConfiguration : DbConfiguration
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public ERServiceDbConfiguration()
        {
            if (_config.DatabaseProvider == DatabaseProviders.MySQLServer)
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
                    _logger.Error(ex);
                }

                SetDefaultConnectionFactory(new MySqlConnectionFactory());
                SetProviderServices("MySql.Data.MySqlClient", new MySqlProviderServices());
            }
            else
            {
                SetProviderServices("System.Data.SqlClient", SqlProviderServices.Instance);
                SetExecutionStrategy("System.Data.SqlClient", () => new DefaultExecutionStrategy());
                SetDefaultConnectionFactory(new LocalDbConnectionFactory("mssqllocaldb"));
            }
        }

        private IConfig _config
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance(typeof(IConfig)) as IConfig;
                }
                catch (System.Exception)
                {
#if DEBUG
                    return new Config();
#endif
                }
            }
        }
    }
}