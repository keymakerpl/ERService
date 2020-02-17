namespace ERService.Services.Services
{
    public interface IBackgroundTaskService
    {
        void Stop();
        void Refresh();
    }
}