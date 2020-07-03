using ERService.Infrastructure.Repositories;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.MSSQLDataAccess
{
    public class MSSQLDataAccessModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IUnitOfWork, UnitOfWork>();
        }
    }
}