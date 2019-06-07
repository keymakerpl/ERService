using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.OrderModule.Data;
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

        public DelegateCommand GoBackCommand { get; private set; }

        private OrderWrapper _order;
        public OrderWrapper Order { get => _order; set { _order = value; RaisePropertyChanged(); } }

        private Customer _customer;
        public Customer Customer { get => _customer; set { _customer = value; RaisePropertyChanged(); } }

        public OrderViewModel(IOrderRepository repository, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _repository = repository;
            GoBackCommand = new DelegateCommand(OnGoBackExecute);
        }

        private void OnGoBackExecute()
        {
            
        }

        #region Navigation

        public bool KeepAlive => false;

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            
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
            InitializeCustomer(order);
        }

        private void InitializeCustomer(Order order)
        {
            Customer = order.Customer;
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
                    ((DelegateCommand)CancelEditDetailCommand).RaiseCanExecuteChanged();
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
            throw new NotImplementedException();
        }

        protected override void OnCancelEditExecute()
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

        #endregion
    }
}
