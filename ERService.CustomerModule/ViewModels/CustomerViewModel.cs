﻿using System;
using System.Threading.Tasks;
using ERService.Business;
using ERService.CustomerModule.Repository;
using ERService.CustomerModule.Wrapper;
using ERService.Infrastructure.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace ERService.CustomerModule.ViewModels
{
    //TODO: Refactor Interface
    public class CustomerViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        private CustomerWrapper _customer;
        private ICustomerRepository _repository;
        private IRegionManager _regionManager;
        private IRegionNavigationService _navigationService;        

        public CustomerWrapper Customer { get => _customer; set { _customer = value; RaisePropertyChanged(); } }        

        public CustomerViewModel(ICustomerRepository customerRepository, IRegionManager regionManager,
            IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _repository = customerRepository;
            _regionManager = regionManager;
        }

        //TODO: Refactor?
        public override async Task LoadAsync(Guid id)
        {
            var customer = id != Guid.Empty ? await _repository.GetByIdAsync(id) : GetNewDetail();

            //ustaw Id dla detailviewmodel, taki sam jak pobranego modelu z repo
            ID = id;

            InitializeCustomer(customer);
        }

        //TODO: Refactor to Generic and base class?
        private Customer GetNewDetail()
        {
            var Customer = new Customer();
            _repository.Add(Customer);

            return Customer;
        }

        private void InitializeCustomer(Customer customer)
        {
            //Opakowanie modelu detala w ModelWrapper aby korzystał z walidacji propertisów
            Customer = new CustomerWrapper(customer);

            //Po załadowaniu detala i każdej zmianie propertisa sprawdzamy CanExecute Sejwa
            Customer.PropertyChanged += ((sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _repository.HasChanges();
                    ((DelegateCommand)CancelEditDetailCommand).RaiseCanExecuteChanged();
                }

                //sprawdzamy czy zmieniony propert w modelu ma błędy i ustawiamy SaveButton
                if (args.PropertyName == nameof(Customer.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

                if (args.PropertyName == nameof(Customer.FirstName) || args.PropertyName == nameof(Customer.LastName))
                {
                    SetTitle();
                }
            });
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if(ID == Guid.Empty)
            {
                Customer.FirstName = ""; // takie se, trzeba tacznąć propertisa aby zadziałała walidacja nowego detalu
                Customer.PhoneNumber = "";
            }
                

            SetTitle();
        }

        private void SetTitle()
        {
            Title = $"{Customer.FirstName} {Customer.LastName}";
        }

        protected override void OnCancelEditExecute()
        {
            _navigationService.Journal.GoBack();
        }

        protected override bool OnCancelEditCanExecute() => true;

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

        #region Navigation

        public bool KeepAlive { get { return true; } }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            var id = navigationContext.Parameters.GetValue<string>("ID");
            if (!String.IsNullOrWhiteSpace(id))
            {
                await LoadAsync(Guid.Parse(id));
            }
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            //TODO: Refactor to dialog service
            var result = true;
            if (HasChanges)
            {
                //result = MessageBox.Show("Continue?", "Continue?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            }

            continuationCallback(result);
        }        

        #endregion
    }
}
