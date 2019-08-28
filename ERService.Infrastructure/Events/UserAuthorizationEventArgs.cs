using System;

namespace ERService.Infrastructure.Events
{
    public class UserAuthorizationEventArgs
    {
        public Guid UserID { get; set; }

        public string UserLogin { get; set; }

        public string UserName { get; set; }

        public string UserLastName { get; set; }
    }
}
