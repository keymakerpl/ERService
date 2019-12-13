
using ERService.Infrastructure.Interfaces;
using System.IO;

namespace ERService.Licensing.Providers
{

    public class PortableLicenseFactory : ILicenseProviderFactory
    {
        public ILicenseProvider GetLicenseProvider(Stream stream)
        {
            return new PortableLicenseProvider(stream);
        }
    }
}
