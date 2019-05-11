using ERService.Application.Views;
using ERService.Header;
using ERService.Navigation;
using ERService.Settings;
using ERService.Settings.ViewModels;
using ERService.Settings.Views;
using ERService.StartPage;
using ERService.StatusBar;
using ERService.Toolbar;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Globalization;
using System.Reflection;
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

            //ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            //{
            //    var viewName = viewType.FullName;
            //    var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            //    var viewModelName = String.Format(CultureInfo.InvariantCulture
            //        , "{0}ViewModel, {1}", viewName, viewAssemblyName);

            //    return Type.GetType(viewModelName);
            //});
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            
            moduleCatalog.AddModule(typeof(NavigationModule));
            moduleCatalog.AddModule(typeof(HeaderModule));
            moduleCatalog.AddModule(typeof(StatusBarModule));
            moduleCatalog.AddModule(typeof(ToolbarModule));
            moduleCatalog.AddModule(typeof(SettingsModule), InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(StartPageModule));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {            
            containerRegistry.Register<SettingsViewModel, SettingsViewModel>();
        }
    }
}
