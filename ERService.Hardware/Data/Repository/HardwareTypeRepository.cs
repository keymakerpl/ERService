using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

namespace ERService.HardwareModule.Data.Repository
{
    public class HardwareTypeRepository : GenericRepository<HardwareType, ERServiceDbContext>, IHardwareTypeRepository
    {
        public HardwareTypeRepository(ERServiceDbContext context) : base(context)
        {

        }  
    }
}
