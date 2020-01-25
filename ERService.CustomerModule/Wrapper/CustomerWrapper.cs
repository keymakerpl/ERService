using ERService.Business;
using ERService.Infrastructure.Attributes;
using ERService.Infrastructure.Wrapper;
using System;

namespace ERService.CustomerModule.Wrapper
{
    public class CustomerWrapper : ModelWrapper<Customer>, ICustomerWrapper
    {
        public CustomerWrapper(Customer model) : base(model)
        {

        }

        public Guid Id { get { return Model.Id; } }

        private string _firstName;

        [Interpreter(Name = "Imię klienta", Pattern = "[%c_FirstName%]")]
        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _firstName, value); }
        }

        private string _lastName;

        [Interpreter(Name = "Nazwisko klienta", Pattern = "[%c_LastName%]")]
        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _lastName, value); }
        }

        private string _companyName;

        [Interpreter(Name = "Nazwa firmy", Pattern = "[%c_CompanyName%]")]
        public string CompanyName
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _companyName, value); }
        }

        private string _nip;

        [Interpreter(Name = "NIP", Pattern = "[%c_nip%]")]
        public string NIP
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _nip, value); }
        }

        private string _email;

        [Interpreter(Name = "Email", Pattern = "[%c_email%]")]
        public string Email
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _email, value); }
        }

        private string _email2;

        [Interpreter(Name = "Email 2", Pattern = "[%c_email2%]")]
        public string Email2
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _email2, value); }
        }

        private string _phoneNumber;

        [Interpreter(Name = "Numer telefonu", Pattern = "[%c_phoneNumber%]")]
        public string PhoneNumber
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _phoneNumber, value); }
        }

        private string _phoneNumber2;

        [Interpreter(Name = "Numer telefonu 2", Pattern = "[%c_phonenumber2%]")]
        public string PhoneNumber2
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _phoneNumber2, value); }
        }

        private string _description;

        [Interpreter(Name = "Opis", Pattern = "[%c_description%]")]
        public string Description
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _description, value); }
        }
    }
}
