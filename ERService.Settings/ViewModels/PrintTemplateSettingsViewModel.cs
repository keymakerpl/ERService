using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.HtmlEditor.Data.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class PrintTemplateSettingsViewModel : DetailViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IPrintTemplateRepository _templateRepository;

        public ObservableCollection<PrintTemplate> Templates { get; }

        public PrintTemplateSettingsViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService, 
            IRegionManager regionManager, IPrintTemplateRepository templateRepository) 
            : base(eventAggregator, messageDialogService)
        {
            Title = "Ustawienia szablonów";

            _regionManager = regionManager;
            _templateRepository = templateRepository;

            Templates = new ObservableCollection<PrintTemplate>();

            EditTemplateCommand = new DelegateCommand(OnEditTemplateExecute);
        }

        #region Events
        private void OnEditTemplateExecute()
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.PrintTemplateEditorView);
        }
        #endregion

        #region Navigation
        public override bool KeepAlive => true;

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Load();
        }
        #endregion

        #region Load
        public override async void Load()
        {
            Templates.Clear();

            var templates = await _templateRepository.GetAllAsync();
            Templates.AddRange(templates);
        }
        #endregion

        public DelegateCommand EditTemplateCommand { get; }
    }
}
