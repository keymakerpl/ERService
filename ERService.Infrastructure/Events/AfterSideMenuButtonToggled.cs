using Prism.Events;
using System;

namespace ERService.Infrastructure.Events
{
    public class AfterSideMenuButtonToggled : PubSubEvent<AfterSideMenuButtonToggledArgs>
    {
    }

    public class AfterSideMenuButtonToggledArgs
    {
        public SideFlyouts Flyout { get; set; }
        public Guid DetailID { get; set; }
        public string ViewName { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public enum SideFlyouts
    {
        NotificationFlyout,
        DetailFlyout
    }
}
