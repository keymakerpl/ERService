using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.OrderModule.Repository;
using ERService.OrderModule.Wrapper;
using MySqlX.XDevAPI.Common;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class StatusGroupLookupItem
    {
        public StatusGroup Group { get; set; }
        public string DisplayableName { get; set; }
    }

    public class StatusConfigViewModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private IOrderStatusRepository _orderStatusRepository;
        private IOrderTypeRepository _orderTypeRepository;
        private OrderStatusWrapper _selectedOrderStatus;
        private OrderTypeWrapper _selectedOrderType;
        private OrderStatusWrapper _newOrderStatus;

        public StatusConfigViewModel(IEventAggregator eventAggregator, IOrderStatusRepository orderStatusRepository,
             IOrderTypeRepository orderTypeRepository, IOrderRepository orderRepository, IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            Title = "Konfiguracja statusów";
            
            _orderStatusRepository = orderStatusRepository;
            _orderTypeRepository = orderTypeRepository;
            _orderRepository = orderRepository;

            _isNewStatusCollapsed = true;

            OrderTypes = new ObservableCollection<OrderTypeWrapper>();
            OrderStatuses = new ObservableCollection<OrderStatusWrapper>();
            Groups = new ObservableCollection<StatusGroupLookupItem>();

            AddOrderTypeCommand = new DelegateCommand(OnAddOrderTypeExecute);
            AddOrderStatusCommand = new DelegateCommand<object>(OnAddOrderStatusExecute, OnAddOrderStatusCanExecute);

            RemoveOrderTypeCommand = new DelegateCommand(OnRemoveOrderTypeExecute, OnRemoveOrderTypeCanExecute);
            RemoveOrderStatusCommand = new DelegateCommand(OnRemoveOrderStatusExecute, OnRemoveOrderStatusCanExecute);

            ToggleNewStatusPaneCommand = new DelegateCommand(OnNewStatusToggled);            
        }

        private bool OnAddOrderStatusCanExecute(object arg)
        {
            return NewOrderStatus != null && !NewOrderStatus.HasErrors;
        }

        private void OnNewStatusToggled()
        {
            NewOrderStatus = new OrderStatusWrapper(new OrderStatus());
            NewOrderStatus.PropertyChanged += WrappedStatus_PropertyChanged;

            NewOrderStatus.Name = "";
            NewOrderStatus.Group = StatusGroup.Open;

            IsNewStatusCollapsed = !IsNewStatusCollapsed;
        }

        public ObservableCollection<OrderStatusWrapper> OrderStatuses { get; set; }
        public ObservableCollection<StatusGroupLookupItem> Groups { get; }
        public ObservableCollection<OrderTypeWrapper> OrderTypes { get; set; }

        public DelegateCommand<object> AddOrderStatusCommand { get; }
        public DelegateCommand AddOrderTypeCommand { get; }
        public DelegateCommand RemoveOrderStatusCommand { get; }
        public DelegateCommand ToggleNewStatusPaneCommand { get; }
        public DelegateCommand RemoveOrderTypeCommand { get; }

        public OrderStatusWrapper SelectedOrderStatus
        {
            get { return _selectedOrderStatus; }
            set { SetProperty(ref _selectedOrderStatus, value); RemoveOrderStatusCommand.RaiseCanExecuteChanged(); }
        }
        public OrderTypeWrapper SelectedOrderType
        {
            get { return _selectedOrderType; }
            set { SetProperty(ref _selectedOrderType, value); RemoveOrderTypeCommand.RaiseCanExecuteChanged(); }
        }
        public OrderStatusWrapper NewOrderStatus
        {
            get { return _newOrderStatus; }
            set { SetProperty(ref _newOrderStatus, value); }
        }

        private bool _isNewStatusCollapsed;
        private readonly IOrderRepository _orderRepository;

        public bool IsNewStatusCollapsed
        {
            get { return _isNewStatusCollapsed; }
            set { SetProperty(ref _isNewStatusCollapsed, value); }
        }

        public async override Task LoadAsync()
        {
            await LoadStatuses();
            await LoadTypes();
            LoadLookups(); 
        }

        private void LoadLookups()
        {
            Groups.Add(new StatusGroupLookupItem() { Group = StatusGroup.Open, DisplayableName = "Otwarte" });
            Groups.Add(new StatusGroupLookupItem() { Group = StatusGroup.InProgress, DisplayableName = "W trakcie" });
            Groups.Add(new StatusGroupLookupItem() { Group = StatusGroup.Finished, DisplayableName = "Zamknięte" });            
        }

        private async Task LoadStatuses()
        {
            var statuses = await _orderStatusRepository.GetAllAsync();

            foreach (var status in OrderStatuses)
            {
                status.PropertyChanged -= WrappedStatus_PropertyChanged;
            }

            OrderStatuses.Clear();
            foreach (var status in statuses)
            {
                var wrappedStatus = new OrderStatusWrapper(status);
                wrappedStatus.PropertyChanged += WrappedStatus_PropertyChanged;
                OrderStatuses.Add(wrappedStatus);
            }

            OrderStatuses.CollectionChanged += (s, a) => SaveCommand.RaiseCanExecuteChanged();
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

            OrderTypes.CollectionChanged += (s, a) => SaveCommand.RaiseCanExecuteChanged();
        }

        #region Events and Event Handlers
        protected override bool OnSaveCanExecute()
        {
            return _orderStatusRepository.HasChanges() || _orderTypeRepository.HasChanges() || HasChanges;
        }

        protected async override void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_orderStatusRepository.SaveAsync, () => { });
            await SaveWithOptimisticConcurrencyAsync(_orderTypeRepository.SaveAsync, () => { });            

            HasChanges = _orderStatusRepository.HasChanges() || _orderTypeRepository.HasChanges();
        }

        private async void OnAddOrderStatusExecute(object arg)
        {
            if (OrderStatuses.Any(s => s.Name.Equals(NewOrderStatus.Name, StringComparison.OrdinalIgnoreCase)))
            {
                await _messageDialogService.ShowInformationMessageAsync(this, "Status naprawy...", "Status naprawy o takiej nazwie już istnieje");
                return;
            }

            var item = arg as StatusGroupLookupItem;
            if (item != null)
                NewOrderStatus.Group = item.Group;            

            _orderStatusRepository.Add(NewOrderStatus.Model);                       
            OrderStatuses.Add(NewOrderStatus);

            IsNewStatusCollapsed = true;
        }

        private async void OnAddOrderTypeExecute()
        {
            var wrappedOrderType = new OrderTypeWrapper(new OrderType());
            _orderTypeRepository.Add(wrappedOrderType.Model);
            wrappedOrderType.PropertyChanged += WrappedType_PropertyChanged;

            var newTypeName = await _messageDialogService.ShowInputMessageAsync(this, "Nowy typ naprawy...", "Podaj nazwę nowego typu: ");
            if (String.IsNullOrWhiteSpace(newTypeName)) 
                return;

            if (OrderTypes.Any(s => s.Name.Equals(newTypeName, StringComparison.OrdinalIgnoreCase)))
            {
                await _messageDialogService.ShowInformationMessageAsync(this, "Typ naprawy...", "Typ naprawy o takiej nazwie już istnieje");
                return;
            }
            wrappedOrderType.Name = newTypeName;

            OrderTypes.Add(wrappedOrderType);
        }
        
        private bool OnRemoveOrderStatusCanExecute()
        {
            return SelectedOrderStatus != null;
        }

        private async void OnRemoveOrderStatusExecute()
        {
            if (await IsOrderStatusInUse(_selectedOrderStatus.Model))
            {
                await _messageDialogService.ShowInformationMessageAsync(this, "Status naprawy w użyciu...", "Status naprawy jest w użyciu, nie można go usunąć.");
                return;
            }

            _orderStatusRepository.Remove(SelectedOrderStatus.Model);
            OrderStatuses.Remove(SelectedOrderStatus);
        }

        private async Task<bool> IsOrderStatusInUse(OrderStatus model)
        {
            var result = await _orderRepository.FindByIncludeAsync(o => o.OrderStatusId == model.Id);
            return result.Any();
        }

        private bool OnRemoveOrderTypeCanExecute()
        {
            return SelectedOrderType != null;
        }

        private async void OnRemoveOrderTypeExecute()
        {
            if (await IsOrderTypeInUse(_selectedOrderType.Model))
            {
                await _messageDialogService.ShowInformationMessageAsync(this, "Typ naprawy w użyciu...", "Typ naprawy jest w użyciu, nie można go usunąć.");
                return;
            }

            _orderTypeRepository.Remove(SelectedOrderType.Model);
            OrderTypes.Remove(SelectedOrderType);
        }

        private async Task<bool> IsOrderTypeInUse(OrderType model)
        {
            var result = await _orderRepository.FindByIncludeAsync(o => o.OrderTypeId == model.Id);
            return result.Any();
        }

        private void WrappedStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _orderTypeRepository.HasChanges();
            }

            SaveCommand.RaiseCanExecuteChanged();
            AddOrderStatusCommand.RaiseCanExecuteChanged();
        }
        
        private void WrappedType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges) 
            {
                HasChanges = _orderTypeRepository.HasChanges();
            }

            SaveCommand.RaiseCanExecuteChanged();
        }

        #endregion Events and Event Hanlers

        #region Navigation

        public override bool KeepAlive => true;

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }
        
        #endregion Navigation
    }
}