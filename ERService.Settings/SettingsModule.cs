using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Interfaces;
using ERService.Settings.Data.Repository;
using ERService.Settings.Manager;
using ERService.Settings.Views;
using ERService.TemplateEditor.Data.Repository;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.Settings
{
    public class SettingsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IPrintTemplateRepository, PrintTemplateRepository>();
            containerRegistry.Register<ISettingsRepository, SettingsRepository>();
            containerRegistry.Register<ISettingsManager, SettingsManager>();

            containerRegistry.RegisterForNavigation<GeneralSettingsView>(ViewNames.GeneralSettingsView);
            containerRegistry.RegisterForNavigation<HardwareTypesView>(ViewNames.HardwareTypesView);
            containerRegistry.RegisterForNavigation<StatusConfigView>(ViewNames.StatusConfigView);
            containerRegistry.RegisterForNavigation<NumerationSettingsView>(ViewNames.NumerationSettingsView);
            containerRegistry.RegisterForNavigation<UsersSettingsView>(ViewNames.UserSettingsView);
            containerRegistry.RegisterForNavigation<UserDetailView>(ViewNames.UserDetailView);
            containerRegistry.RegisterForNavigation<PrintTemplateSettingsView>(ViewNames.PrintTemplateSettingsView);
            containerRegistry.RegisterForNavigation<CompanySettingsView>(ViewNames.CompanySettingsView);
        }
    }
}