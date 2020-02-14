using Prism.Events;
using System;

namespace ERService.Infrastructure.Events
{
    public class AfterDetailSavedEvent : PubSubEvent<AfterDetailSavedEventArgs>
    {
    }

    public class AfterDetailSavedEventArgs
    {
        public Guid Id { get; set; }
        public string DisplayMember { get; set; }
        public string ViewModelName { get; set; }
    }
}