using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class PrintTemplateSettingsViewModel : DetailViewModelBase
    {
        public PrintTemplateSettingsViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            Title = "Ustawienia szablonów";
        }

        public override bool KeepAlive => true;
    }
}
