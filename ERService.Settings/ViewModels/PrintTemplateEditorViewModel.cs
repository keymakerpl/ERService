using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.HtmlEditor.Data.Repository;
using ERService.Infrastructure.PrintTemplateEditor.Interpreter;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;

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

        private string _patternToInsert;
        private string _templateText;
        public PrintTemplateEditorViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            IPrintTemplateRepository templeteRepository) : base(eventAggregator, messageDialogService)
        {
            _templeteRepository = templeteRepository;

            Indexes = new ObservableCollection<IndexLookupItem>();

            AddIndexToEditorCommand = new DelegateCommand<object>(OnAddIndexExecute);
        }

        public DelegateCommand<object> AddIndexToEditorCommand { get; private set; }
        public ObservableCollection<IndexLookupItem> Indexes { get; }
        public string PatternToInsert { get { return _patternToInsert; } set { SetProperty(ref _patternToInsert, value); } }
        public IndexLookupItem SelectedIndex { get; set; }
        public string TemplateText
        {
            get { return _templateText; }
            set { SetProperty(ref _templateText, value); }
        }

        public override void Load()
        {
            LoadIndexList();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Load();
        }

        private void LoadIndexList()
        {
            Indexes.Clear();
            foreach (var indx in IndexCollection.IndexList)
            {
                Indexes.Add(new IndexLookupItem { DisplayableName = indx.Name, IndexPattern = indx.Pattern });
            }
        }

        private void OnAddIndexExecute(object obj)
        {
            PatternToInsert = obj as string ?? "";
            PatternToInsert = "";
        }
    }
}