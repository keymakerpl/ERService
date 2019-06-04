using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using Prism.Events;
using Prism.Regions;
using System;
using System.Threading.Tasks;

namespace ERService.HardwareModule.ViewModels
{
    public class HardwareViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        public bool KeepAlive => true;
        
        private IRegionManager _regionManager;
        private IRegionNavigationService _navigationService;

        public HardwareViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _regionManager = regionManager;
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
            return true;
        }

        protected override void OnCancelEditExecute()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            _navigationService.Journal.GoBack();
        }

        protected override bool OnCancelEditCanExecute()
        {
            return true;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            var id = navigationContext.Parameters.GetValue<string>("ID");
            if (!String.IsNullOrWhiteSpace(id))
            {
                //await LoadAsync(Guid.Parse(id));
            }
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
