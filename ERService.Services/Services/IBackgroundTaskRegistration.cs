using System.Collections.Generic;

namespace ERService.Services.Services
{
    public interface IBackgroundTaskRegistration
    {
        void Register(BackgroundTask backgroundTask);
        IEnumerable<BackgroundTask> Tasks();
    }
}