using ERService.Infrastructure.Base;
using ERService.MSSQLDataAccess;
using ERService.Business;
using Prism.Regions;
using Prism.Commands;
using System;
using ERService.Infrastructure.Constants;
using ERService.OrderModule.Views;
using ERService.CustomerModule.Views;

namespace ERService.OrderModule.ViewModels
{
    public class OrderListViewModel : ListModelBase<Order, ERServiceDbContext>
    {
        public OrderListViewModel(ERServiceDbContext context, IRegionManager regionManager) : base(context, regionManager)
        {
            AddCommand = new DelegateCommand(OnAddExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);

            LoadAsync();
        }

        public override void OnAddExecute()
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("Wizard", true);
            parameters.Add("ViewFullName", typeof(CustomerView).FullName);

            ShowDetail(parameters);
        }

        public override void OnMouseDoubleClickExecute()
        {
            if (SelectedModel != null)
            {
                //ShowDetail(SelectedModel.Id, RegionNames.ContentRegion, typeof(CustomerView).FullName);
            }
        }
    }
}
