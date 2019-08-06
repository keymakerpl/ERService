using System;

namespace ERService.Infrastructure.Events
{
    public interface IAfterAuthorisedEventArgs
    {
        Guid UserID { get; set; }
        string UserLastName { get; set; }
        string UserLogin { get; set; }
        string UserName { get; set; }
    }
}