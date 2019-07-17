using ERService.Business;
using ERService.Infrastructure.Repositories;
using System.Data.Entity;

namespace ERService.OrderModule.Data.Repository
{
    public interface INumerationRepository : IGenericRepository<Numeration>
    {
        void SetEntityStatus(object entity, EntityState entityState);
    }
}
