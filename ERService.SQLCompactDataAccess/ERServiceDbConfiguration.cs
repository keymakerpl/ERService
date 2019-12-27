using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ERService.MSSQLDataAccess
{

    public class ERServiceDbConfiguration : DbConfiguration
    {
        public ERServiceDbConfiguration()
        {
            //MSSQL
            this.SetDefaultConnectionFactory(new SqlConnectionFactory());
            this.SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);  
        }        
    }
}
