using ERService.Infrastructure.Base;
using ERService.OrderModule.Data.Repository;
using ERService.Settings.Wrapper;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class NumerationSettingsViewModel : DetailViewModelBase, INavigationAware
    {
        private INumerationRepository _repository;

        private NumerationWrapper _numeration;        

        public NumerationWrapper Numeration
        {
            get { return _numeration; }
            set { SetProperty(ref _numeration, value); }
        }

        public NumerationSettingsViewModel(IEventAggregator eventAggregator, INumerationRepository numerationRepository) : base(eventAggregator)
        {
            _repository = numerationRepository;
            Title = "Schemat numeracji";
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public override Task LoadAsync(Guid id)
        {
            return null;
        }

        public override async Task LoadAsync()
        {
            await LoadNumeration();
        }

        private async Task LoadNumeration()
        {
            var numerations = await _repository.GetAllAsync();
            var defaultNumeration = numerations.FirstOrDefault(n => n.Name == "default");

            if(defaultNumeration != null)
                Numeration = new NumerationWrapper(defaultNumeration);
        }

        private void Numeration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(!HasChanges)
                HasChanges = _repository.HasChanges();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync();
        }

        protected override void OnCancelEditExecute()
        {
            
        }

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        protected override async void OnSaveExecute()
        {
            _repository.SetEntityStatus(Numeration.Model, EntityState.Modified);
            await _repository.SaveAsync();
        }
    }
}
