using ERService.Infrastructure.Base;
using Prism.Events;
using Prism.Regions;
using System;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class UserSettingsViewModel : DetailViewModelBase, INavigationAware
    {
        public UserSettingsViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public override Task LoadAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        protected override bool OnCancelEditCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnCancelEditExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }
    }
}
