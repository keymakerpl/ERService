using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERService.Business;
using ERService.Infrastructure.Repositories;

namespace ERService.HardwareModule.Data.Repository
{
    public interface ICustomItemRepository : IGenericRepository<CustomItem>
    {
        Task<List<CustomItem>> GetCustomItemsByHardwareTypeAsync(Guid typeId);
        List<string> GetCustomItemsByHardwareType(Guid typeId);
    }
}