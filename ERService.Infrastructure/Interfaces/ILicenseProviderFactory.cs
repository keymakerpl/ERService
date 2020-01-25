
using System.IO;

namespace ERService.Infrastructure.Interfaces
{
    public interface ILicenseProviderFactory
    {
        ILicenseProvider GetLicenseProvider(Stream stream);
    }
}
