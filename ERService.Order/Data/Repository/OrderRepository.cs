using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace ERService.OrderModule.Data
{
    public class OrderRepository : GenericRepository<Order, ERServiceDbContext>, IOrderRepository
    {
        protected OrderRepository(ERServiceDbContext context) : base(context)
        {
        }

        public override async Task<Order> GetByIdAsync(Guid id)
        {
            return await Context.Set<Order>().Include(c => c.Customer).SingleAsync(o => o.Id == id);
        }
    }
}
