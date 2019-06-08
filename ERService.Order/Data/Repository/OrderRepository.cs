using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System.Data.Entity;
using System;
using System.Threading.Tasks;

namespace ERService.OrderModule.Repository
{
    public class OrderRepository : GenericRepository<Order, ERServiceDbContext>, IOrderRepository
    {
        public OrderRepository(ERServiceDbContext context) : base(context)
        {
        }

        public override async Task<Order> GetByIdAsync(Guid id)
        {
            return await Context.Set<Order>().Include(c => c.Customer).SingleAsync(o => o.Id == id);
        }
    }
}
