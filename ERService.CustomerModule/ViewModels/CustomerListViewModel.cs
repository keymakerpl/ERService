﻿using ERService.Infrastructure.Base;
using ERService.Business;
using Prism.Events;
using System;
using Prism.Regions;
using ERService.MSSQLDataAccess;
using ERService.Infrastructure.Constants;
using ERService.RBAC;
using ERService.Infrastructure.Dialogs;
using Prism.Commands;
using ERService.Infrastructure.Events;
using System.Linq;
using System.Threading.Tasks;
using ERService.Infrastructure.Repositories;

namespace ERService.CustomerModule.ViewModels
{
    public class CustomerListViewModel : ListModelBase<Customer, ERServiceDbContext>, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        private IRBACManager _rbacManager;
        private IMessageDialogService _dialogService;

        public DelegateCommand SearchCommand { get; }

        public CustomerListViewModel(
            ERServiceDbContext context,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IRBACManager rBACManager,
            IMessageDialogService dialogService) : base(context, regionManager, eventAggregator)
        {
            _rbacManager = rBACManager;
            _dialogService = dialogService;

            SearchCommand = new DelegateCommand(OnSearchExecute);

            _eventAggregator.GetEvent<SearchQueryEvent>().Subscribe(OnSearchRequest);
        }

        private async void OnSearchRequest(SearchQueryEventArgs args)
        {
            try
            {
                var parameters = new object[0];
                var queryString = args.QueryBuilder.Compile(out parameters);

                await GetBy<Guid>(queryString, parameters)
                                                    .ContinueWith(async (t) => 
                                                    {
                                                        await LoadAsync(c => t.Result.Contains(c.Id), a => a.CustomerAddresses);
                                                    }, 
                                                    TaskContinuationOptions.ExecuteSynchronously);                
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnSearchExecute()
        {
            _eventAggregator.GetEvent<AfterSideMenuExpandToggled>().Publish(new AfterSideMenuExpandToggledArgs()
            {
                Flyout = SideFlyouts.DetailFlyout,
                ViewName = ViewNames.CustomerSearchView
            });
        }
        
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
                    _logger.Error(ex);
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
            
        }

        public bool KeepAlive => true;

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
