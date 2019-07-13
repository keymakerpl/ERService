using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

namespace ERService.OrderModule.Repository
{
    public class OrderTypeRepository : GenericRepository<OrderType, ERServiceDbContext>, IOrderTypeRepository
    {
        public OrderTypeRepository(ERServiceDbContext context) : base(context)
        {

        }
    }
}
