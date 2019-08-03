using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ERService.CustomerModule.Repository
{
    public class CustomerRepository : GenericRepository<Customer, ERServiceDbContext>, ICustomerRepository
    {
        public CustomerRepository(ERServiceDbContext context) : base(context)
        {

        }

        public override async Task<Customer> GetByIdAsync(Guid id)
        {
            return await Context.Set<Customer>().Include(a => a.CustomerAddresses).SingleAsync(c => c.Id == id);
        }   
    }
}
