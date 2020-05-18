using ERService.Application.Views;
using ERService.Header;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Helpers;
using ERService.Navigation;
using ERService.RBAC;
using ERService.Settings;
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
using System.Globalization;
using System.Windows.Markup;
using ERService.MSSQLDataAccess;
using ERService.Infrastructure.Base.Common;
using ERService.Notification;
using Prism.Regions;
using ERService.Infrastructure.Prism.Regions;
using System.Windows.Controls;
using ERService.Services;
using ERService.Infrastructure.Notifications.ToastNotifications;
using ERService.Services.Tasks;
using ERService.Services.Services;
using ERService.Statistics;
using Hangfire.Server;
using ERService.Startup;
using ERService.Views;

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
            InitializeCultures();
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            base.OnStartup(e);
        }

        private static void InitializeCultures()
        {
            //TODO: Make multilanguage
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pl-PL");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pl-PL");

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
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
            //var jobServer = Container.Resolve<IBackgroundProcessingServer>();
            //jobServer.SendStop();
            //jobServer.Dispose();

            base.OnExit(e);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule<MSSQLDataAccessModule>(ModuleNames.SQLDataAccessModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<RBACModule>(ModuleNames.RBACModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<ServicesModule>(ModuleNames.ServicesModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<NotificationModule>(ModuleNames.NotificationModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<StatisticsModule>(ModuleNames.StatisticsModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<LicensingModule>(ModuleNames.LicensingModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<NavigationModule>(ModuleNames.NavigationModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<HeaderModule>(ModuleNames.HeaderModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<StatusBarModule>(ModuleNames.StatusBarModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<SettingsModule>(ModuleNames.SettingsModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<StartPageModule>(ModuleNames.StartPageModule, InitializationMode.OnDemand);            
            moduleCatalog.AddModule<TemplateEditorModule>(ModuleNames.TemplateEditorModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<HardwareModule.HardwareModule>(ModuleNames.HardwareModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<OrderModule.OrderModule>(ModuleNames.OrderModule, InitializationMode.OnDemand);
            moduleCatalog.AddModule<CustomerModule.CustomerModule>(ModuleNames.CustomerModule, InitializationMode.OnDemand);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry   .RegisterSingleton<IBackgroundTaskRegistration, BackgroundTaskRegistration>()
                                .RegisterSingleton<IConfig, Config>()
                                .Register<IERBootstrap, ERBootstrap>()
                                .Register<IPasswordHasher, PasswordHasher>()
                                .Register<IDialogCoordinator, DialogCoordinator>()
                                .Register<IMessageDialogService, MessageDialogService>()
                                .Register<IToastNotificationService, ToastNotificationService>();

            containerRegistry.RegisterForNavigation<LoginView>(ViewNames.LoginView);
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            regionAdapterMappings.RegisterMapping(typeof(StackPanel), Container.Resolve<StackPanelRegionAdapter>());
        }
    }
}
