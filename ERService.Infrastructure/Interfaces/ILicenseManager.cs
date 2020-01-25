using System.Collections.Generic;

namespace ERService.Infrastructure.Interfaces
{
    public interface ILicenseManager
    {
        IEnumerable<IValidationError> Errors { get; }
        ILicenseProvider LicenseProvider { get; }
        bool LicenseHasErrors { get; }

        void Load();
    }
}