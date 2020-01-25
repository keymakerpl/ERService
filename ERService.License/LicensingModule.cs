using ERService.Infrastructure.Interfaces;
using ERService.Licensing.Manager;
using ERService.Licensing.Providers;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.Licensing
{
    public class LicensingModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve(typeof(LicenseManager));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ILicenseProviderFactory, PortableLicenseFactory>();
            containerRegistry.RegisterSingleton<ILicenseManager, LicenseManager>();
        }
    }
}
