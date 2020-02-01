using ERService.Application.Views;
using ERService.CustomerModule.Repository;
using ERService.CustomerModule.Views;
using ERService.HardwareModule.Data.Repository;
using ERService.HardwareModule.Views;
using ERService.Header;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Helpers;
using ERService.Navigation;
using ERService.OrderModule.Data.Repository;
using ERService.OrderModule.Repository;
using ERService.OrderModule.Views;
using ERService.RBAC;
using ERService.RBAC.Data.Repository;
using ERService.Settings;
using ERService.Settings.Views;
using ERService.StartPage;
using ERService.StartPage.Views;
using ERService.StatusBar;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Windows;
using System.Windows.Threading;
using static ERService.RBAC.Data.Repository.RBACRepository;
using MahApps.Metro.Controls.Dialogs;
using ERService.Infrastructure.Dialogs;
using ERService.TemplateEditor;
using ERService.Licensing;
using System.Threading;
using System.Globalization;
using System.Windows.Markup;
using ERService.MSSQLDataAccess;
using ERService.Infrastructure.Base.Common;
using System.IO;
using System.Text;

namespace ERService.Application
{
    public partial class App
    {
        protected override Window CreateShell()
        {            
            return Container.Resolve<Shell>();
        }        

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            base.OnStartup(e);

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("pl-PL");
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("pl-PL");

            FrameworkElement.LanguageProperty.OverrideMetadata(
                         typeof(FrameworkElement),
                         new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //TODO: MessageBox error handler
            MessageBox.Show("Ups... " + Environment.NewLine +
                Environment.NewLine + e.Exception.Message);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule(typeof(MSSQLDataAccessModule));
            moduleCatalog.AddModule(typeof(LicensingModule));
            moduleCatalog.AddModule(typeof(NavigationModule));
            moduleCatalog.AddModule(typeof(HeaderModule));
            moduleCatalog.AddModule(typeof(StatusBarModule));
            moduleCatalog.AddModule(typeof(SettingsModule));
            moduleCatalog.AddModule(typeof(StartPageModule));
            moduleCatalog.AddModule(typeof(RBACModule));
            moduleCatalog.AddModule(typeof(TemplateEditorModule));

            //TODO: Assembly names refactor
            moduleCatalog.AddModule(typeof(OrderModule.OrderModule));
            moduleCatalog.AddModule(typeof(CustomerModule.CustomerModule));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //TODO: Czy możemy przenieść rejestrację typów do modułów tak aby było jak najmniej zależności w solucji?
            containerRegistry.RegisterSingleton<IConfig, Config>();
            containerRegistry.Register<IUserRepository, UserRepository>();
            containerRegistry.Register<IRoleRepository, RoleRepository>();
            containerRegistry.Register<IACLVerbCollection, ACLVerbCollection>();
            containerRegistry.Register<IAclVerbRepository, AclVerbRepository>();
            containerRegistry.Register<IAclRepository, AclRepository>();
            containerRegistry.RegisterSingleton<IRBACManager, RBACManager>();            
            containerRegistry.Register<ICustomerRepository, CustomerRepository>();
            containerRegistry.Register<IOrderRepository, OrderRepository>();
            containerRegistry.Register<IHardwareRepository, HardwareRepository>();
            containerRegistry.Register<IHardwareTypeRepository, HardwareTypeRepository>();
            containerRegistry.Register<ICustomItemRepository, CustomItemRepository>();
            containerRegistry.Register<IOrderStatusRepository, OrderStatusRepository>();
            containerRegistry.Register<IOrderTypeRepository, OrderTypeRepository>();
            containerRegistry.Register<IBlobRepository, BlobRepository>();
            containerRegistry.Register<INumerationRepository, NumerationRepository>();
            containerRegistry.Register<IPasswordHasher, PasswordHasher>();
            containerRegistry.Register<IDialogCoordinator, DialogCoordinator>();
            containerRegistry.Register<IMessageDialogService, MessageDialogService>();

            containerRegistry.RegisterForNavigation<CustomerView>(ViewNames.CustomerView);
            containerRegistry.RegisterForNavigation<CustomerListView>(ViewNames.CustomerListView);
            containerRegistry.RegisterForNavigation<HardwareView>(ViewNames.HardwareView);            
            containerRegistry.RegisterForNavigation<OrderView>(ViewNames.OrderView);
            containerRegistry.RegisterForNavigation<OrderListView>(ViewNames.OrderListView);
            containerRegistry.RegisterForNavigation<SettingsView>(ViewNames.SettingsView);
            containerRegistry.RegisterForNavigation<StartPageView>(ViewNames.StartPageView);            
        }
    }
}
