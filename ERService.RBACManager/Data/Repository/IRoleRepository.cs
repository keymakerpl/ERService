using ERService.Business;
using ERService.Infrastructure.Repositories;
using System.Data.Entity;

namespace ERService.RBAC.Data.Repository
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        void SetEntityStatus(object entity, EntityState entityState);
    }
}
