using ERService.Business;
using ERService.HardwareModule.Data.Repository;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;
using Prism.Mvvm;

namespace ERService.HardwareModule.ViewModels
{
    public class DisblayableCustomItem : BindableBase
    {
        //private string _key;
        //public string Key { get { return _key; } set { SetProperty(ref _key, value); } }

        //private string _value;
        //public string Value { get { return _value; } set { SetProperty(ref _value, value); } }

        private CustomItem _customItem;
        public CustomItem CustomItem { get { return _customItem; } set { SetProperty(ref _customItem, value); } }

        private HwCustomItem _hwCustomItem;
        public HwCustomItem HwCustomItem { get { return _hwCustomItem; } set { SetProperty(ref _hwCustomItem, value); } }
    }

    public class HardwareViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {               
        public bool WizardMode { get => _wizardMode; set { SetProperty(ref _wizardMode, value); } }

        public Customer Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }

        public HardwareWrapper Hardware { get => _hardware; set { SetProperty(ref _hardware, value); } }

        public HardwareType SelectedHardwareType { get => _selectedHardwareType; set { SetProperty(ref _selectedHardwareType, value); } }

        public ObservableCollection<HwCustomItem> HardwareCustomItems;

        public ObservableCollection<HardwareType> HardwareTypes { get => _hardwareTypes; set { SetProperty(ref _hardwareTypes, value); } }

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

        private bool _wizardMode;
        private Customer _customer;
        private HardwareWrapper _hardware;
        private HardwareType _selectedHardwareType;
        private ObservableCollection<HardwareType> _hardwareTypes;
        private IRegionManager _regionManager;
        private IHardwareRepository _repository;
        private IHardwareTypeRepository _typeRepository;
        private ICustomItemRepository _customItemRepository;
        private IRegionNavigationService _navigationService;
        private ObservableCollection<DisblayableCustomItem> _displayableCustomItems;

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

        private void HardwareViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedHardwareType" )
            {
                LoadHardwareCustomItemsAsync();
            }
        }

        private async void LoadHardwareCustomItemsAsync()
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

        private void OnGoBackExecute()
        {
            _navigationService.Journal.GoBack();
        }

        public override async Task LoadAsync(Guid id)
        {
            var hardware = id != Guid.Empty ? await _repository.GetByIdAsync(id) : GetNewDetail();
            
            //TODO: Można to przypisanie id zrobić niżej w bazowych?
            ID = id;

            InitializeHardware(hardware);
            //LoadHardwareCustomItems();
            LoadHardwareTypes();

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            PropertyChanged += HardwareViewModel_PropertyChanged1;
        }

        private void HardwareViewModel_PropertyChanged1(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedHardwareType")
            {
                LoadHardwareCustomItemsAsync();
            }
        }

        private async void LoadHardwareTypes()
        {
            HardwareTypes.Clear();
            var types = await _typeRepository.GetAllAsync();            

            if(types != null)
                HardwareTypes.AddRange(types);
        }

        private void InitializeHardware(Hardware hardware)
        {
            Hardware = new HardwareWrapper(hardware);
        }

        private Hardware GetNewDetail()
        {
            var Hardware = new Hardware();
            _repository.Add(Hardware);

            return Hardware;
        }

        protected override void OnSaveExecute()
        {
           
        }

        protected override bool OnSaveCanExecute()
        {
            return !WizardMode;
        }

        protected override void OnCancelEditExecute()
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
        }

        protected override bool OnCancelEditCanExecute()
        {
            return true;
        }

        #region Navigation

        public bool KeepAlive => true;

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

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        #endregion
    }
}
