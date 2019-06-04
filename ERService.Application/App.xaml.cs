using ERService.Application.Views;
using ERService.Header;
using ERService.Navigation;
using ERService.Settings;
using ERService.StartPage;
using ERService.StatusBar;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace ERService.Application
{
    public partial class App
    {
        //TODO: App unexpected error handler
        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);            
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            
            moduleCatalog.AddModule(typeof(NavigationModule));
            moduleCatalog.AddModule(typeof(HeaderModule));
            moduleCatalog.AddModule(typeof(StatusBarModule));
            moduleCatalog.AddModule(typeof(SettingsModule));
            moduleCatalog.AddModule(typeof(StartPageModule));

            //TODO: Assembly names refactor
            moduleCatalog.AddModule(typeof(OrderModule.OrderModule));
            moduleCatalog.AddModule(typeof(CustomerModule.CustomerModule));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
