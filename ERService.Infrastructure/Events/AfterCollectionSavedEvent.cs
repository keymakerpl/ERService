using Prism.Events;

namespace ERService.Infrastructure.Events
{
    internal class AfterCollectionSavedEvent : PubSubEvent<AfterCollectionSavedEventArgs>
    {
    }

    internal class AfterCollectionSavedEventArgs
    {
        public AfterCollectionSavedEventArgs()
        {
        }

        public string ViewModelName { get; set; }
    }
}