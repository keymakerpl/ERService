using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ERService.HardwareModule.Data.Repository;
using ERService.HardwareModule.Wrapper;
using ERService.Infrastructure.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Linq;
using System.ComponentModel;
using ERService.Business;
using ERService.Infrastructure.Events;

namespace ERService.Settings.ViewModels
{
    public class HardwareTypesViewModel : DetailViewModelBase, INavigationAware
    {
        public CustomItemWrapper SelectedCustomItem
        {
            get {return _selectedCustomItem; }
            set
            {
                SetProperty(ref _selectedCustomItem, value);
                RemoveCustomItemCommand.RaiseCanExecuteChanged();
            }
        }
        public HardwareTypeWrapper SelectedHardwareType
        {
            get { return _selectedHardwareType; }
            set
            {
                SetProperty(ref _selectedHardwareType, value);
                RemoveCommand.RaiseCanExecuteChanged();
                AddCustomItemCommand.RaiseCanExecuteChanged();
            }
        }
        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand RemoveCommand { get; private set; }
        public DelegateCommand AddCustomItemCommand { get; private set; }
        public DelegateCommand RemoveCustomItemCommand { get; private set; }
        public ObservableCollection<HardwareTypeWrapper> HardwareTypes { get; set; }
        public ObservableCollection<CustomItemWrapper> CustomItems { get; set; }

        private IHardwareTypeRepository _hardwareTypeRepository;
        private ICustomItemRepository _customItemRepository;
        private HardwareTypeWrapper _selectedHardwareType;
        private CustomItemWrapper _selectedCustomItem;

        public HardwareTypesViewModel(IEventAggregator eventAggregator, 
            IHardwareTypeRepository hardwareTypeRepository, ICustomItemRepository customItemRepository) 
            : base(eventAggregator)
        {
            Title = "Typy urządzeń";

            _hardwareTypeRepository = hardwareTypeRepository;
            _customItemRepository = customItemRepository;

            HardwareTypes = new ObservableCollection<HardwareTypeWrapper>();
            CustomItems = new ObservableCollection<CustomItemWrapper>();

            AddCommand = new DelegateCommand(OnAddExecute);
            RemoveCommand = new DelegateCommand(OnRemoveExecute, OnRemoveCanExecute);

            AddCustomItemCommand = new DelegateCommand(OnAddCustomItemExecute, OnAddCustomItemCanExecute);
            RemoveCustomItemCommand = new DelegateCommand(OnRemoveCustomItemExecute, OnCanRemoveCustomItemExecute);

            InitializeEvents();
        }

        private bool OnAddCustomItemCanExecute()
        {
            return SelectedHardwareType != null;
        }

        private void InitializeEvents()
        {
            PropertyChanged += HardwareTypesViewModel_PropertyChangedAsync;
        }

        private async void HardwareTypesViewModel_PropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedHardwareType")
            {
                await LoadCustomItems();
            }
        }

        private bool OnCanRemoveCustomItemExecute()
        {
            return SelectedCustomItem != null;
        }

        private void OnRemoveCustomItemExecute()
        {
            SelectedCustomItem.PropertyChanged -= WrappedCustomItem_PropertyChanged;
            _customItemRepository.Remove(SelectedCustomItem.Model);
            CustomItems.Remove(_selectedCustomItem);
            SelectedCustomItem = null;
            HasChanges = _customItemRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddCustomItemExecute()
        {
            var wrappedCustomItem = new CustomItemWrapper(new CustomItem() { HardwareTypeId = SelectedHardwareType.Model.Id });
            wrappedCustomItem.PropertyChanged += WrappedCustomItem_PropertyChanged;
            _customItemRepository.Add(wrappedCustomItem.Model);
            CustomItems.Add(wrappedCustomItem);

            wrappedCustomItem.Key = "";
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedHardwareType != null;
        }

        private void OnRemoveExecute()
        {
            
        }

        private void OnAddExecute()
        {
            var wrappedHardwareType = new HardwareTypeWrapper(new HardwareType());
            wrappedHardwareType.PropertyChanged += WrappedHardwareType_PropertyChanged;
            _hardwareTypeRepository.Add(wrappedHardwareType.Model);
            HardwareTypes.Add(wrappedHardwareType);

            wrappedHardwareType.Name = "";
        }

        public async override Task LoadAsync(Guid id)
        {
            ID = id;

            await LoadHardwareTypes();
        }

        private async Task LoadCustomItems()
        {
            foreach (var wrappedItem in CustomItems)
            {
                wrappedItem.PropertyChanged -= WrappedCustomItem_PropertyChanged;
            }

            CustomItems.Clear();

            var customItems = await _customItemRepository.GetCustomItemsByHardwareTypeAsync(SelectedHardwareType.Id);
            foreach (var item in customItems)
            {
                var wrappedModel = new CustomItemWrapper(item);
                wrappedModel.PropertyChanged += WrappedCustomItem_PropertyChanged;

                if (CustomItems.Contains(wrappedModel)) continue;

                CustomItems.Add(wrappedModel);
            }
        }

        private void WrappedCustomItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges) //odśwerzamy z repo czy już zaszły jakieś zmiany, nie odpalamy jeśli już jest True
            {
                HasChanges = _customItemRepository.HasChanges();
            }

            if (e.PropertyName == nameof(CustomItemWrapper.HasErrors)) //sprawdzamy czy możemy sejwować
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private async Task LoadHardwareTypes()
        {
            foreach (var wrappedType in HardwareTypes)
            {
                wrappedType.PropertyChanged -= WrappedHardwareType_PropertyChanged;
            }

            HardwareTypes.Clear();

            var hardwareTypes = await _hardwareTypeRepository.GetAllAsync();
            foreach (var type in hardwareTypes)
            {
                var wrappedModel = new HardwareTypeWrapper(type);
                wrappedModel.PropertyChanged += WrappedHardwareType_PropertyChanged;
                HardwareTypes.Add(wrappedModel);
            }
        }

        private void WrappedHardwareType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges) //odśwerzamy z repo czy już zaszły jakieś zmiany, nie odpalamy jeśli już jest True
            {
                HasChanges = _hardwareTypeRepository.HasChanges();
            }

            if (e.PropertyName == nameof(HardwareTypeWrapper.HasErrors)) //sprawdzamy czy możemy sejwować
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        protected async override void OnSaveExecute()
        {
            await _hardwareTypeRepository.SaveAsync();
            await _customItemRepository.SaveAsync();

            HasChanges = _hardwareTypeRepository.HasChanges() || _customItemRepository.HasChanges();
        }

        protected override bool OnSaveCanExecute()
        {
            return (_hardwareTypeRepository.HasChanges() || _customItemRepository.HasChanges()) && SelectedHardwareType != null;
        }

        protected override void OnCancelEditExecute()
        {
            
        }

        #region Navigation

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await LoadAsync(Guid.Empty);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        #endregion
    }
}
