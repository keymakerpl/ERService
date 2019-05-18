using System;
using System.Threading.Tasks;
using System.Windows;
using ERService.Business;
using ERService.CustomerModule.Repository;
using ERService.CustomerModule.Wrapper;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using Prism.Events;
using Prism.Regions;

namespace ERService.CustomerModule.ViewModels
{
    public class CustomerViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        private CustomerWrapper _customer;
        private ICustomerRepository _repository;
        private IRegionManager _regionManager;

        public CustomerWrapper Customer { get => _customer; set { _customer = value; RaisePropertyChanged(); } }

        public bool KeepAlive => false;

        public CustomerViewModel(ICustomerRepository customerRepository, IRegionManager regionManager,
            IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _repository = customerRepository;
            _regionManager = regionManager;
        }

        public override async Task LoadAsync(Guid id)
        {
            var customer = await _repository.GetByIdAsync(id);

            //ustaw Id dla detailviewmodel, taki sam jak pobranego modelu z repo
            ID = id;

            InitializeCustomer(customer);
        }

        private void InitializeCustomer(Customer customer)
        {
            Customer = new CustomerWrapper(customer);

            Customer.PropertyChanged += ((sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _repository.HasChanges();
                }
            });

            SetTitle();
        }

        private void SetTitle()
        {
            Title = $"{Customer.FirstName} {Customer.LastName}";
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters.GetValue<string>("ID");
            if (!String.IsNullOrWhiteSpace(id))
            {
                await LoadAsync(Guid.Parse(id));
            }
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            //TODO: refactor to dialog service
            var result = true;
            if (HasChanges)
            {
                result = MessageBox.Show("Continue?", "Continue?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            }

            continuationCallback(result);
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return Customer != null && !Customer.HasErrors && HasChanges;
        }

        protected override async void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_repository.SaveAsync, () =>
            {
                HasChanges = _repository.HasChanges(); // Po zapisie ustawiamy flagę na false jeśli nie ma zmian w repo
                ID = Customer.Id; //odśwież Id friend wrappera

                //Powiadom agregator eventów, że zapisano
                RaiseDetailSavedEvent(Customer.Id, $"{Customer.FirstName} {Customer.LastName}");
            });
        }

        
    }
}
