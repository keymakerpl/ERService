using ERService.Infrastructure.Base;
using ERService.Business;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ERService.CustomerModule.Repository;
using Prism.Regions;
using ERService.CustomerModule.Views;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Events;
using Prism.Commands;

namespace ERService.CustomerModule.ViewModels
{
    //TODO: Refactor to DetailListModel
    //TODO: Move INavigation interface to base class
    /// <summary>
    /// Refactor to ListModelBase
    /// </summary>
    public class CustomerListViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        public DelegateCommand AddCommand { get; private set; }
        private ICustomerRepository _repository;
        private IRegionManager _regionManager;

        public CustomerListViewModel(IRegionManager regionManager, ICustomerRepository repository, 
            IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _repository = repository;
            _regionManager = regionManager;

            AddCommand = new DelegateCommand(OnAddExecuteCommand);

            eventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Subscribe(OnAfterDetailSavedHandler);

            Customers = new ObservableCollection<Customer>();
            LoadCustomersAsync();
        }

        private void OnAddExecuteCommand()
        {
            OpenDetail(Guid.Empty);
        }

        private void OnAfterDetailSavedHandler(AfterDetailSavedEventArgs obj)
        {
            LoadCustomersAsync();
        }
        
        public void OnMouseDoubleClickExecute()
        {
            if (SelectedCustomer != null)
            {
                OpenDetail(SelectedCustomer.Id);
            }
        }

        private void OpenDetail(Guid detailID)
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", detailID);

            var uri = new Uri(typeof(CustomerView).FullName + parameters, UriKind.Relative);
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            _regionManager.RequestNavigate(RegionNames.ContentRegion, uri);
        }

        private async void LoadCustomersAsync()
        {
            var customers = await _repository.GetAllAsync();
            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }

        public Customer SelectedCustomer { get; set; }

        public ObservableCollection<Customer> Customers { get; }

        public bool KeepAlive => false;

        public override Task LoadAsync(Guid id)
        {
            return new Task(null);
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            
        }
    }
}
