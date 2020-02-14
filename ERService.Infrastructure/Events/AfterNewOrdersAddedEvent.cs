using Prism.Events;
using System;

namespace ERService.Infrastructure.Events
{
    public class AfterNewOrdersAddedEvent : PubSubEvent<AfterNewOrdersAddedEventArgs>
    {
    }

    public class AfterNewOrdersAddedEventArgs
    {
        public Guid[] NewItemsIDs { get; set; }
    }
}
