using System.Threading.Tasks;

namespace ERService.Infrastructure.Base
{
    public interface IDetailViewModelBase
    {
        bool HasChanges { get; set; }

        //TODO: Change to Guid
        int Id { get; }

        Task LoadAsync(int id);
    }
}