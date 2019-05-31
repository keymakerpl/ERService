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
    public class CustomerListViewModel : ListModelBase<Customer, ERServiceDbContext>
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
            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("ViewFullName", typeof(CustomerView).FullName);

            ShowDetail(parameters);
        }

        public override void OnMouseDoubleClickExecute()
        {
            if (SelectedModel != null)
            {
                var parameters = new NavigationParameters();
                parameters.Add("ID", SelectedModel.Id);
                parameters.Add("ViewFullName", typeof(CustomerView).FullName);

                ShowDetail(parameters);
            }
        }            

    }
}
