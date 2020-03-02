using ERService.Business;
using ERService.HardwareModule.Data.Repository;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.HardwareModule.ViewModels
{
    public class DisblayableCustomItem : BindableBase
    {
        private CustomItem _customItem;
        private HwCustomItem _hwCustomItem;
        public CustomItem CustomItem { get { return _customItem; } set { SetProperty(ref _customItem, value); } }
        public HwCustomItem HwCustomItem { get { return _hwCustomItem; } set { SetProperty(ref _hwCustomItem, value); } }
    }

    public class HardwareViewModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public ObservableCollection<HwCustomItem> HardwareCustomItems;
        private ObservableCollection<DisblayableCustomItem> _displayableCustomItems;
        private ObservableCollection<HardwareType> _hardwareTypes;
        private Customer _customer;
        private HardwareType _selectedHardwareType;
        private HardwareWrapper _hardware;
        private IRegionManager _regionManager;
        private readonly IHwCustomItemRepository _hwCustomItemRepository;
        private IHardwareRepository _hardwareRepository;
        private ICustomItemRepository _customItemRepository;
        private IHardwareTypeRepository _typeRepository;
        private IRegionNavigationService _navigationService;
        private bool _wizardMode;

        public HardwareViewModel(IHardwareRepository hardwareRepository, IHardwareTypeRepository typeRepository, ICustomItemRepository customItemRepository,
            IRegionManager regionManager, IEventAggregator eventAggregator, IMessageDialogService messageDialogService, IHwCustomItemRepository hwCustomItemRepository) : base(eventAggregator, messageDialogService)
        {
            _regionManager = regionManager;
            _hwCustomItemRepository = hwCustomItemRepository;
            _hardwareRepository = hardwareRepository;
            _typeRepository = typeRepository;
            _customItemRepository = customItemRepository;

            HardwareCustomItems = new ObservableCollection<HwCustomItem>();
            HardwareTypes = new ObservableCollection<HardwareType>();
            DisplayableCustomItems = new ObservableCollection<DisblayableCustomItem>();            

            GoBackCommand = new DelegateCommand(OnGoBackExecute);
            GoForwardCommand = new DelegateCommand(OnGoForwardExecute, OnGoForwardCanExecute);
        }

        public Customer Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }

        public ObservableCollection<DisblayableCustomItem> DisplayableCustomItems
        {
            get => _displayableCustomItems;
            set
            {
                SetProperty(ref _displayableCustomItems, value);
            }
        }

        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand GoForwardCommand { get; private set; }
        public HardwareWrapper Hardware { get => _hardware; set { SetProperty(ref _hardware, value); } }
        public ObservableCollection<HardwareType> HardwareTypes { get => _hardwareTypes; set { SetProperty(ref _hardwareTypes, value); } }
        public HardwareType SelectedHardwareType
        {
            get => _selectedHardwareType;
            set
            {
                SetProperty(ref _selectedHardwareType, value);
                Hardware.Model.HardwareTypeID = value?.Id;
                GoForwardCommand.CanExecute();
            }
        }

        public bool WizardMode { get => _wizardMode; set { SetProperty(ref _wizardMode, value); } }

        public override async Task LoadAsync(Guid id)
        {
            var hardware = id != Guid.Empty ? await _hardwareRepository.GetByIdAsync(id) : GetNewDetail();

            //TODO: Można to przypisanie id zrobić niżej w bazowych?
            ID = id;

            await LoadHardwareTypes();
            await InitializeHardware(hardware);
            await LoadHardwareCustomItemsAsync();

            PropertyChanged += async (s, a) =>
            {
                if (a.PropertyName == nameof(SelectedHardwareType))
                {
                    await LoadHardwareCustomItemsAsync();
                }
            };
        }

        protected override void OnCancelEditExecute()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
        }

        protected override bool OnSaveCanExecute()
        {
            return !WizardMode;
        }

        protected override void OnSaveExecute()
        {
        }

        private Hardware GetNewDetail()
        {
            var Hardware = new Hardware();
            _hardwareRepository.Add(Hardware);

            return Hardware;
        }

        private async Task InitializeHardware(Hardware hardware)
        {            
            Hardware = new HardwareWrapper(hardware);

            Hardware.PropertyChanged += (s,a) => 
            {
                SaveCommand.RaiseCanExecuteChanged();
                GoForwardCommand.RaiseCanExecuteChanged();
            };

            if (hardware.HardwareType != null)
            {
                SelectedHardwareType = HardwareTypes.SingleOrDefault(ht => ht.Id == hardware.HardwareType.Id);
            }

            HardwareCustomItems.Clear();
            var hwCustomItems = await _hwCustomItemRepository.FindByAsync(i => i.HardwareId == hardware.Id);
            foreach (var item in hwCustomItems)
            {
                HardwareCustomItems.Add(item);
            }
        }
        
        private async Task LoadHardwareCustomItemsAsync()
        {
            if (SelectedHardwareType == null) return;
            
            DisplayableCustomItems.Clear();

            try
            {
                var items = await _customItemRepository
                                                        .FindByAsync(i => i.HardwareTypeId == SelectedHardwareType.Id);

                foreach (var item in items)
                {
                    if (HardwareCustomItems.Any(i => i.CustomItemId == item.Id))
                        continue;

                    HardwareCustomItems
                                        .Add(new HwCustomItem
                                                        {
                                                            CustomItemId = item.Id, Hardware = Hardware.Model, Value = ""
                                                        });
                }

                var query = from hci in HardwareCustomItems
                            from ci in items
                            where hci.CustomItemId == ci.Id
                            select new DisblayableCustomItem { HwCustomItem = hci, CustomItem = ci };

                var displayableItems = query.ToList();

                foreach (var item in displayableItems)
                {
                    if (!DisplayableCustomItems.Contains(item))
                        DisplayableCustomItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                _logger.Error(ex);
            }
        }

        private async Task LoadHardwareTypes()
        {
            HardwareTypes.Clear();

            var types = await _typeRepository.GetAllAsync();
            if (types != null)
            {
                foreach (var type in types)
                {
                    HardwareTypes.Add(type);
                }
            }                
        }

        private void OnGoBackExecute()
        {
            _navigationService.Journal.GoBack();
        }

        private bool OnGoForwardCanExecute()
        {
            return WizardMode && Hardware != null && !String.IsNullOrWhiteSpace(Hardware.Name) && SelectedHardwareType != null;
        }

        private void OnGoForwardExecute()
        {
            Hardware.Model.HardwareCustomItems.Clear();
            foreach (var item in DisplayableCustomItems)
            {
                Hardware.Model.HardwareCustomItems.Add(item.HwCustomItem);
            }

            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("Wizard", true);
            parameters.Add("Customer", Customer);
            parameters.Add("Hardware", Hardware.Model);

            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.OrderView, parameters);
        }

        #region Navigation

        public override bool KeepAlive
        {
            get
            {
                return false;
            }
        }

        public override void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (WizardMode)
            {
                AllowLoadAsync = false;
            }
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            var id = navigationContext.Parameters.GetValue<Guid>("ID");
            var customer = navigationContext.Parameters.GetValue<Customer>("Customer");
            WizardMode = navigationContext.Parameters.GetValue<bool>("Wizard");

            if (WizardMode)
            {
                Customer = customer;
                if (Hardware == null)
                {
                    await LoadAsync(Guid.Empty);
                }
            }
            else
            {                
                await LoadAsync(id);
            }
        }

        #endregion Navigation
    }
}