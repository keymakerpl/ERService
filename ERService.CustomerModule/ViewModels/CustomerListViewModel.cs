using ERService.Infrastructure.Base;
using ERService.Infrastructure.Repositories;
using ERService.Business;
using Prism.Events;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ERService.CustomerModule.ViewModels
{
    public class CustomerListViewModel : DetailViewModelBase
    {
        private ICustomerRepository _repository;

        public CustomerListViewModel(ICustomerRepository repository, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _repository = repository;

            Customers = new ObservableCollection<Business.Customer>();
            LoadCustomersAsync();
        }

        private async void LoadCustomersAsync()
        {
            var customers = await _repository.GetAllAsync();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }

        public ObservableCollection<Business.Customer> Customers { get; }

        public override Task LoadAsync(Guid id)
        {
            return new Task(null);
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }
    }
}
