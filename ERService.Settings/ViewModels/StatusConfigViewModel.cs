using ERService.Business;
using ERService.HardwareModule.Data.Repository;
using ERService.Infrastructure.Base;
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
        private OrderStatus _selectedOrderStatus;

        private OrderType _selectedOrderType;

        private IOrderStatusRepository _orderStatusRepository;
        private IOrderTypeRepository _orderTypeRepository;
        private ObservableCollection<OrderStatus> OrderStatuses;
        private ObservableCollection<OrderType> OrderTypes;

        public StatusConfigViewModel(IEventAggregator eventAggregator, IOrderStatusRepository orderStatusRepository,
             IOrderTypeRepository orderTypeRepository) : base(eventAggregator)
        {
            Title = "Konfiguracja statusów";

            _orderStatusRepository = orderStatusRepository;
            _orderTypeRepository = orderTypeRepository;

            OrderTypes = new ObservableCollection<OrderType>();
            OrderStatuses = new ObservableCollection<OrderStatus>();

            AddOrderTypeCommand = new DelegateCommand(OnAddOrderTypeExecute);
            AddOrderStatusCommand = new DelegateCommand(OnAddOrderStatusExecute);

            RemoveOrderTypeCommand = new DelegateCommand(OnRemoveOrderTypeExecute, OnRemoveOrderTypeCanExecute);
            RemoveOrderStatusCommand = new DelegateCommand(OnRemoveOrderStatusExecute, OnRemoveOrderStatusCanExecute);
        }

        public DelegateCommand AddOrderStatusCommand { get; private set; }

        public DelegateCommand AddOrderTypeCommand { get; private set; }

        public DelegateCommand RemoveOrderStatusCommand { get; private set; }

        public DelegateCommand RemoveOrderTypeCommand { get; private set; }

        public OrderStatus SelectedOrderStatus
        {
            get { return _selectedOrderStatus; }
            set { SetProperty(ref _selectedOrderStatus, value); }
        }

        public OrderType SelectedOrderType
        {
            get { return _selectedOrderType; }
            set { SetProperty(ref _selectedOrderType, value); }
        }

        public async override Task LoadAsync(Guid id)
        {
            await LoadStatuses();
            await LoadTypes();
        }

        private async Task LoadTypes()
        {
            OrderTypes.Clear();
            var types = await _orderTypeRepository.GetAllAsync();
            foreach (var type in types)
            {
                OrderTypes.Add(type);
            }
        }

        private async Task LoadStatuses()
        {
            OrderStatuses.Clear();
            var statuses = await _orderStatusRepository.GetAllAsync();
            foreach (var status in statuses)
            {
                OrderStatuses.Add(status);
            }
        }

        protected override bool OnCancelEditCanExecute()
        {
            return false;
        }

        protected override void OnCancelEditExecute()
        {
        }

        protected override bool OnSaveCanExecute()
        {
            return false;
        }

        protected override void OnSaveExecute()
        {
        }

        private void OnAddOrderStatusExecute()
        {
            
        }

        private void OnAddOrderTypeExecute()
        {
        }

        private bool OnRemoveOrderStatusCanExecute()
        {
            return SelectedOrderStatus != null;
        }

        private void OnRemoveOrderStatusExecute()
        {
        }

        private bool OnRemoveOrderTypeCanExecute()
        {
            return SelectedOrderType != null;
        }

        private void OnRemoveOrderTypeExecute()
        {
        }

        #region Navigation

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
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