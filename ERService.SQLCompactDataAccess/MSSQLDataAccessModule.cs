using Prism.Ioc;
using Prism.Modularity;

namespace ERService.MSSQLDataAccess
{
    public class MSSQLDataAccessModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //var instance = System.Data.Entity.SqlServerCompact.SqlCeProviderServices.Instance;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}