namespace ERService.Startup
{
    public interface IERBootstrap
    {
        void ColdStart();
        void HotStart();
    }
}