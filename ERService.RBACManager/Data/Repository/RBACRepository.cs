using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System.Linq;
using System.Data.Entity;

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
    }
}
