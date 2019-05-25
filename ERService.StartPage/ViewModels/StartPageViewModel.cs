using System;
using ERService.Infrastructure.Constants;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.StartPage.ViewModels
{
    public class StartPageViewModel : BindableBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        private string _message;
        private IRegionManager _regionManager;

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public bool KeepAlive => false;

        public StartPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            Message = "View StarPage from your Prism Module";
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
        }
    }
}
