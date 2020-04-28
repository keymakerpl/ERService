using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.MSSQLDataAccess;
using ERService.OrderModule.Wrapper;
using ERService.RBAC;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Linq;
using ERService.Infrastructure.Repositories;
using Prism.Commands;

namespace ERService.OrderModule.ViewModels
{
    public class OrderListViewModel : ListModelBase<Order, ERServiceDbContext>, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private IMessageDialogService _dialogService;

        public DelegateCommand SearchCommand { get; }

        private IRBACManager _rbacManager;

        public OrderListViewModel(ERServiceDbContext context, IRegionManager regionManager, IRBACManager rBACManager,
            IMessageDialogService messageDialogService, IEventAggregator eventAggregator) : base(context, regionManager, eventAggregator)
        {
            _rbacManager = rBACManager;
            _dialogService = messageDialogService;

            SearchCommand = new DelegateCommand(OnSearchExecute);

            _eventAggregator.GetEvent<SearchQueryEvent>().Subscribe(OnSearchRequest);

            Orders = new ObservableCollection<OrderWrapper>();
            Models.CollectionChanged += Models_CollectionChanged;
        }

        private void OnSearchExecute()
        {
            _eventAggregator.GetEvent<AfterSideMenuExpandToggled>().Publish(new AfterSideMenuExpandToggledArgs() { Flyout = SideFlyouts.DetailFlyout, ViewName = ViewNames.OrderSearchView });
        }

        private async void OnSearchRequest(SearchQueryEventArgs args)
        {
            try
            {
                Orders.Clear();
                var ids = await GetIDsBy(args.QueryBuilder);
                Load(o => ids.Contains(o.Id), h => h.Hardwares, c => c.Customer, s => s.OrderStatus, t => t.OrderType);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private OrderWrapper _selectedOrder;
        public OrderWrapper SelectedOrder
        {
            get { return _selectedOrder; }
            set {_selectedOrder = value; SelectedModel = value?.Model; DeleteCommand.RaiseCanExecuteChanged(); }
        }

        public ObservableCollection<OrderWrapper> Orders { get; private set; }
        public bool KeepAlive => true;

        #region Events

        public async override void OnAddExecute()
        {
            if (!_rbacManager.LoggedUserHasPermission(AclVerbNames.CanAddOrder))
            {
                await _dialogService.ShowAccessDeniedMessageAsync(this);
                return;
            }

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.OrderWizardView);
        }

        public async override void OnDeleteExecute()
        {
            if (!_rbacManager.LoggedUserHasPermission(AclVerbNames.CanDeleteOrder))
            {
                await _dialogService.ShowAccessDeniedMessageAsync(this);
                return;
            }

            var confirmDialogResult = await _dialogService.ShowConfirmationMessageAsync(this, "Usuwanie zlecenia..."
                , $"Czy usunąć zlecenie numer: {SelectedOrder.Number}?");

            if (confirmDialogResult == DialogResult.OK)
            {
                try
                {
                    Remove(SelectedOrder.Model);
                    Orders.Remove(SelectedOrder);
                    await SaveAsync();
                }
                catch (Exception ex)
                {
                    //TODO: exception hunter
                    MessageBox.Show("Ups... " + Environment.NewLine +
                    Environment.NewLine + ex.Message);
                }
            }
        } 

        public override void OnMouseDoubleClickExecute()
        {
            if (SelectedOrder != null)
            {
                var parameters = new NavigationParameters();
                parameters.Add("ID", SelectedOrder.Id);
                parameters.Add("ViewFullName", ViewNames.OrderView);

                ShowDetail(parameters);
            }
        }        

        #endregion

        #region Navigation
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
            var orderNumber = navigationContext.Parameters.GetValue<string>("OrderNumber");
            if (orderNumber != null)
            {
                var query = new QueryBuilder(nameof(Order)).Select($"{nameof(Order)}.{nameof(Order.Id)}");
                query.WhereRaw($"(CAST([{nameof(Order.OrderId)}] AS NVARCHAR)+'/'+[{nameof(Order.Number)}]) = ?", orderNumber);

                OnSearchRequest(new SearchQueryEventArgs() { QueryBuilder = query } );
            }
        }

        private void Models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;
            foreach (var item in e.NewItems)
            {
                Orders.Add(new OrderWrapper((Order)item));
            }
        }

        #endregion Navigation
    }
}