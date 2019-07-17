using ERService.Business;
using ERService.HardwareModule.Data.Repository;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
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

    public class HardwareViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        public ObservableCollection<HwCustomItem> HardwareCustomItems;
        private Customer _customer;
        private ICustomItemRepository _customItemRepository;
        private ObservableCollection<DisblayableCustomItem> _displayableCustomItems;
        private HardwareWrapper _hardware;
        private ObservableCollection<HardwareType> _hardwareTypes;
        private IRegionNavigationService _navigationService;
        private IRegionManager _regionManager;
        private IHardwareRepository _repository;
        private HardwareType _selectedHardwareType;
        private IHardwareTypeRepository _typeRepository;
        private bool _wizardMode;

        public HardwareViewModel(IHardwareRepository repository, IHardwareTypeRepository typeRepository, ICustomItemRepository customItemRepository,
            IRegionManager regionManager, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            //TODO: Czy można zrobić tu refactor? Może jakiś wzorzec Fasada? Aby wrzucić repo w jedno miejsce
            _regionManager = regionManager;
            _repository = repository;
            _typeRepository = typeRepository;
            _customItemRepository = customItemRepository;

            HardwareCustomItems = new ObservableCollection<HwCustomItem>();
            HardwareTypes = new ObservableCollection<HardwareType>();
            DisplayableCustomItems = new ObservableCollection<DisblayableCustomItem>();

            PropertyChanged += HardwareViewModel_PropertyChanged;

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
        public HardwareType SelectedHardwareType { get => _selectedHardwareType; set { SetProperty(ref _selectedHardwareType, value); } }
        public bool WizardMode { get => _wizardMode; set { SetProperty(ref _wizardMode, value); } }

        public override async Task LoadAsync(Guid id)
        {
            var hardware = id != Guid.Empty ? await _repository.GetByIdAsync(id) : GetNewDetail();

            //TODO: Można to przypisanie id zrobić niżej w bazowych?
            ID = id;

            InitializeHardware(hardware);
            LoadHardwareTypes();
        }

        protected override bool OnCancelEditCanExecute()
        {
            return true;
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
            _repository.Add(Hardware);

            return Hardware;
        }

        private async void HardwareViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedHardwareType")
            {
                await LoadHardwareCustomItemsAsync();
            }
        }

        private void InitializeHardware(Hardware hardware)
        {
            Hardware = new HardwareWrapper(hardware);
        }

        private async Task LoadHardwareCustomItemsAsync()
        {
            var items = await _customItemRepository
                .FindByAsync(i => i.HardwareTypeId == SelectedHardwareType.Id);

            HardwareCustomItems.Clear();
            foreach (var item in items)
            {
                HardwareCustomItems
                    .Add(new HwCustomItem
                    { CustomItemId = item.Id, Hardware = Hardware.Model, Value = "" });
            }

            var query = from hci in HardwareCustomItems
                        from ci in items
                        where hci.CustomItemId == ci.Id
                        select new DisblayableCustomItem { HwCustomItem = hci, CustomItem = ci };

            var result = query.ToList();

            DisplayableCustomItems.Clear();
            DisplayableCustomItems.AddRange(result);
        }

        private async void LoadHardwareTypes()
        {
            HardwareTypes.Clear();
            var types = await _typeRepository.GetAllAsync();

            if (types != null)
                HardwareTypes.AddRange(types);
        }

        private void OnGoBackExecute()
        {
            _navigationService.Journal.GoBack();
        }

        private bool OnGoForwardCanExecute()
        {
            return true;
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

        public bool KeepAlive => true;

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (WizardMode)
            {
                AllowLoadAsync = false;
            }
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
            WizardMode = navigationContext.Parameters.GetValue<bool>("Wizard");

            var customer = navigationContext.Parameters.GetValue<Customer>("Customer");
            if (WizardMode && customer != null)
                Customer = customer;

            var id = navigationContext.Parameters.GetValue<Guid>("ID");
            if (id != null && AllowLoadAsync)
            {
                await LoadAsync(id);
            }
        }

        public override Task LoadAsync()
        {
            throw new NotImplementedException();
        }

        #endregion Navigation
    }
}