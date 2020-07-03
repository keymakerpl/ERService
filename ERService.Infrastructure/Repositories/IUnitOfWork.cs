namespace ERService.Infrastructure.Repositories
{
    public interface IUnitOfWork
    {
        void CommitTransaction();
        void StartTransaction();
    }
}