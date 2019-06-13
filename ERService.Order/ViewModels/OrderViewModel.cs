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
        private IOrderRepository _repository;
        private IRegionManager _regionManager;
        private IRegionNavigationService _navigationService;
        public DelegateCommand GoBackCommand { get; private set; }
        
        private bool _wizardMode;
        public bool WizardMode { get => _wizardMode; set { SetProperty(ref _wizardMode, value); } }

        private OrderWrapper _order;
        public OrderWrapper Order { get => _order; set { SetProperty(ref _order, value); } }

        private Customer _customer;
        public Customer Customer { get => _customer; set { SetProperty(ref _customer, value); } }

        private Hardware _hardware;
        private Hardware Hardware { get => _hardware; set { SetProperty(ref _hardware, value); } }

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

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
            WizardMode = navigationContext.Parameters.GetValue<bool>("Wizard");

            var customer = navigationContext.Parameters.GetValue<Customer>("Customer");
            if (customer != null)
                Customer = customer;

            //var id = navigationContext.Parameters.GetValue<string>("ID");
            //if (!String.IsNullOrWhiteSpace(id))
            //{
            //    await LoadAsync(Guid.Parse(id));
            //}
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

        protected override void OnSaveExecute()
        {
            
        }

        #endregion
    }
}
