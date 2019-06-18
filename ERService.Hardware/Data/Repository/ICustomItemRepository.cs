using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERService.Business;
using ERService.Infrastructure.Repositories;

namespace ERService.HardwareModule.Data.Repository
{
    public interface ICustomItemRepository : IGenericRepository<CustomItem>
    {
        Task<IEnumerable<CustomItem>> GetAllAsync();
        Task<CustomItem> GetByIdAsync(Guid id);
        Task<List<CustomItem>> GetCustomItemsByHardwareTypeAsync(Guid typeId);
        List<string> GetCustomItemsByHardwareType(Guid typeId);
    }
}