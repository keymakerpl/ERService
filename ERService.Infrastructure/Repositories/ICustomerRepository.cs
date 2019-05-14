using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERService.Business;

namespace ERService.Infrastructure.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
    }
}