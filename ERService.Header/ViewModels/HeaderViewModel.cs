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
        private int? _badgeValue = null;
        private bool _isToogleButtonVisible;

        public HeaderViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            _eventAggregator.GetEvent<AfterNewOrdersAddedEvent>().Subscribe(OnNewOrdersAdded, true);
            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Subscribe((a) => IsToogleButtonVisible = true);
            _eventAggregator.GetEvent<AfterUserLoggedoutEvent>().Subscribe((a) => IsToogleButtonVisible = false);

            SideMenuToggleCommand = new DelegateCommand(OnSideMenuToggleExecute);
        }

        public int? BadgeValue
        {
            get { return _badgeValue; }
            set { SetProperty(ref _badgeValue, value); }
        }

        public bool IsToogleButtonVisible
        {
            get { return _isToogleButtonVisible; }
            set { SetProperty(ref _isToogleButtonVisible, value); }
        }

        public DelegateCommand SideMenuToggleCommand { get; }

        private void OnNewOrdersAdded(AfterNewOrdersAddedEventArgs args)
        {
            BadgeValue = args.NewItemsIDs.Length;
        }

        private void OnSideMenuToggleExecute()
        {
            BadgeValue = null;

            _eventAggregator
                .GetEvent<AfterSideMenuExpandToggled>()
                .Publish(new AfterSideMenuExpandToggledArgs()
                {
                    Flyout = SideFlyouts.NotificationFlyout
                });
        }
    }
}