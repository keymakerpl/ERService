using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

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
