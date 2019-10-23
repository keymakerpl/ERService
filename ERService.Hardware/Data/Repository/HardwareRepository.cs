using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

namespace ERService.HardwareModule.Data.Repository
{
    public class HardwareRepository : GenericRepository<Hardware, ERServiceDbContext>, IHardwareRepository
    {
        public HardwareRepository(ERServiceDbContext context) : base(context)
        {
        }
    }
}
