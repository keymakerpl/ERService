using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class AfterLogedoutEvent : PubSubEvent<UserAuthorizationEventArgs>
    {
    }
}
