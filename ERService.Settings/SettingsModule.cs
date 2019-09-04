using ERService.Infrastructure.Constants;
using ERService.Infrastructure.HtmlEditor.Data.Repository;
using ERService.Settings.Views;
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

            containerRegistry.RegisterForNavigation<GeneralSettingsView>(ViewNames.GeneralSettingsView);
            containerRegistry.RegisterForNavigation<HardwareTypesView>(ViewNames.HardwareTypesView);
            containerRegistry.RegisterForNavigation<StatusConfigView>(ViewNames.StatusConfigView);
            containerRegistry.RegisterForNavigation<NumerationSettingsView>(ViewNames.NumerationSettingsView);
            containerRegistry.RegisterForNavigation<UsersSettingsView>(ViewNames.UserSettingsView);
            containerRegistry.RegisterForNavigation<UserDetailView>(ViewNames.UserDetailView);
            containerRegistry.RegisterForNavigation<PrintTemplateSettingsView>(ViewNames.PrintTemplateSettingsView);
            containerRegistry.RegisterForNavigation<PrintTemplateEditorView>(ViewNames.PrintTemplateEditorView);
        }
    }
}