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
    public static class RBACRepository
    {
        public class UserRepository : GenericRepository<User, ERServiceDbContext>, IUserRepository
        {
            public UserRepository(ERServiceDbContext context) : base(context)
            {
            }

            public override async Task<IEnumerable<User>> GetAllAsync()
            {
                return await Context.Set<User>()
                    .Include(u => u.Role.ACLs.Select(a => a.AclVerb))
                    .Include(r => r.Role)
                    .ToListAsync();
            }

            public IEnumerable<User> GetAll()
            {
                return Context.Set<User>()
                      .Include(u => u.Role.ACLs.Select(a => a.AclVerb))
                      .Include(r => r.Role)
                      .ToList();
            }

            public override async Task<User> GetByIdAsync(Guid id)
            {
                return await Context.Set<User>()
                    .Where(i => i.Id == id)
                    .Include(u => u.Role.ACLs.Select(a => a.AclVerb))
                    .Include(r => r.Role)
                    .FirstOrDefaultAsync();
            }
        }

        public class RoleRepository : GenericRepository<Role, ERServiceDbContext>, IRoleRepository
        {
            public RoleRepository(ERServiceDbContext context) : base(context)
            {
            }

            public override async Task<IEnumerable<Role>> GetAllAsync()
            {
                return await Context.Set<Role>()
                    .Include(a => a.ACLs.Select(v => v.AclVerb))
                    .Include(u => u.Users)
                    .ToListAsync();
            }

            public IEnumerable<Role> GetAll()
            {
                return Context.Set<Role>()
                    .Include(a => a.ACLs.Select(v => v.AclVerb))
                    .Include(u => u.Users)
                    .ToList();
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