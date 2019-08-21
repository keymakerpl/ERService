using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.RBAC.Data.Repository
{
    public class RBACRepository
    {
        public class UserRepository : GenericRepository<User, ERServiceDbContext>, IUserRepository
        {
            public UserRepository(ERServiceDbContext context) : base(context)
            {
            }

            public override async Task<IEnumerable<User>> GetAllAsync()
            {
                return await Context.Set<User>().Include(u => u.Role.ACLs).Include(r => r.Role).ToListAsync();
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