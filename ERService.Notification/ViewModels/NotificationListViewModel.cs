using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using ERService.OrderModule.Repository;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.Notification.ViewModels
{
    public class DisplayableOrderItem
    {
        public Guid OrderID { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string Fault { get; set; }
    }

    public class NotificationListViewModel : DetailViewModelBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRegionManager _regionManager;

        public ObservableCollection<DisplayableOrderItem> Orders { get; }

        public NotificationListViewModel(
            IRegionManager regionManager,
            IOrderRepository orderRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            _orderRepository = orderRepository;
            _regionManager = regionManager;

            Orders = new ObservableCollection<DisplayableOrderItem>();

            _eventAggregator.GetEvent<AfterNewOrdersAddedEvent>().Subscribe(OnNewOrdersAdded, ThreadOption.UIThread);
        }

        public override bool KeepAlive => true;

        private void OnNewOrdersAdded(AfterNewOrdersAddedEventArgs args)
        {
            if (args.NewItemsIDs.Length == 0)
                return;

            var ids = args.NewItemsIDs;
            LoadOrders(ids);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }

        public override async Task LoadAsync()
        {
            var query = new QueryBuilder<Order>();
            query.OrderByDesc(nameof(Order.DateAdded));
            query.Limit(4);

            var ids = await _orderRepository.GetIDsBy(query);
            LoadOrders(ids);
        }

        private void LoadOrders(Guid[] ids)
        {
            var orders = _orderRepository.FindByInclude(o => ids.Contains(o.Id), o => o.Customer);

            Orders.Clear();
            foreach (var order in orders)
            {
                Orders.Add(new DisplayableOrderItem()
                {
                    OrderID = order.Id,
                    OrderNumber = order.OrderNumber,
                    CustomerName = order.Customer.FullName,
                    Fault = order.Fault
                });
            }
        }
    }
}
