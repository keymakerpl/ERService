using System;
using System.Threading.Tasks;
using ERService.Business;
using ERService.Infrastructure.Repositories;

namespace ERService.HardwareModule.Data.Repository
{
    public interface IHardwareTypeRepository : IGenericRepository<HardwareType>
    {
        Task<Guid[]> GetHardwareIDsWith(Guid hardwareTypeID);
    }
}