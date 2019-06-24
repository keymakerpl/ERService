using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERService.Business;

namespace ERService.HardwareModule.Data.Repository
{
    public interface IHwCustomItemRepository
    {
        Task<IEnumerable<HwCustomItem>> GetAllAsync();
        Task<HwCustomItem> GetByIdAsync(Guid id);
    }
}