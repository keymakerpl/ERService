using ERService.Business;
using ERService.Infrastructure.Wrapper;
using System;

namespace ERService.CustomerModule.Wrapper
{
    public interface ICustomerWrapper : IModelWrapper<Customer>
    {
        string CompanyName { get; set; }
        string Description { get; set; }
        string Email { get; set; }
        string Email2 { get; set; }
        string FirstName { get; set; }
        Guid Id { get; }
        string LastName { get; set; }
        string NIP { get; set; }
        string PhoneNumber { get; set; }
        string PhoneNumber2 { get; set; }
    }
}