using System;
using System.Linq;
using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using Prism.Events;

namespace ERService.CustomerModule.ViewModels
{
    public class CustomerSearchViewModel : SearchViewModelBase
    {
        public Customer Customer { get; }
        public CustomerAddress CustomerAddress { get; }

        public CustomerSearchViewModel(IEventAggregator eventAggregator): base(eventAggregator)
        {
            Customer =  new Customer();
            CustomerAddress = new CustomerAddress();
        }
        
        protected override void OnSearchExecute()
        {
            var predicate = PredicateBuilder.True<Customer>();

            if (!String.IsNullOrWhiteSpace(Customer.FirstName))
            {
                predicate = predicate.And(c => c.FirstName.Contains(Customer.FirstName));
            }

            if (!String.IsNullOrWhiteSpace(Customer.LastName))
            {
                predicate = predicate.And(c => c.LastName.Contains(Customer.LastName));
            }

            if (!String.IsNullOrWhiteSpace(Customer.CompanyName))
            {
                predicate = predicate.And(c => c.CompanyName.Contains(Customer.CompanyName));
            }

            if (!String.IsNullOrWhiteSpace(Customer.Email))
            {
                predicate = predicate.And(c => c.Email.Contains(Customer.Email));
            }

            if (!String.IsNullOrWhiteSpace(Customer.Email2))
            {
                predicate = predicate.And(c => c.Email2.Contains(Customer.Email2));
            }

            if (!String.IsNullOrWhiteSpace(Customer.NIP))
            {
                var nip = Customer.NIP.Replace("-", string.Empty).Replace(" ", string.Empty).Trim();
                predicate = predicate.And(c => c.NIP.Replace("-", string.Empty).Replace(" ", string.Empty).Trim().Contains(nip));
            }

            if (!String.IsNullOrWhiteSpace(Customer.PhoneNumber))
            {
                var number = Customer.PhoneNumber.Replace(" ", string.Empty).Replace("-", string.Empty).Trim();
                predicate = predicate.And(c => c.PhoneNumber.Replace(" ", string.Empty).Replace("-", string.Empty).Trim().Contains(number));
            }

            if (!String.IsNullOrWhiteSpace(Customer.PhoneNumber2))
            {
                var number = Customer.PhoneNumber2.Replace(" ", string.Empty).Replace("-", string.Empty).Trim();
                predicate = predicate.And(c => c.PhoneNumber2.Replace(" ", string.Empty).Replace("-", string.Empty).Trim().Contains(number));
            }

            if (!String.IsNullOrWhiteSpace(CustomerAddress.Street))
            {
                predicate = predicate.And(c => c.CustomerAddresses.Any(a => a.Street.Contains(CustomerAddress.Street)));
            }

            if (!String.IsNullOrWhiteSpace(CustomerAddress.HouseNumber))
            {
                predicate = predicate.And(c => c.CustomerAddresses.Any(a => a.HouseNumber.Contains(CustomerAddress.HouseNumber)));
            }

            if (!String.IsNullOrWhiteSpace(CustomerAddress.City))
            {
                predicate = predicate.And(c => c.CustomerAddresses.Any(a => a.City.Contains(CustomerAddress.City)));
            }

            if (!String.IsNullOrWhiteSpace(CustomerAddress.Postcode))
            {
                predicate = predicate.And(c => c.CustomerAddresses.Any(a => a.Postcode.Contains(CustomerAddress.Postcode)));
            }

            EventAggregator.GetEvent<SearchEvent<Customer>>().Publish(new SearchEventArgs<Customer>() { Predicate = predicate});
        }
    }
}
