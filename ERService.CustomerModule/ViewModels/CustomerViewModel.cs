using System;
using System.Threading.Tasks;
using ERService.Business;
using ERService.CustomerModule.Repository;
using ERService.CustomerModule.Wrapper;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Linq;

namespace ERService.CustomerModule.ViewModels
{
    //TODO: Refactor Interface
    public class CustomerViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {                
        private ICustomerRepository _repository;
        private IRegionManager _regionManager;
        private IRegionNavigationService _navigationService;

        public DelegateCommand GoForwardCommand { get; private set; }
        public DelegateCommand OrdersCommand { get; private set; }        

        private CustomerWrapper _customer;
        public CustomerWrapper Customer { get => _customer; set { SetProperty(ref _customer, value); } }

        private CustomerAddress _customerAddress;
        public CustomerAddress CustomerAddress { get => _customerAddress ?? new CustomerAddress(); set { SetProperty(ref _customerAddress, value); } }

        private bool _wizardMode;
        public bool WizardMode { get => _wizardMode; set { SetProperty(ref _wizardMode, value); } }

        public CustomerViewModel(ICustomerRepository customerRepository, IRegionManager regionManager,
            IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _repository = customerRepository;
            _regionManager = regionManager;

            GoForwardCommand = new DelegateCommand(OnGoForwardExecute, OnGoForwardCanExecute);
            OrdersCommand = new DelegateCommand(OnOrdersCommandExecute, OnOrdersCommandCanExecute);
        }

        private bool OnOrdersCommandCanExecute()
        {
            return !WizardMode;
        }

        private void OnOrdersCommandExecute()
        {
            
        }

        //TODO: Refactor?
        public override async Task LoadAsync(Guid id)
        {
            var customer = id != Guid.Empty ? await _repository.GetByIdAsync(id) : GetNewDetail();

            //ustaw Id dla detailviewmodel, taki sam jak pobranego modelu z repo
            ID = id;

            InitializeCustomer(customer);
            InitializeAddress(customer.CustomerAddresses.FirstOrDefault());
        }

        private void InitializeAddress(CustomerAddress customerAddress)
        {
            CustomerAddress = customerAddress ?? new CustomerAddress();
        }

        //TODO: Refactor to Generic and base class?
        private Customer GetNewDetail()
        {
            var customer = new Customer();
            _repository.Add(customer);

            return customer;
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
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                    ((DelegateCommand)GoForwardCommand).RaiseCanExecuteChanged();
                }

                //sprawdzamy czy zmieniony propert w modelu ma błędy i ustawiamy SaveButton
                if (args.PropertyName == nameof(Customer.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                    ((DelegateCommand)GoForwardCommand).RaiseCanExecuteChanged();
                }

                if (args.PropertyName == nameof(Customer.FirstName) || args.PropertyName == nameof(Customer.LastName))
                {
                    SetTitle();
                }
            });
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)GoForwardCommand).RaiseCanExecuteChanged();

            if (ID == Guid.Empty)
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
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
        }

        protected override bool OnCancelEditCanExecute() => true;

        protected override bool OnSaveCanExecute()
        {
            return Customer != null && !Customer.HasErrors && HasChanges && !WizardMode;
        }

        protected override async void OnSaveExecute()
        {
            Customer.Model.CustomerAddresses.Clear();
            Customer.Model.CustomerAddresses.Add(CustomerAddress);

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
            if (WizardMode)
            {
                AllowLoadAsync = false;
            }
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            WizardMode = navigationContext.Parameters.GetValue<bool>("Wizard");

            var id = navigationContext.Parameters.GetValue<string>("ID");
            if (!String.IsNullOrWhiteSpace(id) && AllowLoadAsync)
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

        private bool OnGoForwardCanExecute()
        {
            return Customer != null && !Customer.HasErrors && HasChanges && WizardMode;
        }

        private void OnGoForwardExecute()
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("Wizard", true);
            parameters.Add("Customer", Customer.Model);

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.HardwareView, parameters);
        }

        #endregion
    }
}
