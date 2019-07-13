using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

namespace ERService.OrderModule.Repository
{
    public class OrderStatusRepository : GenericRepository<OrderStatus, ERServiceDbContext>, IOrderStatusRepository
    {
        public OrderStatusRepository(ERServiceDbContext context) : base(context)
        {

        }
    }
}
