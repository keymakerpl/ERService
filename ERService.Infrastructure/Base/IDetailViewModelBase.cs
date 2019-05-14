using System;
using System.Threading.Tasks;

namespace ERService.Infrastructure.Base
{
    public interface IDetailViewModelBase
    {
        bool HasChanges { get; set; }
        
        Guid ID { get; }

        Task LoadAsync(Guid id);
    }
}