using System.Collections.Generic;
using System.Threading.Tasks;
using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System.Data.Entity;
using System.Linq;

namespace ERService.RBAC.Data.Repository
{
    public class RBACRepository 
    {
        public class UserRepository : GenericRepository<User, ERServiceDbContext>, IUserRepository
        {
            public UserRepository(ERServiceDbContext context) : base(context)
            {
            }
        }

        public class RoleRepository : GenericRepository<Role, ERServiceDbContext>, IRoleRepository
        {
            public RoleRepository(ERServiceDbContext context) : base(context)
            {
                
            }

            public override async Task<IEnumerable<Role>> GetAllAsync()
            {
                return await Context.Set<Role>().Include(a => a.ACLs.Select(v => v.AclVerb)).ToListAsync();
            }
        }

        public class AclRepository : GenericRepository<Acl, ERServiceDbContext>, IAclRepository
        {
            public AclRepository(ERServiceDbContext context) : base(context)
            {
            }
        }

        public class AclVerbRepository : GenericRepository<AclVerb, ERServiceDbContext>, IAclVerbRepository
        {
            public AclVerbRepository(ERServiceDbContext context) : base(context)
            {
            }
        }
    }
}
