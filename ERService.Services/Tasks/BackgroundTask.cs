using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Hangfire;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Unity;
using System.Linq;
using Prism.Events;
using ERService.Infrastructure.Events;

namespace ERService.Services.Services
{

    public abstract class BackgroundTask
    {
        private const string RunMethodName = "Run";

        public string TaskName { get { return GetType().Name; } }        
        public abstract string CronExpression { get; }
        public Type Type { get { return GetType(); } }
        public MethodInfo MethodInfo { get { return GetType().GetMethod(RunMethodName); } }
        public abstract void Run();
        public Job Job { get { return new Job(Type, MethodInfo); } }
    }
}
