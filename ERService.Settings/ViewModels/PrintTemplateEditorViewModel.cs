using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.HtmlEditor.Data.Repository;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class PrintTemplateEditorViewModel : DetailViewModelBase
    {
        private readonly IPrintTemplateRepository _templeteRepository;

        public PrintTemplateEditorViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService, IPrintTemplateRepository templeteRepository) 
            : base(eventAggregator, messageDialogService)
        {
            _templeteRepository = templeteRepository;
        }
    }
}
