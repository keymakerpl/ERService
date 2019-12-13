using Prism.Events;

namespace ERService.Infrastructure.Events
{
    public class AfterLicenseValidationRequestEvent : PubSubEvent<AfterLicenseValidationRequestEventArgs>
    {
    }

    public class AfterLicenseValidationRequestEventArgs
    {
        public bool IsValid { get; set; }
    }
}
