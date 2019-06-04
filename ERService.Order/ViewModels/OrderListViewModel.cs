using ERService.Infrastructure.Base;
using ERService.MSSQLDataAccess;
using ERService.Business;
using Prism.Regions;
using Prism.Commands;
using System;

namespace ERService.OrderModule.ViewModels
{
    public class OrderListViewModel : ListModelBase<Order, ERServiceDbContext>, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        public OrderListViewModel(ERServiceDbContext context, IRegionManager regionManager) : base(context, regionManager)
        {
            AddCommand = new DelegateCommand(OnAddExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);            
        }        

        public override void OnAddExecute()
        {
            //var parameters = new NavigationParameters();
            //parameters.Add("ID", Guid.Empty);
            //parameters.Add("Wizard", true);
            //parameters.Add("ViewFullName", typeof(CustomerView).FullName);

            //ShowDetail(parameters);
        }

        public override void OnMouseDoubleClickExecute()
        {
            if (SelectedModel != null)
            {
                //ShowDetail(SelectedModel.Id, RegionNames.ContentRegion, typeof(CustomerView).FullName);
            }
        }

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
            LoadAsync();
        }

        #endregion
    }
}
