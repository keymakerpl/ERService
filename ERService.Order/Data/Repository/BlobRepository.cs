using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

namespace ERService.OrderModule.Data.Repository
{
    public class BlobRepository : GenericRepository<Blob, ERServiceDbContext>, IBlobRepository
    {
        public BlobRepository(ERServiceDbContext context) : base(context)
        {
        }
    }
}
