﻿using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class AfterAuthorisedEvent : PubSubEvent<UserAuthorizationEventArgs>
    {
    }
}
