using CronEspresso.NETCore;
using ERService.Services.Services;

namespace ERService.OrderModule.Tasks
{

    public class NewOrdersNotificationBackgroundTask : BackgroundTask
    {
        public override string CronExpression { get => "*/1 * * * *"; }

        public override void Run()
        {
            System.Console.WriteLine("[DEBUG] Task completed.");
        }
    }
}
