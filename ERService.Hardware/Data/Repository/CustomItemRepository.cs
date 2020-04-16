using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System.Data.Entity;
using System.Linq;

namespace ERService.HardwareModule.Data.Repository
{
    public class CustomItemRepository : GenericRepository<CustomItem, ERServiceDbContext>, ICustomItemRepository
    {
        public CustomItemRepository(ERServiceDbContext context) : base(context)
        {

        }

        [Obsolete]
        public async Task<List<CustomItem>> GetCustomItemsByHardwareTypeAsync(Guid typeId)
        {
            var result = await Context.Set<CustomItem>().AsNoTracking().Include(c => c.HardwareType).ToListAsync();

            return result.FindAll(e => e.HardwareType.Id == typeId);
        }

        public Guid[] GetHardwareIDsByCustomItemID(Guid customItemID)
        {
            var query = from c in Context.HardwareCustomItems
                        where c.CustomItemId == customItemID
                        select c.HardwareId;

            var result = query.ToArray();
            return result;
        }
    }
}
