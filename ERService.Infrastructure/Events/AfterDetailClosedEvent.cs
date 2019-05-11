using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class AfterDetailClosedEvent : PubSubEvent<AfterDetailClosedEventArgs>
    {
    }

    public class AfterDetailClosedEventArgs
    {
        public AfterDetailClosedEventArgs()
        {
        }

        public int Id { get; set; }
        public string ViewModelName { get; set; }
    }
}