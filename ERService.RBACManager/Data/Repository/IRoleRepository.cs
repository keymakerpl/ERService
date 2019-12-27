using ERService.Business;
using ERService.Infrastructure.Repositories;
using System.Collections.Generic;

namespace ERService.RBAC.Data.Repository
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        IEnumerable<Role> GetAll();
    }
}
