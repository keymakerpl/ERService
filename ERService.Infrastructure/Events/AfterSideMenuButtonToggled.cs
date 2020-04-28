using Prism.Events;
using System;

namespace ERService.Infrastructure.Events
{
    public class AfterSideMenuExpandToggled : PubSubEvent<AfterSideMenuExpandToggledArgs>
    {
    }

    public class AfterSideMenuExpandToggledArgs
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
