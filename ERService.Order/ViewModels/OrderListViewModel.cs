using ERService.Infrastructure.Base;
using ERService.MSSQLDataAccess;
using ERService.Business;
using Prism.Regions;
using Prism.Commands;
using System;
using ERService.Infrastructure.Constants;
using System.Collections.ObjectModel;
using ERService.OrderModule.Wrapper;
using System.Collections.Specialized;
using ERService.RBAC;
using ERService.Infrastructure.Dialogs;

namespace ERService.OrderModule.ViewModels
{
    public class OrderListViewModel : ListModelBase<Order, ERServiceDbContext>, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {           
        public OrderWrapper SelectedOrder { get; set; }

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

        public async override void OnAddExecute()
        {
            //TODO: Trzeba to uprościć
            if(!_rbacManager.LoggedUserHasAccess(AclVerbNames.NewOrderPermission))
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

        #region Navigation

        public bool KeepAlive => true;

        private IACLVerbCollection _verbCollection;
        private IRBACManager _rbacManager;
        private IMessageDialogService _dialogService;

        public ObservableCollection<OrderWrapper> Orders { get; private set; }

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

        #endregion
    }
}
