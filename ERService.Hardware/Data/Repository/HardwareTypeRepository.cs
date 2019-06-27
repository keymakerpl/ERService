using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;

namespace ERService.HardwareModule.Data.Repository
{
    public class HardwareTypeRepository : GenericRepository<HardwareType, ERServiceDbContext>, IHardwareTypeRepository
    {
        public HardwareTypeRepository(ERServiceDbContext context) : base(context)
        {

        }  
    }
}
