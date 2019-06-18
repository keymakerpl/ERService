using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public override Task<IEnumerable<Hardware>> GetAllAsync()
        {
            return base.GetAllAsync();
        }

        public override Task<Hardware> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }
    }
}
