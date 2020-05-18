using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using ERService.RBAC;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.OrderModule.ViewModels
{
    public class OrderListViewModel : ListModelBase<Order, ERServiceDbContext>, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private IMessageDialogService _dialogService;

        private IRBACManager _rbacManager;
        private Order _selectedOrder;
        public OrderListViewModel(ERServiceDbContext context, IRegionManager regionManager, IRBACManager rBACManager,
            IMessageDialogService messageDialogService, IEventAggregator eventAggregator) : base(context, regionManager, eventAggregator)
        {
            _rbacManager = rBACManager;
            _dialogService = messageDialogService;

            SearchCommand = new DelegateCommand(OnSearchExecute);

            _eventAggregator.GetEvent<SearchQueryEvent>().Subscribe(OnSearchRequest);
        }        

        public DelegateCommand SearchCommand { get; }

        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set { _selectedOrder = value; SelectedModel = value; DeleteCommand.RaiseCanExecuteChanged(); }
        }

        #region Events

        private void OnSearchExecute()
        {
            _eventAggregator.GetEvent<AfterSideMenuExpandToggled>().Publish(new AfterSideMenuExpandToggledArgs()
            {
                Flyout = SideFlyouts.DetailFlyout,
                ViewName = ViewNames.OrderSearchView
            });
        }

        private async void OnSearchRequest(SearchQueryEventArgs args)
        {
            try
            {
                var parameters = new object[0];
                var queryString = args.QueryBuilder.Compile(out parameters);

                var ids = await GetBy<Guid>(queryString, parameters);
                await LoadAsync(o => ids.Contains(o.Id), h => h.Hardwares, c => c.Customer, s => s.OrderStatus, t => t.OrderType);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

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
                    Remove(SelectedOrder);                    
                    await SaveAsync().ContinueWith(async t => 
                    {
                        await RefreshListAsync();
                    }
                    , TaskContinuationOptions.ExecuteSynchronously);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
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

        #endregion Events

        #region Navigation

        public bool KeepAlive => true;

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

                OnSearchRequest(new SearchQueryEventArgs() { QueryBuilder = query });
            }
        }        

        #endregion Navigation
    }
}