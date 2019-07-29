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
    public class NumerationSettingsViewModel : DetailViewModelBase, INavigationAware, IRegionMemberLifetime
    {
        private INumerationRepository _repository;

        private NumerationWrapper _numeration;        

        public NumerationWrapper Numeration
        {
            get { return _numeration; }
            set { SetProperty(ref _numeration, value); }
        }

        public bool KeepAlive => false;

        public NumerationSettingsViewModel(IEventAggregator eventAggregator, INumerationRepository numerationRepository) : base(eventAggregator)
        {
            _repository = numerationRepository;
            Title = "Schemat numeracji";
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override Task LoadAsync(Guid id)
        {
            return null;
        }

        public override async Task LoadAsync()
        {
            var numerations = await _repository.FindByAsync(n => n.Name == "default");

            Numeration = new NumerationWrapper(numerations.First());
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

        protected override bool OnCancelEditCanExecute()
        {
            return true;
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
