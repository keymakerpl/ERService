using System.Collections.Generic;

namespace ERService.Services.Tasks
{
    public interface IBackgroundTaskRegistration
    {
        void Register(IBackgroundTask backgroundTask);
        IEnumerable<IBackgroundTask> Tasks();
    }
}