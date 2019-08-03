using ERService.Business;
using ERService.Infrastructure.Repositories;
using System.Data.Entity;

namespace ERService.OrderModule.Data.Repository
{
    public interface INumerationRepository : IGenericRepository<Numeration>
    {
        //TODO: Może to przenieść do generyka?
        void SetEntityStatus(object entity, EntityState entityState);
    }
}
