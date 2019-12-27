using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using ERService.OrderModule.Data.Repository;
using ERService.Settings.Wrapper;
using Prism.Events;
using Prism.Regions;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class NumerationSettingsViewModel : DetailViewModelBase
    {
        private NumerationWrapper _numeration;
        private INumerationRepository _repository;
        public NumerationSettingsViewModel(IEventAggregator eventAggregator, INumerationRepository numerationRepository,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            _repository = numerationRepository;
            Title = "Schemat numeracji";
        }

        public NumerationWrapper Numeration
        {
            get { return _numeration; }
            set { SetProperty(ref _numeration, value); }
        }

        public override async Task LoadAsync()
        {
            await LoadNumeration();
        }

        private async Task LoadNumeration()
        {
            var numerations = await _repository.GetAllAsync();
            var defaultNumeration = numerations.FirstOrDefault(n => n.Name == "default");

            if (defaultNumeration != null)
                Numeration = new NumerationWrapper(defaultNumeration);
        }

        #region Events and Event Handlers

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        protected override async void OnSaveExecute()
        {
            await _repository.SaveAsync();
        }

        private void Numeration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
                HasChanges = _repository.HasChanges();
        }
        #endregion Events and Event Handlers

        #region Navigation

        public override bool KeepAlive => true;

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }
        
        #endregion Navigation
    }
}