using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERService.HardwareModule.Data.Repository
{
    public class HwCustomItemRepository : GenericRepository<HwCustomItem, ERServiceDbContext>, IHwCustomItemRepository
    {
        protected HwCustomItemRepository(ERServiceDbContext context) : base(context)
        {
        }

        public override Task<IEnumerable<HwCustomItem>> GetAllAsync()
        {
            return base.GetAllAsync();
        }

        public override Task<HwCustomItem> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }
    }
}
