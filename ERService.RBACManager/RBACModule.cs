using ERService.Infrastructure.Events;
using ERService.RBAC.Data.Repository;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using static ERService.RBAC.Data.Repository.RBACRepository;

namespace ERService.RBAC
{
    public class RBACModule : IModule
    {
        private readonly IEventAggregator _eventAggregator;

        public RBACModule(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var rbac = containerProvider.Resolve<IRBACManager>();            
            rbac.LoadAsync();            
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