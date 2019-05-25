using ERService.Infrastructure.Base;
using ERService.Business;
using Prism.Events;
using System;
using Prism.Regions;
using ERService.CustomerModule.Views;
using ERService.Infrastructure.Constants;
using Prism.Commands;
using ERService.MSSQLDataAccess;

namespace ERService.CustomerModule.ViewModels
{
    public class CustomerListViewModel : ListViewModelBase<Customer, ERServiceDbContext>
    {
        public CustomerListViewModel(ERServiceDbContext context, IRegionManager regionManager, 
            IEventAggregator eventAggregator) : base(context, regionManager)
        {        
            AddCommand = new DelegateCommand(OnAddExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);
            
            LoadAsync();
        }

        public override void OnAddExecute()
        {
            ShowDetail(Guid.Empty, RegionNames.ContentRegion, typeof(CustomerView).FullName);
        }

        public override void OnMouseDoubleClickExecute()
        {
            if (SelectedModel != null)
            {
                ShowDetail(SelectedModel.Id, RegionNames.ContentRegion, typeof(CustomerView).FullName);
            }
        }            

    }
}
