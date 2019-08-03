using ERService.Business;
using ERService.OrderModule.Repository;
using ERService.Infrastructure.Base;
using ERService.OrderModule.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class StatusConfigViewModel : DetailViewModelBase, INavigationAware
    {
        private IOrderStatusRepository _orderStatusRepository;
        private IOrderTypeRepository _orderTypeRepository;
        private OrderStatusWrapper _selectedOrderStatus;
        private OrderTypeWrapper _selectedOrderType;

        public StatusConfigViewModel(IEventAggregator eventAggregator, IOrderStatusRepository orderStatusRepository,
             IOrderTypeRepository orderTypeRepository) : base(eventAggregator)
        {
            Title = "Konfiguracja statusów";

            _orderStatusRepository = orderStatusRepository;
            _orderTypeRepository = orderTypeRepository;

            OrderTypes = new ObservableCollection<OrderTypeWrapper>();
            OrderStatuses = new ObservableCollection<OrderStatusWrapper>();

            AddOrderTypeCommand = new DelegateCommand(OnAddOrderTypeExecute);
            AddOrderStatusCommand = new DelegateCommand(OnAddOrderStatusExecute);

            RemoveOrderTypeCommand = new DelegateCommand(OnRemoveOrderTypeExecute, OnRemoveOrderTypeCanExecute);
            RemoveOrderStatusCommand = new DelegateCommand(OnRemoveOrderStatusExecute, OnRemoveOrderStatusCanExecute);
        }

        public DelegateCommand AddOrderStatusCommand { get; private set; }
        public DelegateCommand AddOrderTypeCommand { get; private set; }
        public ObservableCollection<OrderStatusWrapper> OrderStatuses { get; set; }
        public ObservableCollection<OrderTypeWrapper> OrderTypes { get; set; }
        public DelegateCommand RemoveOrderStatusCommand { get; private set; }
        public DelegateCommand RemoveOrderTypeCommand { get; private set; }

        public OrderStatusWrapper SelectedOrderStatus
        {
            get { return _selectedOrderStatus; }
            set { SetProperty(ref _selectedOrderStatus, value); }
        }

        public OrderTypeWrapper SelectedOrderType
        {
            get { return _selectedOrderType; }
            set { SetProperty(ref _selectedOrderType, value); }
        }

        public async override Task LoadAsync(Guid id)
        {
            await LoadStatuses();
            await LoadTypes();
        }

        protected override void OnCancelEditExecute()
        {
        }

        protected override bool OnSaveCanExecute()
        {
            return (_orderStatusRepository.HasChanges() || _orderTypeRepository.HasChanges());
        }

        protected async override void OnSaveExecute()
        {
            await _orderStatusRepository.SaveAsync();
            await _orderTypeRepository.SaveAsync();

            HasChanges = _orderStatusRepository.HasChanges() || _orderTypeRepository.HasChanges();
        }

        private async Task LoadStatuses()
        {
            foreach (var status in OrderStatuses)
            {
                status.PropertyChanged -= WrappedStatus_PropertyChanged;
            }

            OrderStatuses.Clear();
            var statuses = await _orderStatusRepository.GetAllAsync();
            foreach (var status in statuses)
            {
                var wrappedStatus = new OrderStatusWrapper(status);
                wrappedStatus.PropertyChanged += WrappedStatus_PropertyChanged;
                OrderStatuses.Add(wrappedStatus);
            }
        }

        private async Task LoadTypes()
        {
            foreach (var type in OrderTypes)
            {
                type.PropertyChanged -= WrappedType_PropertyChanged;
            }

            OrderTypes.Clear();
            var types = await _orderTypeRepository.GetAllAsync();
            foreach (var type in types)
            {
                var wrappedType = new OrderTypeWrapper(type);
                wrappedType.PropertyChanged += WrappedType_PropertyChanged;
                OrderTypes.Add(wrappedType);
            }
        }

        private void OnAddOrderStatusExecute()
        {
            var wrappedOrderStatus = new OrderStatusWrapper(new OrderStatus());
            _orderStatusRepository.Add(wrappedOrderStatus.Model);
            wrappedOrderStatus.PropertyChanged += WrappedStatus_PropertyChanged;
            wrappedOrderStatus.Name = "";

            OrderStatuses.Add(wrappedOrderStatus);
        }

        private void OnAddOrderTypeExecute()
        {
            var wrappedOrderType = new OrderTypeWrapper(new OrderType());
            _orderTypeRepository.Add(wrappedOrderType.Model);
            wrappedOrderType.PropertyChanged += WrappedType_PropertyChanged;
            wrappedOrderType.Name = "";

            OrderTypes.Add(wrappedOrderType);
        }

        //TODO: Czy można zrobić tak aby usuwany element był generykiem aby można było przenieść ADD/REMOVE niżej?
        private bool OnRemoveOrderStatusCanExecute()
        {
            return SelectedOrderStatus != null;
        }

        private void OnRemoveOrderStatusExecute()
        {
            _orderStatusRepository.Remove(SelectedOrderStatus.Model);
            OrderStatuses.Remove(SelectedOrderStatus);
        }

        private bool OnRemoveOrderTypeCanExecute()
        {
            return SelectedOrderType != null;
        }

        private void OnRemoveOrderTypeExecute()
        {
            _orderTypeRepository.Remove(SelectedOrderType.Model);
            OrderTypes.Remove(SelectedOrderType);
        }

        private void WrappedStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges) //odśwerzamy z repo czy już zaszły jakieś zmiany, nie odpalamy jeśli już jest True
            {
                HasChanges = _orderTypeRepository.HasChanges();
            }

            if (e.PropertyName == nameof(OrderTypeWrapper.HasErrors)) //sprawdzamy czy możemy sejwować
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        //TODO: Zastanówmy się czy tego handlera nie można przenieść gdzieś niżej, czy może być generykiem?
        private void WrappedType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges) //odśwerzamy z repo czy już zaszły jakieś zmiany, nie odpalamy jeśli już jest True
            {
                HasChanges = _orderTypeRepository.HasChanges();
            }

            if (e.PropertyName == nameof(OrderTypeWrapper.HasErrors)) //sprawdzamy czy możemy sejwować
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
        #region Navigation

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync(Guid.Empty);
        }

        #endregion Navigation
    }
}