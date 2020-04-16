using ERService.Business;
using ERService.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERService.HardwareModule.Data.Repository
{
    public interface ICustomItemRepository : IGenericRepository<CustomItem>
    {
        Task<List<CustomItem>> GetCustomItemsByHardwareTypeAsync(Guid typeId);
        Guid[] GetHardwareIDsByCustomItemID(Guid id);
    }
}