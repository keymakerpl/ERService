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

        public override Task<IEnumerable<CustomItem>> GetAllAsync()
        {
            return base.GetAllAsync();
        }

        public override Task<CustomItem> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        [Obsolete]
        public async Task<List<CustomItem>> GetCustomItemsByHardwareTypeAsync(Guid typeId)
        {
            var result = await Context.Set<CustomItem>().AsNoTracking().Include(c => c.HardwareType).ToListAsync();

            return result.FindAll(e => e.HardwareType.Id == typeId);
        }

        public List<string> GetCustomItemsByHardwareType(Guid typeId)
        {
            var result = from h in Context.HardwareTypes
                         join c in Context.CustomItems on h.Id equals c.HardwareType.Id
                         where c.HardwareType.Id == typeId
                         select c.Key;

            return result.ToList();
        }
    }
}
