using ERService.Infrastructure.Constants;
using Prism.Modularity;

namespace ERService.Startup
{
    public class ERBootstrap : IERBootstrap
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IModuleManager _moduleManager;

        public ERBootstrap(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;

            _moduleManager.LoadModuleCompleted += _moduleManager_LoadModuleCompleted;
        }

        private void _moduleManager_LoadModuleCompleted(object sender, LoadModuleCompletedEventArgs e)
        {
            _logger.Debug($"Load module completed: {e.ModuleInfo.ModuleName} | {e.ModuleInfo.State}");
            if (e.Error != null)
            {
                _logger.Debug(e.Error);
                _logger.Error(e.Error);
            }
        }

        public void ColdStart()
        {
            _moduleManager.LoadModule(ModuleNames.SQLDataAccessModule);
            _moduleManager.LoadModule(ModuleNames.RBACModule);
        }

        public void HotStart()
        {
            _moduleManager.LoadModule(ModuleNames.NavigationModule);
            _moduleManager.LoadModule(ModuleNames.SettingsModule);
            _moduleManager.LoadModule(ModuleNames.ServicesModule);
            _moduleManager.LoadModule(ModuleNames.LicensingModule);
            _moduleManager.LoadModule(ModuleNames.NotificationModule);
            _moduleManager.LoadModule(ModuleNames.HardwareModule);
            _moduleManager.LoadModule(ModuleNames.OrderModule);
            _moduleManager.LoadModule(ModuleNames.CustomerModule);
            _moduleManager.LoadModule(ModuleNames.StatusBarModule);
            _moduleManager.LoadModule(ModuleNames.StatisticsModule);
            _moduleManager.LoadModule(ModuleNames.StartPageModule);
            _moduleManager.LoadModule(ModuleNames.TemplateEditorModule);
            _moduleManager.LoadModule(ModuleNames.HeaderModule);
        }
    }
}
