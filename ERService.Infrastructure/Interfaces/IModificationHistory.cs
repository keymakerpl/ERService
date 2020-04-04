using System;

namespace ERService.Infrastructure.Interfaces
{
    public interface IModificationHistory
    {
        DateTime DateAdded { get; set; }
        DateTime? DateModified { get; set; }
    }
}
