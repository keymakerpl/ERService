using ERService.Infrastructure.Commands;
using ERService.Settings.Views;
using Prism.Commands;
using Prism.Regions;
using System;

namespace ERService.Navigation.ViewModels
{
    public class NavigationViewModel
    {
        public DelegateCommand OpenDetailViewCommand { get; }
        public IRegionManager _regionManager { get; private set; }

        public NavigationViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute);
        }

        private void OnOpenDetailViewExecute()
        {
            _regionManager.RequestNavigate("ContentRegion", new Uri(typeof(SettingsView).FullName, UriKind.Relative));
        }
    }
}
