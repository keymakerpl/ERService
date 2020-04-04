using ERService.Services.Services;
using ERService.Services.Tasks;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Prism.Ioc;
using Prism.Modularity;
using System;
using Unity;

namespace ERService.Services
{
    public class ContainerJobActivator : JobActivator
    {
        private readonly IContainerProvider _container;

        public ContainerJobActivator(IContainerProvider container)
        {
            _container = container;
        }

        public override object ActivateJob(Type type)
        {
            return _container.Resolve(type);
        }
    }

    public class ServicesModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var container = new ContainerJobActivator(containerProvider);

            GlobalConfiguration.Configuration.UseMemoryStorage();
            GlobalConfiguration.Configuration.UseActivator(container);

            containerProvider.Resolve(typeof(BackgroundTaskService));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry   
                                .RegisterSingleton<IBackgroundProcessingServer, BackgroundJobServer>()
                                .RegisterSingleton<IRecurringJobManager, RecurringJobManager>()
                                .RegisterSingleton<IBackgroundTaskService, BackgroundTaskService>();
        }
    }
}