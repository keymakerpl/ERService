using ERService.Business;
using ERService.Infrastructure.Attributes;
using ERService.Infrastructure.Wrapper;

namespace ERService.CustomerModule.Wrapper
{
    public class AddressWrapper : ModelWrapper<CustomerAddress>
    {
        private string _street;
        private string _houseNumber;
        private string _city;
        private string _postcode;

        public AddressWrapper(CustomerAddress model) : base(model)
        {
        }

        [Interpreter(Name = "Ulica", Pattern = "[%c_street%]")]
        public string Street
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _street, value); }
        }

        [Interpreter(Name = "Numer domu", Pattern = "[%c_houseNumber%]")]
        public string HouseNumber
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _houseNumber, value); }
        }

        [Interpreter(Name = "Miasto", Pattern = "[%c_city%]")]
        public string City
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _city, value); }
        }

        [Interpreter(Name = "Kod pocztowy", Pattern = "[%c_postCode%]")]
        public string Postcode
        {
            get { return GetValue<string>(); }
            set { SetProperty(ref _postcode, value); }
        }
    }
}
