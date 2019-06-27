using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.OrderModule.Repository;
using ERService.OrderModule.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Threading.Tasks;

namespace ERService.OrderModule.ViewModels
{
    public class OrderViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {        
        public bool WizardMode { get => _wizardMode; set { SetProperty(ref _wizardMode, value); } }

        public OrderWrapper Order { get => _order; set { SetProperty(ref _order, value); } }

        public Customer Customer { get => _customer; set { SetProperty(ref _customer, value); } }

        public Hardware Hardware { get => _hardware; set { SetProperty(ref _hardware, value); } }

        public DelegateCommand GoBackCommand { get; private set; }

        private bool _wizardMode;
        private OrderWrapper _order;
        private Customer _customer;
        private Hardware _hardware;
        private IOrderRepository _repository;
        private IRegionManager _regionManager;
        private IRegionNavigationService _navigationService;

        public OrderViewModel(IRegionManager regionManager, IOrderRepository repository, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _repository = repository;
            _regionManager = regionManager;

            GoBackCommand = new DelegateCommand(OnGoBackExecute);
        }

        #region Navigation

        private void OnGoBackExecute()
        {
            _navigationService.Journal.GoBack();
        }

        public bool KeepAlive => false;

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

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
            WizardMode = navigationContext.Parameters.GetValue<bool>("Wizard");

            if (WizardMode)
            {
                //TODO: REFACTOR, czy możemy użyć tutaj budowniczego np. OrderBuilder?
                var customer = navigationContext.Parameters.GetValue<Customer>("Customer");
                if (customer != null)
                    Customer = customer;

                var hardware = navigationContext.Parameters.GetValue<Hardware>("Hardware");
                if (customer != null)
                    Hardware = hardware;

                await LoadAsync(Guid.Empty);

                //Order.DateAdded = DateTime.Now;
                Order.Model.Hardwares.Clear();
                Order.Model.Hardwares.Add(Hardware);
                Order.Model.Customer = Customer;
            }
            else
            {
                //var id = navigationContext.Parameters.GetValue<string>("ID");
                //if (!String.IsNullOrWhiteSpace(id))
                //{
                //    await LoadAsync(Guid.Parse(id));
                //}
            }

        }

        #endregion

        #region Overrides

        //TODO: Refactor?
        public override async Task LoadAsync(Guid id)
        {
            var order = id != Guid.Empty ? await _repository.GetByIdAsync(id) : GetNewDetail();

            //ustaw Id dla detailviewmodel, taki sam jak pobranego modelu z repo
            ID = id;
            
            InitializeOrder(order);
        }

        //TODO: Refactor?
        private Order GetNewDetail()
        {
            var order = new Order();
            _repository.Add(order);

            return order;
        }

        private void InitializeOrder(Order order)
        {
            //Opakowanie modelu detala w ModelWrapper aby korzystał z walidacji propertisów
            Order = new OrderWrapper(order);

            //Po załadowaniu detala i każdej zmianie propertisa sprawdzamy CanExecute Sejwa
            Order.PropertyChanged += ((sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _repository.HasChanges();
                    ((DelegateCommand)CancelCommand).RaiseCanExecuteChanged();
                }

                //sprawdzamy czy zmieniony propert w modelu ma błędy i ustawiamy SaveButton
                if (args.PropertyName == nameof(Order.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

                if (args.PropertyName == nameof(Order.Number))
                {
                    SetTitle();
                }
            });
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if (ID == Guid.Empty)
            {
                Order.Number = "";
            }


            SetTitle();
        }

        private void SetTitle()
        {
            Title = $"{Order.Number}";
        }

        protected override bool OnCancelEditCanExecute()
        {
            return true;
        }

        protected override void OnCancelEditExecute()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
        }

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        protected async override void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_repository.SaveAsync, () =>
            {
                HasChanges = _repository.HasChanges(); // Po zapisie ustawiamy flagę na false jeśli nie ma zmian w repo
                ID = Customer.Id; //odśwież Id friend wrappera                

                //Powiadom agregator eventów, że zapisano
                RaiseDetailSavedEvent(Customer.Id, $"{Customer.FirstName} {Customer.LastName}");
                _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            });
        }

        #endregion
    }
}
