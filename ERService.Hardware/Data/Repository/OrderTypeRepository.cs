using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

namespace ERService.HardwareModule.Data.Repository
{
    public class OrderTypeRepository : GenericRepository<OrderType, ERServiceDbContext>, IOrderTypeRepository
    {
        public OrderTypeRepository(ERServiceDbContext context) : base(context)
        {

        }
    }
}
