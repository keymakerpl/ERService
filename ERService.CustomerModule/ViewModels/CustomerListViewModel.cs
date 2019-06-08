using ERService.Infrastructure.Base;
using ERService.Business;
using Prism.Events;
using System;
using Prism.Regions;
using ERService.CustomerModule.Views;
using Prism.Commands;
using ERService.MSSQLDataAccess;
using ERService.Infrastructure.Constants;

namespace ERService.CustomerModule.ViewModels
{
    public class CustomerListViewModel : ListModelBase<Customer, ERServiceDbContext>, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        public CustomerListViewModel(ERServiceDbContext context, IRegionManager regionManager, 
            IEventAggregator eventAggregator) : base(context, regionManager)
        {        
            AddCommand = new DelegateCommand(OnAddExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);                        
        }        

        //TODO: Refactor with OnMouseDoubleClick
        public override void OnAddExecute()
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("ViewFullName", ViewNames.CustomerView);

            ShowDetail(parameters);
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
            LoadAsync();
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
