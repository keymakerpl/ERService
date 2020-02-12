using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Events;
using ERService.Infrastructure.Events;

namespace ERService.Services.Services
{

    public class BackgroundTaskRegistration : Collection<BackgroundTask>, IBackgroundTaskRegistration
    {
        private readonly IEventAggregator _eventAggregator;

        public BackgroundTaskRegistration(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public IEnumerable<BackgroundTask> Tasks()
        {
            return this;
        }

        public void Register(BackgroundTask backgroundTask)
        {
            Add(backgroundTask);
            _eventAggregator.GetEvent<AfterTaskRegistrationChanged>().Publish();
        }

        public BackgroundTask this[string taskName]
        {
            get { return this.FirstOrDefault(j => j.TaskName == taskName); }
        }        
    }
}
