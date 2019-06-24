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

        private string _firstName;
        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _firstName, value); }
        }

        private string _lastName;
        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _lastName, value); }
        }

        private string _customerName;
        public string CustomerName
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _customerName, value); }
        }

        private string _nip;
        public string NIP
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _nip, value); }
        }

        private string _email;
        public string Email
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _email, value); }
        }

        private string _email2;
        public string Email2
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _email2, value); }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _phoneNumber, value); }
        }

        private string _phoneNumber2;
        public string PhoneNumber2
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _phoneNumber2, value); }
        }

        private string _description;
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _description, value); }
        }
    }
}
