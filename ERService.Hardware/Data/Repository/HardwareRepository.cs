using System;
using System.Threading.Tasks;
using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System.Data.Entity;

namespace ERService.HardwareModule.Data.Repository
{
    public class HardwareRepository : GenericRepository<Hardware, ERServiceDbContext>, IHardwareRepository
    {
        public HardwareRepository(ERServiceDbContext context) : base(context)
        {
        }

        public override async Task<Hardware> GetByIdAsync(Guid id)
        {
            return await Context.Set<Hardware>().Include(t => t.HardwareType).SingleAsync(h => h.Id == id);
        }
    }
}
