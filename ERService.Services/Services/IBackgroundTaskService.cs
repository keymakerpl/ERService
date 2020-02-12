namespace ERService.Services.Services
{
    public interface IBackgroundTaskService
    {
        void Start();
        void Stop();
        void Refresh();
    }
}