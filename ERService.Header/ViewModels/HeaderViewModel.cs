using System;
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

        public bool BadgeIsVisible { get; set; }

        public HeaderViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            _eventAggregator.GetEvent<AfterNewOrdersAddedEvent>().Subscribe(OnNewOrdersAdded, true);

            SideMenuToggleCommand = new DelegateCommand(OnSideMenuToggleExecute);
        }

        private void OnNewOrdersAdded(AfterNewOrdersAddedEventArgs args)
        {
            BadgeIsVisible = true;
            BadgeValue = args.NewItemsIDs.Length;
        }

        public int BadgeValue
        {
            get { return _badgeValue; }
            set { SetProperty(ref _badgeValue, value); }
        }

        public DelegateCommand SideMenuToggleCommand { get; }
        private void OnSideMenuToggleExecute()
        {
            BadgeIsVisible = false;
            BadgeValue = 0;

            _eventAggregator.GetEvent<AfterSideMenuButtonToggled>().Publish(new AfterSideMenuButtonToggledArgs() { Flyout = SideFlyouts.NotificationFlyout });
        }
    }
}