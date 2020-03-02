using ERService.RBAC.Data.Repository;
using Prism.Ioc;
using Prism.Modularity;
using static ERService.RBAC.Data.Repository.RBACRepository;

namespace ERService.RBAC
{
    public class RBACModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
 
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IUserRepository, UserRepository>()
                             .Register<IRoleRepository, RoleRepository>()
                             .Register<IACLVerbCollection, ACLVerbCollection>()
                             .Register<IAclVerbRepository, AclVerbRepository>()
                             .Register<IAclRepository, AclRepository>()
                             .RegisterSingleton<IRBACManager, RBACManager>();
        }
    }
}