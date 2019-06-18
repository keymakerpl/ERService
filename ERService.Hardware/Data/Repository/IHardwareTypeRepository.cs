using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERService.Business;
using ERService.Infrastructure.Repositories;

namespace ERService.HardwareModule.Data.Repository
{
    public interface IHardwareTypeRepository : IGenericRepository<HardwareType>
    {
        Task<IEnumerable<HardwareType>> GetAllAsync();
        Task<HardwareType> GetByIdAsync(Guid id);
    }
}