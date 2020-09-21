using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System;
using System.Threading.Tasks;

namespace ERService.HardwareModule.Data.Repository
{
    public class HardwareTypeRepository : GenericRepository<HardwareType, ERServiceDbContext>, IHardwareTypeRepository
    {
        public HardwareTypeRepository(ERServiceDbContext context) : base(context)
        {

        }
        
        /// <summary>
        /// Get Hardware IDs with provided Hardware Type ID
        /// </summary>
        /// <param name="hardwareTypeID">Provided Hardware Type ID</param>
        /// <returns>Array of Hardware IDs</returns>
        public async Task<Guid[]> GetHardwareIDsWith(Guid hardwareTypeID)
        {
            var query = SQLQueryBuilder.CreateQuery(nameof(Hardware))
                                .Select($"{nameof(Hardware)}.{nameof(Hardware.Id)}")
                                .Join(nameof(HardwareType), $"{nameof(Hardware)}.{nameof(Hardware.HardwareTypeID)}"
                                , $"{nameof(HardwareType)}.{nameof(HardwareType.Id)}");
            
            var sqlQuery = query.Compile();
            var ids = await GetAsync<Guid>(sqlQuery.Query, sqlQuery.Parameters);

            return ids.ToArray();
        }
    }
}
