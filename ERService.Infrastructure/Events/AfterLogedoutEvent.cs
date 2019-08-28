using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class AfterUserLoggedoutEvent : PubSubEvent<UserAuthorizationEventArgs>
    {
    }
}
