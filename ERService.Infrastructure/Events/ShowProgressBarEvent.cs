using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class ShowProgressBarEvent : PubSubEvent<ShowProgressBarEventArgs>
    {
    }

    public class ShowProgressBarEventArgs
    {
        public bool IsShowing { get; set; }
    }
}