using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

namespace ERService.HardwareModule.Data.Repository
{
    public class HwCustomItemRepository : GenericRepository<HwCustomItem, ERServiceDbContext>, IHwCustomItemRepository
    {
        protected HwCustomItemRepository(ERServiceDbContext context) : base(context)
        {

        }
    }
}
