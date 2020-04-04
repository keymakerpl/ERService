using ERService.Business;
using ERService.HardwareModule.Data.Repository;
using ERService.HardwareModule.Wrapper;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ERService.Settings.ViewModels
{
    public class HardwareTypesViewModel : DetailViewModelBase
    {
        private ICustomItemRepository _customItemRepository;
        private IHardwareTypeRepository _hardwareTypeRepository;
        private CustomItemWrapper _selectedCustomItem;
        private HardwareTypeWrapper _selectedHardwareType;
        
        public HardwareTypesViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            IHardwareTypeRepository hardwareTypeRepository, ICustomItemRepository customItemRepository)
            : base(eventAggregator, messageDialogService)
        {
            Title = "Typy urządzeń";

            _hardwareTypeRepository = hardwareTypeRepository;
            _customItemRepository = customItemRepository;

            HardwareTypes = new ObservableCollection<HardwareTypeWrapper>();
            CustomItems = new ObservableCollection<CustomItemWrapper>();

            AddHardwareTypeCommand = new DelegateCommand(OnAddHardwareTypeExecute);
            RemoveHardwareTypeCommand = new DelegateCommand(OnRemoveHardwareTypeExecute, OnRemoveHardwareTypeCanExecute);

            AddCustomItemCommand = new DelegateCommand(OnAddCustomItemExecute, OnAddCustomItemCanExecute);
            RemoveCustomItemCommand = new DelegateCommand(OnRemoveCustomItemExecute, OnCanRemoveCustomItemExecute);

            InitializeEvents();
        }

        public DelegateCommand AddHardwareTypeCommand { get; private set; }

        public DelegateCommand AddCustomItemCommand { get; private set; }

        public ObservableCollection<CustomItemWrapper> CustomItems { get; set; }

        public ObservableCollection<HardwareTypeWrapper> HardwareTypes { get; set; }

        public DelegateCommand RemoveHardwareTypeCommand { get; private set; }

        public DelegateCommand RemoveCustomItemCommand { get; private set; }

        public CustomItemWrapper SelectedCustomItem
        {
            get { return _selectedCustomItem; }
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
                RemoveHardwareTypeCommand.RaiseCanExecuteChanged();
                AddCustomItemCommand.RaiseCanExecuteChanged();
            }
        }

        public async override Task LoadAsync()
        {
            await LoadHardwareTypes();
        }

        private void InitializeEvents()
        {
            PropertyChanged += HardwareTypesViewModel_PropertyChangedAsync;
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

        #region Events and Event Handlers

        protected override bool OnSaveCanExecute()
        {
            return (_hardwareTypeRepository.HasChanges() || _customItemRepository.HasChanges());
        }

        protected async override void OnSaveExecute()
        {
            await _hardwareTypeRepository.SaveAsync();
            await _customItemRepository.SaveAsync();

            HasChanges = _hardwareTypeRepository.HasChanges() || _customItemRepository.HasChanges();
        }

        private async void HardwareTypesViewModel_PropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {            
            if (e.PropertyName == nameof(SelectedHardwareType))
            {
                if(SelectedHardwareType == null)
                {
                    CustomItems.Clear();
                    return;
                }

                await LoadCustomItems();
            }
        }

        private bool OnAddCustomItemCanExecute()
        {
            return SelectedHardwareType != null;
        }

        private async void OnAddCustomItemExecute()
        {
            var dialogResult = await _messageDialogService.ShowInputMessageAsync(this, "Nowe pole zdefiniowane...", "Podaj nazwę nowego pola:");

            if (!String.IsNullOrWhiteSpace(dialogResult))
            {
                var wrappedCustomItem = new CustomItemWrapper(new CustomItem() { HardwareTypeId = SelectedHardwareType.Model.Id });
                wrappedCustomItem.PropertyChanged += WrappedCustomItem_PropertyChanged;
                _customItemRepository.Add(wrappedCustomItem.Model);
                CustomItems.Add(wrappedCustomItem);

                wrappedCustomItem.Key = dialogResult;
            }
        }

        private async void OnAddHardwareTypeExecute()
        {
            var dialogResult = await _messageDialogService.ShowInputMessageAsync(this, "Nowy typ naprawy...", "Podaj nazwę nowego typu:");
            if (!String.IsNullOrWhiteSpace(dialogResult))
            {
                var wrappedHardwareType = new HardwareTypeWrapper(new HardwareType());
                wrappedHardwareType.PropertyChanged += WrappedHardwareType_PropertyChanged;
                _hardwareTypeRepository.Add(wrappedHardwareType.Model);
                HardwareTypes.Add(wrappedHardwareType);

                wrappedHardwareType.Name = dialogResult;
            }                        
        }

        private bool OnCanRemoveCustomItemExecute()
        {
            return SelectedCustomItem != null;
        }

        private bool OnRemoveHardwareTypeCanExecute()
        {
            return SelectedHardwareType != null;
        }

        private void OnRemoveCustomItemExecute()
        {
            SelectedCustomItem.PropertyChanged -= WrappedCustomItem_PropertyChanged;
            _customItemRepository.Remove(SelectedCustomItem.Model);
            CustomItems.Remove(_selectedCustomItem);
            SelectedCustomItem = null;
            HasChanges = _customItemRepository.HasChanges();
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void OnRemoveHardwareTypeExecute()
        {
            SelectedHardwareType.PropertyChanged -= WrappedHardwareType_PropertyChanged;
            _hardwareTypeRepository.Remove(SelectedHardwareType.Model);
            HardwareTypes.Remove(_selectedHardwareType);
            SelectedHardwareType = null;
            HasChanges = _hardwareTypeRepository.HasChanges();
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void WrappedCustomItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges) 
            {
                HasChanges = _customItemRepository.HasChanges();
            }

            if (e.PropertyName == nameof(CustomItemWrapper.HasErrors))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void WrappedHardwareType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _hardwareTypeRepository.HasChanges();
            }

            if (e.PropertyName == nameof(HardwareTypeWrapper.HasErrors))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
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