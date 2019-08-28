using System.Data.Entity;
using System.Threading.Tasks;
using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

namespace ERService.OrderModule.Data.Repository
{
    public class NumerationRepository : GenericRepository<Numeration, ERServiceDbContext>, INumerationRepository
    {
        public NumerationRepository(ERServiceDbContext context) : base(context)
        {

        }

        public void SetEntityStatus(object entity, EntityState entityState)
        {
            Context.Entry(entity).State = entityState;
        }
    }
}
