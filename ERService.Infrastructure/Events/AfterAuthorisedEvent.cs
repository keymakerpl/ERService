using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class AfterAuthorisedEvent : PubSubEvent<AfterAuthorisedEventArgs>
    {
    }

    public class AfterAuthorisedEventArgs
    {
        public AfterAuthorisedEventArgs()
        {

        }

        public string UserLogin { get; set; }
    }
}
