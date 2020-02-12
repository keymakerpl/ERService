using System.Collections.Generic;
using Hangfire;
using Hangfire.Server;
using Prism.Events;
using ERService.Infrastructure.Events;

namespace ERService.Services.Services
{
    public class BackgroundTaskService : IBackgroundTaskService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundProcessingServer _backgroundProcessingServer;
        private readonly IBackgroundTaskRegistration _backgroundTaskRegistration;
        private readonly IEventAggregator _eventAggregator;

        private IEnumerable<BackgroundTask> BackgroundTasks => _backgroundTaskRegistration.Tasks();

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
            
        }

        public void Start()
        {
            ActivateTasks();
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

        private void Activate(BackgroundTask backgroundTask)
        {
            _recurringJobManager.AddOrUpdate(backgroundTask.TaskName, backgroundTask.Job, backgroundTask.CronExpression);
        }
    }
}
