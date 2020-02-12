using ERService.Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Header.ViewModels
{
    public class HeaderViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private int _badgeValue = 0;

        public HeaderViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            SideMenuToggleCommand = new DelegateCommand(OnSideMenuToggleExecute);
        }

        public int BadgeValue
        {
            get { return _badgeValue; }
            set { SetProperty(ref _badgeValue, value); }
        }

        public DelegateCommand SideMenuToggleCommand { get; }
        private void OnSideMenuToggleExecute()
        {
            _eventAggregator.GetEvent<AfterSideMenuButtonToggled>().Publish();
        }
    }
}