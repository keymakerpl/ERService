using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class AfterSideMenuButtonToggled : PubSubEvent<AfterSideMenuButtonToggledArgs>
    {
    }

    public class AfterSideMenuButtonToggledArgs
    {
        public SideFlyouts FlyoutSide;
    }

    public enum SideFlyouts
    {
        RightSide,
        BottomSearch
    }
}
