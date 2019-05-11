using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class AfterDetailDeletedEvent : PubSubEvent<AfterDetailDeletedEventArgs>
    {
    }

    public class AfterDetailDeletedEventArgs
    {
        public AfterDetailDeletedEventArgs()
        {
        }

        public int Id { get; set; }
        public string ViewModelName { get; set; }
    }
}