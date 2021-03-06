﻿using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ERService.Services.Tasks
{
    public class BackgroundTask<T> : IBackgroundTask where T : ITaskRunnable
    {
        private const string _runMethodName = "Run";
        private readonly string _cronExpression;

        public BackgroundTask(string cronExpression)
        {
            _cronExpression = cronExpression;
        }

        public string CronExpression { get { return _cronExpression; } }
        public string TaskName { get { return typeof(T).Name; } }        
        public Type Type { get { return typeof(T); } }
        public MethodInfo MethodInfo { get { return typeof(T).GetMethod(_runMethodName); } }                
    }

    public interface ITaskRunnable
    {
        Task Run();
    }
}
