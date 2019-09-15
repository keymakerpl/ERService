using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.HtmlEditor.Data.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;

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

            EditTemplateCommand = new DelegateCommand(OnEditTemplateExecute, OnEditTemplateCanExecute);
            AddTemplateCommand = new DelegateCommand(OnAddTemplateExecute);
        }

        private bool OnEditTemplateCanExecute()
        {
            return SelectedTemplate != null;
        }

        private void OnAddTemplateExecute()
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.PrintTemplateEditorView);
        }

        private PrintTemplate _selectedTemplate;

        public PrintTemplate SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { SetProperty(ref _selectedTemplate, value); EditTemplateCommand.RaiseCanExecuteChanged(); }
        }

        #region Events
        private void OnEditTemplateExecute()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("ID", SelectedTemplate.Id);
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.PrintTemplateEditorView, navigationParameters);
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
        public DelegateCommand AddTemplateCommand { get; }
    }
}
