namespace ERService.Infrastructure.Interfaces
{
    public interface IVersionedRow
    {
        long RowVersion { get; set; }
    }
}
