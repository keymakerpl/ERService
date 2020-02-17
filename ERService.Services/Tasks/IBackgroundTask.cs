using System;
using System.Reflection;

namespace ERService.Services.Tasks
{
    public interface IBackgroundTask
    {
        string CronExpression { get; }
        MethodInfo MethodInfo { get; }
        string TaskName { get; }
        Type Type { get; }
    }
}