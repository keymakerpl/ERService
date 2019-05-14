using Prism.Events;
using System;

namespace ERService.Infrastructure.Events
{
    public class OpenDetailViewEvent : PubSubEvent<OpenDetailViewEventArgs>
    {
    }

    public class OpenDetailViewEventArgs
    {
        public Guid Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
