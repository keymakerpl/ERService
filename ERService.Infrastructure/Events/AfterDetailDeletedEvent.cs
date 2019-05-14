using Prism.Events;
using System;

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

        public Guid Id { get; set; }
        public string ViewModelName { get; set; }
    }
}