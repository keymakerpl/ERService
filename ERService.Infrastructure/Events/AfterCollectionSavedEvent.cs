using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class AfterCollectionSavedEvent : PubSubEvent<AfterCollectionSavedEventArgs>
    {
    }

    public class AfterCollectionSavedEventArgs
    {
        public AfterCollectionSavedEventArgs()
        {
        }

        public string ViewModelName { get; set; }
    }
}