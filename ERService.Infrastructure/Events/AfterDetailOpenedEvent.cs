using Prism.Events;
using System;

namespace ERService.Infrastructure.Events
{
    public class AfterDetailOpenedEvent : PubSubEvent<AfterDetailOpenedEventArgs>
    {
    }

    public class AfterDetailOpenedEventArgs
    {
        public Guid Id { get; set; }
        public string ViewModelName { get; set; }
        public string DisplayableName { get; set; }
    }
}
