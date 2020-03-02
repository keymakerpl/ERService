using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Base.Common.Collections;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using ERService.OrderModule.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.Notification.ViewModels
{
    public class DisplayableOrderItem
    {
        public string CustomerName { get; set; }
        public string Fault { get; set; }
        public Guid OrderID { get; set; }
        public string OrderNumber { get; set; }
    }

    public class NotificationListViewModel : DetailViewModelBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRegionManager _regionManager;

        public NotificationListViewModel(
            IRegionManager regionManager,
            IOrderRepository orderRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            _orderRepository = orderRepository;
            _regionManager = regionManager;

            Orders = new ObservableQueue<DisplayableOrderItem>(4);
            ShowOrderCommand = new DelegateCommand<object>(OnShowOrderExecute);

            _eventAggregator.GetEvent<AfterNewOrdersAddedEvent>().Subscribe(OnNewOrdersAdded, ThreadOption.UIThread);
        }

        public override bool KeepAlive => true;

        public ObservableQueue<DisplayableOrderItem> Orders { get; }

        public DelegateCommand<object> ShowOrderCommand { get; }

        public override async Task LoadAsync()
        {
            var query = new QueryBuilder<Order>();
            query.OrderByDesc(nameof(Order.DateAdded));
            query.Limit(4);

            var ids = await _orderRepository.GetIDsBy(query);
            LoadOrders(ids);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }

        private void LoadOrders(Guid[] ids)
        {
            var orders = _orderRepository.FindByInclude(o => ids.Contains(o.Id), o => o.Customer);

            foreach (var order in orders)
            {
                if (Orders.Count() == 4)
                    Orders.Dequeue();

                if (Orders.Where(o => o.OrderID == order.Id).Count() > 1)
                    continue;

                Orders.Enqueue(new DisplayableOrderItem()
                {
                    OrderID = order.Id,
                    OrderNumber = order.OrderNumber,
                    CustomerName = order.Customer.FullName,
                    Fault = order.Fault
                });
            }
        }

        private void OnNewOrdersAdded(AfterNewOrdersAddedEventArgs args)
        {
            if (args.NewItemsIDs.Length == 0)
                return;

            var ids = args.NewItemsIDs;
            LoadOrders(ids);
        }

        private void OnShowOrderExecute(object args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));            

            var parameters = new NavigationParameters();
            parameters.Add("ID", args);

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.OrderView, parameters);
            _eventAggregator.GetEvent<AfterSideMenuButtonToggled>().Publish(new AfterSideMenuButtonToggledArgs() { Flyout = SideFlyouts.NotificationFlyout });
        }
    }
}