using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.MSSQLDataAccess;
using ERService.OrderModule.Wrapper;
using ERService.RBAC;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ERService.OrderModule.ViewModels
{
    public class OrderListViewModel : ListModelBase<Order, ERServiceDbContext>, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        private IMessageDialogService _dialogService;
        private IRBACManager _rbacManager;

        public OrderListViewModel(ERServiceDbContext context, IRegionManager regionManager, IRBACManager rBACManager,
            IMessageDialogService messageDialogService) : base(context, regionManager)
        {
            _rbacManager = rBACManager;
            _dialogService = messageDialogService;

            Orders = new ObservableCollection<OrderWrapper>();
            Models.CollectionChanged += Models_CollectionChanged;

            AddCommand = new DelegateCommand(OnAddExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);
        }

        public OrderWrapper SelectedOrder { get; set; }
        public ObservableCollection<OrderWrapper> Orders { get; private set; }
        public bool KeepAlive => true;

        #region Events

        public async override void OnAddExecute()
        {
            if (!_rbacManager.LoggedUserHasPermission(AclVerbNames.CanAddOrder))
            {
                await _dialogService.ShowInformationMessageAsync(this, "Brak dostępu...", "Nie masz uprawnień do tej funkcji");
                return;
            }

            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("Wizard", true);
            parameters.Add("ViewFullName", ViewNames.CustomerView);

            ShowDetail(parameters);
        }

        public override bool OnDeleteCanExecute()
        {
            return SelectedOrder != null;
        }

        public async override void OnDeleteExecute()
        {
            if (!_rbacManager.LoggedUserHasPermission(AclVerbNames.CanDeleteOrder))
            {
                await _dialogService.ShowInformationMessageAsync(this, "Brak dostępu...", "Nie masz uprawnień do tej funkcji");
                return;
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
            LoadAsync();
        }

        private void Models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
            {
                Orders.Add(new OrderWrapper((Order)item));
            }
        }

        #endregion Navigation
    }
}