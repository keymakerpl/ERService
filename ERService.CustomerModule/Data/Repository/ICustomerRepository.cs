using ERService.Business;
using ERService.Infrastructure.Repositories;

namespace ERService.CustomerModule.Repository
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
    }
}