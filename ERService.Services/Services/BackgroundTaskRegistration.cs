using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Events;
using ERService.Infrastructure.Events;
using ERService.Services.Tasks;

namespace ERService.Services.Services
{

    public class BackgroundTaskRegistration : Collection<IBackgroundTask>, IBackgroundTaskRegistration
    {
        private readonly IEventAggregator _eventAggregator;

        public BackgroundTaskRegistration(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public IEnumerable<IBackgroundTask> Tasks()
        {
            return this;
        }

        public void Register(IBackgroundTask backgroundTask)
        {
            Add(backgroundTask);
            _eventAggregator.GetEvent<AfterTaskRegistrationChanged>().Publish();
        }

        public IBackgroundTask this[string taskName]
        {
            get { return this.FirstOrDefault(j => j.TaskName == taskName); }
        }        
    }
}
