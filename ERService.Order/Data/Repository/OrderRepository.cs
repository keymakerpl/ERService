using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System.Data.Entity;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace ERService.OrderModule.Repository
{
    public class OrderRepository : GenericRepository<Order, ERServiceDbContext>, IOrderRepository
    {
        public OrderRepository(ERServiceDbContext context) : base(context)
        {
        }

        public override async Task<Order> GetByIdAsync(Guid id)
        {
            return await Context.Set<Order>()
                                .Include(c => c.Customer)
                                .Include(c => c.Customer.CustomerAddresses)
                                .Include(h => h.Hardwares)
                                .Include(a => a.Attachments)
                                .Include(os => os.OrderStatus)
                                .Include(ot => ot.OrderType)
                                .SingleAsync(o => o.Id == id);
        }
    }
}
