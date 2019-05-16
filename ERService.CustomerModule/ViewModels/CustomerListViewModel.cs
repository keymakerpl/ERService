using ERService.Infrastructure.Base;
using ERService.Business;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using ERService.CustomerModule.Repository;
using Prism.Regions;
using ERService.CustomerModule.Views;
using ERService.Infrastructure.Constants;

namespace ERService.CustomerModule.ViewModels
{
    public class CustomerListViewModel : DetailViewModelBase
    {
        private ICustomerRepository _repository;
        private IRegionManager _regionManager;

        public CustomerListViewModel(IRegionManager regionManager, ICustomerRepository repository, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _repository = repository;
            _regionManager = regionManager;

            Customers = new ObservableCollection<Customer>();
            LoadCustomersAsync();
        }

        public void OnMouseDoubleClickExecute()
        {
            if (SelectedCustomer != null)
            {
                var parameters = new NavigationParameters();
                parameters.Add("ID", SelectedCustomer.Id);

                var uri = new Uri(typeof(CustomerView).FullName + parameters, UriKind.Relative);
                _regionManager.RequestNavigate(RegionNames.ContentRegion, uri);
            }
        }   

        private async void LoadCustomersAsync()
        {
            var customers = await _repository.GetAllAsync();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }

        public Customer SelectedCustomer { get; set; }

        public ObservableCollection<Customer> Customers { get; }

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
