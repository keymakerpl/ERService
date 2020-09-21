﻿using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;
using ERService.OrderModule.Data.Repository;
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
        private readonly IBlobRepository _blobRepository;
        private Order _selectedOrder;

        public OrderListViewModel(
            ERServiceDbContext context,
            IRegionManager regionManager,
            IRBACManager rBACManager,
            IBlobRepository blobRepository,
            IMessageDialogService messageDialogService,
            IEventAggregator eventAggregator) : base(context, regionManager, eventAggregator)
        {
            _rbacManager = rBACManager;
            _blobRepository = blobRepository;
            _dialogService = messageDialogService;

            SearchCommand = new DelegateCommand(OnSearchExecute);

            _eventAggregator.GetEvent<SearchEvent<Order>>().Subscribe(OnSearchRequest);
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

        private async void OnSearchRequest(SearchEventArgs<Order> args)
        {
            try
            {
                var predicate = args.Predicate;
                await LoadAsync(predicate, h => h.Hardwares, c => c.Customer, s => s.OrderStatus, t => t.OrderType, u => u.User);
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
                , $"Czy usunąć zlecenie numer: {SelectedOrder.OrderNumber}?");

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
                    _logger.Debug(ex);
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

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var orderNumber = navigationContext.Parameters.GetValue<string>("OrderNumber");
            if (orderNumber != null)
            {
                var predicate = PredicateBuilder.True<Order>().And(o => orderNumber == $"{o.OrderId}/{o.OrderNumber}");
                await LoadAsync(predicate, h => h.Hardwares, c => c.Customer, s => s.OrderStatus, t => t.OrderType);
            }
        }        

        #endregion Navigation
    }
}