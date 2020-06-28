using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Base.Common.Collections;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using ERService.OrderModule.Repository;
using ERService.RBAC;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
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
        private readonly IRBACManager _rbacManager;
        private readonly IRegionManager _regionManager;
        private string _userName;

        public NotificationListViewModel(
            IRegionManager regionManager,
            IOrderRepository orderRepository,
            IEventAggregator eventAggregator,
            IRBACManager rbacManager,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            _orderRepository = orderRepository;
            _rbacManager = rbacManager;
            _regionManager = regionManager;

            UserLogoutCommand = new DelegateCommand(OnUserLogoutExecute);
            UserSettingsCommand = new DelegateCommand(OnUserSettingsExecute);
            ShowOrderCommand = new DelegateCommand<object>(OnShowOrderExecute);

            LastOrders = new ObservableQueue<DisplayableOrderItem>(4);
            OutdatedOrders = new ObservableQueue<DisplayableOrderItem>(4);

            _eventAggregator.GetEvent<AfterNewOrdersAddedEvent>().Subscribe(OnNewOrdersAdded, ThreadOption.UIThread);
            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Subscribe(OnUserLoggedin, true);
            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>().Subscribe(OnUserLoggedout, true);
        }

        public override bool KeepAlive => true;

        public ObservableQueue<DisplayableOrderItem> LastOrders { get; }

        public ObservableQueue<DisplayableOrderItem> OutdatedOrders { get; }

        public DelegateCommand<object> ShowOrderCommand { get; }

        public DelegateCommand UserLogoutCommand { get; }

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        public DelegateCommand UserSettingsCommand { get; }

        public override async Task LoadAsync()
        {
            var lastOrdersQuery = new SQLQueryBuilder(nameof(Order))
                .Select($"{nameof(Order)}.{nameof(Order.Id)}")
                .OrderByDesc(nameof(Order.DateAdded))
                .Limit(4);
            
            await FillTable(lastOrdersQuery, LastOrders);

            var outdatedOrdersQuery = new SQLQueryBuilder(nameof(Order))
                .Select($"{nameof(Order)}.{nameof(Order.Id)}")
                .WhereNotNull(nameof(Order.DateEnded))
                .WhereDate(nameof(Order.DateEnded), SQLOperators.LessOrEqual, DateTime.Now)
                .OrderByDesc(nameof(Order.DateEnded))
                .Limit(4);

            await FillTable(outdatedOrdersQuery, OutdatedOrders);
        }

        private async Task FillTable(SqlKata.Query query, ObservableQueue<DisplayableOrderItem> listToFill)
        {
            var parameters = new object[0];
            var queryString = query.Compile(out parameters);
            var ids = await GetIDs(parameters, queryString);
            await LoadOrders(ids.ToArray(), listToFill);
        }

        private async Task<List<Guid>> GetIDs(object[] parameters, string queryString)
        {
            return await _orderRepository.GetAsync<Guid>(queryString, parameters).ConfigureAwait(false);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }        

        private async Task LoadOrders(Guid[] ids, ObservableQueue<DisplayableOrderItem> listToFill)
        {
            var orders = await _orderRepository.FindByIncludeAsync(o => ids.Contains(o.Id), o => o.Customer);

            foreach (var order in orders)
            {
                if (listToFill.Count() == 4)
                    listToFill.Dequeue();

                if (listToFill.Where(o => o.OrderID == order.Id).Count() > 1)
                    continue;

                listToFill.Enqueue(new DisplayableOrderItem()
                {
                    OrderID = order.Id,
                    OrderNumber = order.OrderNumber,
                    CustomerName = order.Customer.FullName,
                    Fault = order.Fault
                });
            }
        }

        private async void OnNewOrdersAdded(AfterNewOrdersAddedEventArgs args)
        {
            if (args.NewItemsIDs.Length == 0)
                return;

            var ids = args.NewItemsIDs;
            await LoadOrders(ids, LastOrders);
        }

        private void OnShowOrderExecute(object args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var parameters = new NavigationParameters();
            parameters.Add("ID", args);

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.OrderView, parameters);
        }

        private void OnUserLoggedin(UserAuthorizationEventArgs args)
        {
            UserName = !String.IsNullOrEmpty(args.UserLastName) ? $"{args.UserName} {args.UserLastName}" : args.UserLogin;
        }

        private void OnUserLoggedout(UserAuthorizationEventArgs args)
        {
            UserName = String.Empty;
        }

        private void OnUserLogoutExecute()
        {
            _rbacManager.Logout();
        }

        private void OnUserSettingsExecute()
        {
            if (_rbacManager.LoggedUser == null) return;

            _eventAggregator.GetEvent<AfterSideMenuExpandToggled>().Publish(new AfterSideMenuExpandToggledArgs
            {
                DetailID = _rbacManager.LoggedUser.Id,
                Flyout = SideFlyouts.DetailFlyout,
                ViewName = ViewNames.UserDetailView
            });
        }
    }
}