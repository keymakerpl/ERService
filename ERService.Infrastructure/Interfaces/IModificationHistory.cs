using System;

namespace ERService.Infrastructure.Interfaces
{
    //TODO: Zaimplementować w modelu krotek
    public interface IModificationHistory
    {
        DateTime DateAdded { get; set; }
        DateTime DateModified { get; set; }
        bool IsDirty { get; set; }
    }
}
