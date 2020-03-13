using ERService.Business;
using ERService.CustomerModule.Repository;
using ERService.CustomerModule.Wrapper;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.RBAC;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.CustomerModule.ViewModels
{
    public class CustomerViewModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private ICustomerWrapper _customer;
        private IRegionManager _regionManager;
        private ICustomerRepository _repository;
        private CustomerAddress _customerAddress;
        private readonly IRBACManager _rBACManager;
        private IRegionNavigationService _navigationService;

        public CustomerViewModel(ICustomerRepository customerRepository, IRegionManager regionManager,
            IEventAggregator eventAggregator, IMessageDialogService messageDialogService, IRBACManager rBACManager)
            : base(eventAggregator, messageDialogService)
        {
            _repository = customerRepository;
            _regionManager = regionManager;
            _rBACManager = rBACManager;

            OrdersCommand = new DelegateCommand(OnOrdersCommandExecute, OnOrdersCommandCanExecute);
        }

        public ICustomerWrapper Customer { get => _customer; set { SetProperty(ref _customer, value); } }
        public CustomerAddress CustomerAddress { get => _customerAddress ?? new CustomerAddress(); set { SetProperty(ref _customerAddress, value); } }
        public DelegateCommand OrdersCommand { get; private set; }

        public override async Task LoadAsync(Guid id)
        {
            var customer = id != Guid.Empty ? await _repository.GetByIdAsync(id) : GetNewDetail();

            ID = id;

            InitializeCustomer(customer);
            InitializeAddress(customer.CustomerAddresses.FirstOrDefault());
        }

        #region Navigation

        public override bool KeepAlive { get { return false; } }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            IsReadOnly = navigationContext.Parameters.GetValue<bool>("IsReadOnly");

            var id = navigationContext.Parameters.GetValue<Guid>("ID");
            if (id != null)
            {
                await LoadAsync(id);
            }

            if (!_rBACManager.LoggedUserHasPermission(AclVerbNames.CanEditCustomer))
                IsReadOnly = true;
        }

        #endregion Navigation
        
        private void AddAddress()
        {
            Customer.Model.CustomerAddresses.Clear();
            Customer.Model.CustomerAddresses.Add(CustomerAddress);
        }
        
        private Customer GetNewDetail()
        {
            var customer = new Customer();
            _repository.Add(customer);

            _logger.Debug("New customer object created.");

            return customer;
        }

        private void InitializeAddress(CustomerAddress customerAddress)
        {
            CustomerAddress = customerAddress ?? new CustomerAddress();
        }

        private void InitializeCustomer(Customer customer)
        {
            if (customer == null) return;

            //Opakowanie modelu detala w ModelWrapper aby korzystał z walidacji propertisów
            Customer = new CustomerWrapper(customer);

            //Po załadowaniu detala i każdej zmianie propertisa sprawdzamy CanExecute Sejwa
            Customer.PropertyChanged += ((sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _repository.HasChanges();
                    SaveCommand.RaiseCanExecuteChanged();
                }

                //sprawdzamy czy zmieniony propert w modelu ma błędy i ustawiamy SaveButton
                if (args.PropertyName == nameof(Customer.HasErrors))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }

                if (args.PropertyName == nameof(Customer.FirstName) || args.PropertyName == nameof(Customer.LastName))
                {
                    SetTitle();
                }
            });
            SaveCommand.RaiseCanExecuteChanged();

            if (Customer.Id == Guid.Empty)
            {
                Customer.LastName = ""; // takie se, trzeba tacznąć propertisa aby zadziałała walidacja nowego detalu
                Customer.PhoneNumber = "";
            }

            SetTitle();
        }

        protected override void OnCancelEditExecute()
        {
            _navigationService.Journal.GoBack();
        }

        protected override bool OnSaveCanExecute()
        {
            return base.OnSaveCanExecute() && Customer != null && !Customer.HasErrors;
        }

        protected override async void OnSaveExecute()
        {
            AddAddress();

            await SaveWithOptimisticConcurrencyAsync(_repository.SaveAsync, () =>
            {
                HasChanges = _repository.HasChanges(); // Po zapisie ustawiamy flagę na false jeśli nie ma zmian w repo
                ID = Customer.Id; //odśwież Id z wrappera

                //Powiadom agregator eventów, że zapisano
                RaiseDetailSavedEvent(Customer.Id, $"{Customer.FirstName} {Customer.LastName}");
                _navigationService.Journal.GoBack();
            });
        }

        private bool OnOrdersCommandCanExecute()
        {
            return true;
        }

        private void OnOrdersCommandExecute()
        {
        }

        private void SetTitle()
        {
            Title = $"{Customer.FirstName} {Customer.LastName}";
        }
    }
}