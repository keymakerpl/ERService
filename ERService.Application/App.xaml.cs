using ERService.Application.Views;
using ERService.Header;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Helpers;
using ERService.Navigation;
using ERService.RBAC;
using ERService.Settings;
using ERService.Settings.Views;
using ERService.StartPage;
using ERService.StatusBar;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using ERService.Infrastructure.Dialogs;
using ERService.TemplateEditor;
using ERService.Licensing;
using System.Threading;
using System.Globalization;
using System.Windows.Markup;
using ERService.MSSQLDataAccess;
using ERService.Infrastructure.Base.Common;
using ERService.Views;
using ERService.Notification;
using Prism.Regions;
using ERService.Infrastructure.Prism.Regions;
using System.Windows.Controls;
using ERService.Services;
using ERService.Infrastructure.Notifications.ToastNotifications;
using ERService.Services.Tasks;
using ERService.Services.Services;
using ERService.Statistics;

namespace ERService.Application
{
    public partial class App
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        protected override Window CreateShell()
        {            
            return Container.Resolve<Shell>();
        }        

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Fatal(e.Exception);

            //TODO: MessageBox error handler
            MessageBox.Show("Ups... " + Environment.NewLine +
                Environment.NewLine + e.Exception.Message);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            NLog.LogManager.Shutdown();

            base.OnExit(e);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule(typeof(MSSQLDataAccessModule));
            moduleCatalog.AddModule<ServicesModule>(ModuleNames.ServicesModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<NotificationModule>(ModuleNames.NotificationModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<StatisticsModule>(ModuleNames.StatisticsModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule(typeof(LicensingModule));
            moduleCatalog.AddModule(typeof(NavigationModule));
            moduleCatalog.AddModule(typeof(HeaderModule));
            moduleCatalog.AddModule(typeof(StatusBarModule));
            moduleCatalog.AddModule(typeof(SettingsModule));
            moduleCatalog.AddModule(typeof(StartPageModule));
            moduleCatalog.AddModule(typeof(RBACModule));
            moduleCatalog.AddModule(typeof(TemplateEditorModule));
            moduleCatalog.AddModule(typeof(HardwareModule.HardwareModule));
            moduleCatalog.AddModule(typeof(OrderModule.OrderModule));
            moduleCatalog.AddModule(typeof(CustomerModule.CustomerModule));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry   .RegisterSingleton<IBackgroundTaskRegistration, BackgroundTaskRegistration>()
                                .RegisterSingleton<IConfig, Config>()                                                                                                                                            
                                .Register<IPasswordHasher, PasswordHasher>()
                                .Register<IDialogCoordinator, DialogCoordinator>()
                                .Register<IMessageDialogService, MessageDialogService>()
                                .Register<IToastNotificationService, ToastNotificationService>();            

            containerRegistry.RegisterForNavigation<LoggedUserView>(ViewNames.LoggedUserView); 
            containerRegistry.RegisterForNavigation<SettingsView>(ViewNames.SettingsView);                
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            regionAdapterMappings.RegisterMapping(typeof(StackPanel), Container.Resolve<StackPanelRegionAdapter>());
        }
    }
}
