using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class AfterUserLoggedinEvent : PubSubEvent<UserAuthorizationEventArgs>
    {
    }    
}
