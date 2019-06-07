using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Threading.Tasks;

namespace ERService.HardwareModule.ViewModels
{
    public class HardwareViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        public bool KeepAlive => true;

        public bool WizardMode { get; set; }

        private IRegionManager _regionManager;

        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand GoForwardCommand { get; private set; }

        private IRegionNavigationService _navigationService;

        private Customer _customer;
        public Customer Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }

        public HardwareViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _regionManager = regionManager;
            GoBackCommand = new DelegateCommand(OnGoBackExecute);
            GoForwardCommand = new DelegateCommand(OnGoForwardExecute, OnGoForwardCanExecute);
        }

        private bool OnGoForwardCanExecute()
        {
            return WizardMode;
        }

        private void OnGoForwardExecute()
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("Wizard", true);

            //_regionManager.RequestNavigate(RegionNames.ContentRegion, typeof(OrderView).FullName, parameters);
        }

        private void OnGoBackExecute()
        {
            _navigationService.Journal.GoBack();
        }

        public override Task LoadAsync(Guid id)
        {
            return new Task(null);
        }

        protected override void OnSaveExecute()
        {
           
        }

        protected override bool OnSaveCanExecute()
        {
            return !WizardMode;
        }

        protected override void OnCancelEditExecute()
        {
            //_navigationService.Journal.GoBack();
        }

        protected override bool OnCancelEditCanExecute()
        {
            return true;
        }



        #region Navigation

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
            WizardMode = navigationContext.Parameters.GetValue<bool>("Wizard");

            var customer = navigationContext.Parameters.GetValue<Customer>("Customer");
            if (customer != null)
                Customer = customer;

            var id = navigationContext.Parameters.GetValue<Guid>("ID");
            //if (!String.IsNullOrWhiteSpace(id))
            //{
            //    //await LoadAsync(Guid.Parse(id));
            //}
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
            continuationCallback(true);
        }

        #endregion
    }
}
