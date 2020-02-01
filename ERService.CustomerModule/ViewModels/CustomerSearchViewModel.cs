using System;
using ERService.Business;
using ERService.CustomerModule.Wrapper;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using Prism.Commands;
using Prism.Events;

namespace ERService.CustomerModule.ViewModels
{
    //TODO: refactor do bardziej abstrakcyjnej klasy
    public class CustomerSearchViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand SearchCommand { get; }
        public CustomerWrapper Customer { get; }
        public AddressWrapper CustomerAddress { get; }

        public CustomerSearchViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            SearchCommand = new DelegateCommand(OnSearchExecute);

            Customer = new CustomerWrapper(new Customer());
            CustomerAddress = new AddressWrapper(new CustomerAddress());
        }

        //TODO: przebudować tak aby budował zapytnie przez refleksje na propertisach w customerwrapper
        private void OnSearchExecute()
        {
            var query = new QueryBuilder<Customer>();            

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

            if (!String.IsNullOrWhiteSpace(CustomerAddress.Street) || !String.IsNullOrWhiteSpace(CustomerAddress.HouseNumber) || !String.IsNullOrWhiteSpace(CustomerAddress.City))
            {
                query.Join(nameof(Business.CustomerAddress), $"{nameof(CustomerAddress)}.{nameof(CustomerAddress.Model.CustomerId)}", $"{nameof(Customer)}.{nameof(Customer.Id)}");

                query.WhereContains(nameof(CustomerAddress.Street), CustomerAddress.Street);
                query.WhereContains(nameof(CustomerAddress.HouseNumber), CustomerAddress.HouseNumber);
                query.WhereContains(nameof(CustomerAddress.City), CustomerAddress.City);
            }

            _eventAggregator.GetEvent<SearchQueryEvent<Customer>>().Publish(new SearchQueryEventArgs<Customer>() { queryBuilder = query });
        }
    }
}
