using ERService.Infrastructure.Base;
using ERService.Business;
using Prism.Events;
using System;
using Prism.Regions;
using ERService.MSSQLDataAccess;
using ERService.Infrastructure.Constants;
using ERService.RBAC;
using ERService.Infrastructure.Dialogs;
using System.Windows;

namespace ERService.CustomerModule.ViewModels
{
    public class CustomerListViewModel : ListModelBase<Customer, ERServiceDbContext>, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        private IRBACManager _rbacManager;
        private IMessageDialogService _dialogService;

        public CustomerListViewModel(
            ERServiceDbContext context,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IRBACManager rBACManager,
            IMessageDialogService dialogService) : base(context, regionManager, eventAggregator)
        {
            _rbacManager = rBACManager;
            _dialogService = dialogService;                                    
        }        

        //TODO: Refactor with OnMouseDoubleClick
        public async override void OnAddExecute()
        {
            if (!_rbacManager.LoggedUserHasPermission(AclVerbNames.CanAddCustomer))
            {
                await _dialogService.ShowAccessDeniedMessageAsync(this);
                return;
            }

            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("ViewFullName", ViewNames.CustomerView);

            ShowDetail(parameters);
        }

        public override async void OnDeleteExecute()
        {
            if (!_rbacManager.LoggedUserHasPermission(AclVerbNames.CanDeleteCustomer))
            {
                await _dialogService.ShowAccessDeniedMessageAsync(this);
                return;
            }

            var confirmDialogResult = await _dialogService.ShowConfirmationMessageAsync(this, "Usuwanie klienta..."
                , $"Czy usunąć klienta {SelectedModel.FirstName} {SelectedModel.LastName}?");

            if (confirmDialogResult == DialogResult.OK)
            {
                try
                {
                    base.OnDeleteExecute();
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
            if (SelectedModel != null)
            {
                var parameters = new NavigationParameters();
                parameters.Add("ID", SelectedModel.Id);
                parameters.Add("ViewFullName", ViewNames.CustomerView);

                ShowDetail(parameters);
            }
        }

        #region Navigation

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Load(c => c.Id != Guid.Empty, o => o.Orders);
        }

        public bool KeepAlive => false;

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        #endregion
    }
}
