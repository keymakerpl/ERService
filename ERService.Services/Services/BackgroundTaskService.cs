using System.Collections.Generic;
using Hangfire;
using Hangfire.Server;
using Prism.Events;
using ERService.Infrastructure.Events;
using ERService.Services.Tasks;
using Hangfire.Common;

namespace ERService.Services.Services
{
    public class BackgroundTaskService : IBackgroundTaskService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundProcessingServer _backgroundProcessingServer;
        private readonly IBackgroundTaskRegistration _backgroundTaskRegistration;
        private readonly IEventAggregator _eventAggregator;

        private IEnumerable<IBackgroundTask> BackgroundTasks => _backgroundTaskRegistration.Tasks();

        public BackgroundTaskService(
            IBackgroundProcessingServer backgroundProcessingServer,
            IRecurringJobManager recurringJobManager,
            IBackgroundTaskRegistration backgroundTaskRegistration,
            IEventAggregator eventAggregator)
        {
            _recurringJobManager = recurringJobManager;
            _backgroundProcessingServer = backgroundProcessingServer;
            _backgroundTaskRegistration = backgroundTaskRegistration;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<AfterTaskRegistrationChanged>()
                            .Subscribe(() =>
                            {
                                Refresh();
                            }, true);

            Refresh();
        }

        public void Stop()
        {
            _backgroundProcessingServer.SendStop();
        }

        public void Refresh()
        {
            ActivateTasks();
        }

        private void ActivateTasks()
        {
            foreach (var task in BackgroundTasks)
            {
                Activate(task);
            }
        }

        private void Activate(IBackgroundTask backgroundTask)
        {
            var job = new Job(backgroundTask.Type, backgroundTask.MethodInfo);
            _recurringJobManager.AddOrUpdate(backgroundTask.TaskName, job, backgroundTask.CronExpression);
        }
    }
}
