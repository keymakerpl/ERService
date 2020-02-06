using System;
using ERService.Business;
using ERService.CustomerModule.Wrapper;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using Prism.Events;

namespace ERService.CustomerModule.ViewModels
{
    //TODO: refactor do bardziej abstrakcyjnej klasy
    public class CustomerSearchViewModel : SearchViewModelBase
    {
        public Customer Customer { get; }
        public CustomerAddress CustomerAddress { get; }

        public CustomerSearchViewModel(IEventAggregator eventAggregator): base(eventAggregator)
        {
            Customer =  new Customer();
            CustomerAddress = new CustomerAddress();
        }

        //TODO: przebudować tak aby budował zapytnie przez refleksje na propertisach w customerwrapper
        protected override void OnSearchExecute()
        {
            var query = new QueryBuilder<Customer>();

            if (!String.IsNullOrWhiteSpace(CustomerAddress.Street) || !String.IsNullOrWhiteSpace(CustomerAddress.HouseNumber) 
                || !String.IsNullOrWhiteSpace(CustomerAddress.City) || !String.IsNullOrWhiteSpace(CustomerAddress.Postcode))
            {
                query.LeftJoin(nameof(Business.CustomerAddress), $"{nameof(CustomerAddress)}.{nameof(CustomerAddress.CustomerId)}", $"{nameof(Customer)}.{nameof(Customer.Id)}");

                query.WhereContains(nameof(CustomerAddress.Street), CustomerAddress.Street);
                query.WhereContains(nameof(CustomerAddress.HouseNumber), CustomerAddress.HouseNumber);
                query.WhereContains(nameof(CustomerAddress.City), CustomerAddress.City);
                query.WhereContains(nameof(CustomerAddress.Postcode), CustomerAddress.Postcode ?? "");
            }

            if (!String.IsNullOrWhiteSpace(Customer.FirstName))
            {
                query.WhereContains(nameof(Customer.FirstName), Customer.FirstName);
            }

            if (!String.IsNullOrWhiteSpace(Customer.LastName))
            {
                query.WhereContains(nameof(Customer.LastName), Customer.LastName);
            }

            if (!String.IsNullOrWhiteSpace(Customer.CompanyName))
            {
                query.WhereContains(nameof(Customer.CompanyName), Customer.CompanyName);
            }

            if (!String.IsNullOrWhiteSpace(Customer.Email))
            {
                query.WhereContains(nameof(Customer.Email), Customer.Email);
            }

            if (!String.IsNullOrWhiteSpace(Customer.Email2))
            {
                query.WhereContains(nameof(Customer.Email2), Customer.Email2);
            }

            if (!String.IsNullOrWhiteSpace(Customer.NIP))
            {
                query.WhereContains(nameof(Customer.NIP), Customer.NIP);
            }

            if (!String.IsNullOrWhiteSpace(Customer.PhoneNumber))
            {
                query.WhereContains(nameof(Customer.PhoneNumber), Customer.PhoneNumber);
            }

            if (!String.IsNullOrWhiteSpace(Customer.PhoneNumber2))
            {
                query.WhereContains(nameof(Customer.PhoneNumber2), Customer.PhoneNumber2);
            }            

            EventAggregator.GetEvent<SearchQueryEvent<Customer>>().Publish(new SearchQueryEventArgs<Customer>() { QueryBuilder = query });
        }
    }
}
