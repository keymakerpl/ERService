using ERService.Infrastructure.Base;
using ERService.MSSQLDataAccess;
using ERService.Business;
using Prism.Regions;
using Prism.Commands;

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
            //ShowDetail(Guid.Empty, RegionNames.ContentRegion, typeof(CustomerView).FullName);
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
