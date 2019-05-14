using Prism.Events;
using System;

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

        public Guid Id { get; set; }
        public string ViewModelName { get; set; }
    }
}