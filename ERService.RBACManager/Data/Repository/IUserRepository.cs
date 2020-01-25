using ERService.Business;
using ERService.Infrastructure.Repositories;
using System.Collections.Generic;

namespace ERService.RBAC.Data.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        IEnumerable<User> GetAll();
    }
}
