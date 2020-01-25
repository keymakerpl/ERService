using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.TemplateEditor.Data.Repository;
using ERService.TemplateEditor.Interpreter;
using ERService.TemplateEditor.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ERService.TemplateEditor.ViewModels
{
    public class IndexLookupItem
    {
        public string DisplayableName { get; set; }

        public string IndexPattern { get; set; }
    }

    public class PrintTemplateEditorViewModel : DetailViewModelBase
    {
        private readonly IPrintTemplateRepository _templeteRepository;
        private readonly IRegionManager _regionManager;
        private readonly IInterpreter _interpreter;
        private string _patternToInsert;

        public PrintTemplateEditorViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            IPrintTemplateRepository templeteRepository, IRegionManager regionManager, IInterpreter interpreter) : base(eventAggregator, messageDialogService)
        {
            _templeteRepository = templeteRepository;
            _regionManager = regionManager;
            _interpreter = interpreter;

            Indexes = new ObservableCollection<IndexLookupItem>();
            PrintTemplates = new ObservableCollection<PrintTemplate>();

            AddIndexToEditorCommand = new DelegateCommand<object>(OnAddIndexExecute);
            SelectTemplateCommand = new DelegateCommand<object>(OnSelectTemplateExecute);

            PrintCommand = new DelegateCommand(OnPrintExecute);
        }

        private void OnPrintExecute()
        {
            _eventAggregator.GetEvent<PrintEvent>().Publish();
        }

        private void OnSelectTemplateExecute(object arg)
        {
            var template = arg as PrintTemplate;
            if (template != null)
            {
                InitializeTemplate(template);
            }
        }

        private PrintTemplateWrapper _printTemplate;

        public PrintTemplateWrapper PrintTemplate
        {
            get { return _printTemplate; }
            set { SetProperty(ref _printTemplate, value); }
        }

        public override bool KeepAlive => false;

        public DelegateCommand<object> AddIndexToEditorCommand { get; private set; }
        public DelegateCommand<object> SelectTemplateCommand { get; private set; }
        public DelegateCommand PrintCommand { get; private set; }
        public ObservableCollection<IndexLookupItem> Indexes { get; }
        public ObservableCollection<PrintTemplate> PrintTemplates { get; }

        public string PatternToInsert { get { return _patternToInsert; } set { SetProperty(ref _patternToInsert, value); } }
        public IndexLookupItem SelectedIndex { get; set; }

        public override async Task LoadAsync(Guid id)
        {
            var template = id != Guid.Empty ? await _templeteRepository.GetByIdAsync(id) : GetNewDetail();

            await LoadIndexList();
            InitializeTemplate(template);
        }

        private async void InitializeTemplate(PrintTemplate template)
        {            
            PrintTemplate = new PrintTemplateWrapper(template);

            if (ModelWrappers != null)
            {
                _interpreter.Context = new Context(template.Template);
                _interpreter.Expression = Expression.IndexExpression;
                _interpreter.Wrappers = ModelWrappers;

                var intepretedTemplate = _interpreter.GetInterpretedContext();

                PrintTemplate.Template = intepretedTemplate.Output;
                IsNavigationBarVisible = true;

                PrintTemplates.Clear();
                var templates = await _templeteRepository.GetAllAsync();
                foreach (var temp in templates)
                {
                    PrintTemplates.Add(temp);
                }
            }

            PrintTemplate.PropertyChanged += (s, a) =>
            {
                HasChanges = _templeteRepository.HasChanges();
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        } 

        private PrintTemplate GetNewDetail()
        {
            var template = new PrintTemplate();
            _templeteRepository.Add(template);

            return template;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigatonContext = navigationContext;

            var id = navigationContext.Parameters.GetValue<Guid>("ID");
            IsReadOnly = navigationContext.Parameters.GetValue<bool>("IsReadOnly");
            IsToolbarVisible = navigationContext.Parameters.GetValue<bool>("IsToolbarVisible");
            ModelWrappers = navigationContext.Parameters.GetValue<object[]>("ModelWrappers");

            await LoadAsync(id);           
        }

        private async Task LoadIndexList()
        {
            if (IsReadOnly) return;

            Indexes.Clear();
            IEnumerable<Index> indexes = await _interpreter.GetIndexesAsync();
            foreach (var indx in indexes)
            {
                Indexes.Add(new IndexLookupItem { DisplayableName = indx.Name, IndexPattern = indx.Pattern });
            }
        }

        private void OnAddIndexExecute(object patternString)
        {
            PatternToInsert = patternString as string ?? "";
            PatternToInsert = "";
        }

        protected override async void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_templeteRepository.SaveAsync, 
                () => 
                {
                    HasChanges = _templeteRepository.HasChanges();
                    ID = PrintTemplate.Model.Id;
                    _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
                    _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.SettingsView);
                });
        }

        protected override void OnCancelEditExecute()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.StartPageView);
        }

        protected override bool OnSaveCanExecute()
        {
            return !String.IsNullOrWhiteSpace(PrintTemplate?.Name) && HasChanges;
        }

        protected override void OnCloseDetailViewExecute()
        {
            _navigatonContext.NavigationService.Journal.GoBack();
        }

        private bool _isToolbarVisible;

        public bool IsToolbarVisible
        {
            get { return _isToolbarVisible; }
            set { SetProperty(ref _isToolbarVisible, value); }
        }

        private bool _isNavigationBarVisible;

        public bool IsNavigationBarVisible
        {
            get { return _isNavigationBarVisible; }
            set { SetProperty(ref _isNavigationBarVisible, value); }
        }

        private object[] ModelWrappers;
        private NavigationContext _navigatonContext;
    }
}