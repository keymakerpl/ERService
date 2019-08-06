using Prism.Events;
using System;

namespace ERService.Infrastructure.Events
{
    public class AfterAuthorisedEvent : PubSubEvent<AfterAuthorisedEventArgs>
    {
    }

    public class AfterAuthorisedEventArgs : IAfterAuthorisedEventArgs
    {
        public AfterAuthorisedEventArgs()
        {

        }

        public Guid UserID { get; set; }

        public string UserLogin { get; set; }

        public string UserName { get; set; }

        public string UserLastName { get; set; }
    }
}
