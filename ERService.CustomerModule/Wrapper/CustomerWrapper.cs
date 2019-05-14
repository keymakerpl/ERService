using ERService.Business;
using ERService.Infrastructure.Wrapper;
using System;

namespace ERService.CustomerModule.Wrapper
{
    public class CustomerWrapper : ModelWrapper<Customer>
    {
        public CustomerWrapper(Customer model) : base(model)
        {
        }

        public Guid Id { get { return Model.Id; } }

        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
