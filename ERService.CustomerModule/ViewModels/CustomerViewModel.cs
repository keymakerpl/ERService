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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ERService.CustomerModule.ViewModels
{
    //TODO: Refactor Interface
    public class CustomerViewModel : DetailViewModelBase
    {
        private ICustomerWrapper _customer;
        private CustomerAddress _customerAddress;
        private IRegionNavigationService _navigationService;
        private IRegionManager _regionManager;
        private readonly IRBACManager _rBACManager;

        public ObservableCollection<Customer> Customers { get; private set; }

        private ICustomerRepository _repository;
        private bool _wizardMode;
        private Customer _selectedCustomer;

        public CustomerViewModel(ICustomerRepository customerRepository, IRegionManager regionManager,
            IEventAggregator eventAggregator, IMessageDialogService messageDialogService, IRBACManager rBACManager) 
            : base(eventAggregator, messageDialogService)
        {
            _repository = customerRepository;
            _regionManager = regionManager;
            _rBACManager = rBACManager;

            Customers = new ObservableCollection<Customer>();

            GoForwardCommand = new DelegateCommand(OnGoForwardExecute, OnGoForwardCanExecute);
            OrdersCommand = new DelegateCommand(OnOrdersCommandExecute, OnOrdersCommandCanExecute);
            DropDownCloseCommand = new DelegateCommand<object>(OnDropDownClosed);
        }

        private void OnDropDownClosed(object arg)
        {
            //if (WizardMode)
            //{
            //    var box = arg as AutoCompleteBox;

            //    if (box?.SelectedItem != null && box.IsMouseOver)
            //    {
            //        InitializeCustomer(SelectedCustomer);
            //        IsReadOnly = true;
            //    }
            //}
        }

        public ICustomerWrapper Customer { get => _customer; set { SetProperty(ref _customer, value); } }
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                SetProperty(ref _selectedCustomer, value);                
            }
        }
        public CustomerAddress CustomerAddress { get => _customerAddress ?? new CustomerAddress(); set { SetProperty(ref _customerAddress, value); } }
        public DelegateCommand GoForwardCommand { get; private set; }
        public DelegateCommand OrdersCommand { get; private set; }
        public DelegateCommand<object> DropDownCloseCommand { get; private set; }

        public bool WizardMode { get => _wizardMode; set { SetProperty(ref _wizardMode, value); } }

        //TODO: Refactor?
        public override async Task LoadAsync(Guid id)
        {
            var customer = id != Guid.Empty ? await _repository.GetByIdAsync(id) : GetNewDetail();

            //ustaw Id dla detailviewmodel, taki sam jak pobranego modelu z repo
            ID = id;

            InitializeCustomer(customer);            
            InitializeAddress(customer.CustomerAddresses.FirstOrDefault());
        }

        private async void InitializeCustomers()
        {
            var customers = await _repository.GetAllAsync();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }

        #region Navigation

        public override bool KeepAlive { get { return true; } }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (WizardMode) AllowLoadAsync = false;                
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            WizardMode = navigationContext.Parameters.GetValue<bool>("Wizard"); //TODO: Można by się tu zastanowić czy nie skorzystać z State lub Strategy?

            if(WizardMode)
                InitializeCustomers();

            var id = navigationContext.Parameters.GetValue<string>("ID");
            if (!String.IsNullOrWhiteSpace(id) && AllowLoadAsync)
            {
                await LoadAsync(Guid.Parse(id));
            }

            if (!_rBACManager.LoggedUserHasPermission(AclVerbNames.CanEditCustomer))
                IsReadOnly = true;
        }
        #endregion Navigation

        protected override void OnCancelEditExecute()
        {
            _navigationService.Journal.GoBack();
        }

        protected override bool OnSaveCanExecute()
        {
            return base.OnSaveCanExecute() && Customer != null && !Customer.HasErrors && !WizardMode;
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
            });
        }

        //TODO: Refactor to Generic and base class?
        private Customer GetNewDetail()
        {
            var customer = new Customer();
            _repository.Add(customer);

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

            if (Customer.Id == Guid.Empty)
            {
                Customer.LastName = ""; // takie se, trzeba tacznąć propertisa aby zadziałała walidacja nowego detalu
                Customer.PhoneNumber = "";
            }

            SetTitle();
        }

        private bool OnGoForwardCanExecute()
        {
            return base.HasChanges && Customer != null && !Customer.HasErrors && WizardMode;
        }

        private void OnGoForwardExecute()
        {
            AddAddress();

            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("Wizard", true);
            parameters.Add("Customer", Customer.Model);

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.HardwareView, parameters);
        }

        private void AddAddress()
        {
            Customer.Model.CustomerAddresses.Clear();
            Customer.Model.CustomerAddresses.Add(CustomerAddress);
        }

        private bool OnOrdersCommandCanExecute()
        {
            return !WizardMode;
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