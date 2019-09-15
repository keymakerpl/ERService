using ERService.Business;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.HtmlEditor.Data.Repository;
using ERService.Infrastructure.PrintTemplateEditor.Interpreter;
using ERService.Settings.Wrapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
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
        private string _patternToInsert;
        public PrintTemplateEditorViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            IPrintTemplateRepository templeteRepository, IRegionManager regionManager) : base(eventAggregator, messageDialogService)
        {
            _templeteRepository = templeteRepository;
            _regionManager = regionManager;
            Indexes = new ObservableCollection<IndexLookupItem>();

            AddIndexToEditorCommand = new DelegateCommand<object>(OnAddIndexExecute);
        }

        private PrintTemplateWrapper _template;

        public PrintTemplateWrapper PrintTemplate
        {
            get { return _template; }
            set { SetProperty(ref _template, value); }
        }

        public DelegateCommand<object> AddIndexToEditorCommand { get; private set; }
        public ObservableCollection<IndexLookupItem> Indexes { get; }
        public string PatternToInsert { get { return _patternToInsert; } set { SetProperty(ref _patternToInsert, value); } }
        public IndexLookupItem SelectedIndex { get; set; }

        public override async Task LoadAsync(Guid id)
        {
            var template = id != Guid.Empty ? await _templeteRepository.GetByIdAsync(id) : GetNewDetail();

            InitializeTemplate(template);
            LoadIndexList();
        }

        private void InitializeTemplate(PrintTemplate template)
        {
            PrintTemplate = new PrintTemplateWrapper(template);

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
            var id = navigationContext.Parameters.GetValue<Guid>("ID");
            await LoadAsync(id);
        }

        private void LoadIndexList()
        {
            Indexes.Clear();
            foreach (var indx in IndexCollection.IndexList)
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
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.SettingsView);
        }

        protected override bool OnSaveCanExecute()
        {
            return !String.IsNullOrWhiteSpace(PrintTemplate?.Name) && HasChanges;
        }
    }
}