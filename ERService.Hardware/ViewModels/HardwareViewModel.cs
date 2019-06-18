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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace ERService.HardwareModule.ViewModels
{
    public class HardwareViewModel : DetailViewModelBase, INavigationAware, IConfirmNavigationRequest, IRegionMemberLifetime
    {       
        private bool _wizardMode;
        public bool WizardMode { get => _wizardMode; set { SetProperty(ref _wizardMode, value); } }

        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand GoForwardCommand { get; private set; }

        public HardwareWrapper _hardware;
        public HardwareWrapper Hardware { get => _hardware; set { SetProperty(ref _hardware, value); } }

        public HardwareType _selectedHardwareType;
        public HardwareType SelectedHardwareType { get => _selectedHardwareType; set { SetProperty(ref _selectedHardwareType, value); } }

        public ObservableCollection<HwCustomItem> HardwareCustomItems;

        public ObservableCollection<HardwareType> _hardwareTypes;
        public ObservableCollection<HardwareType> HardwareTypes { get => _hardwareTypes; set { SetProperty(ref _hardwareTypes, value); } }

        private Customer _customer;
        public Customer Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }

        private IRegionManager _regionManager;
        private IHardwareRepository _repository;
        private IHardwareTypeRepository _typeRepository;
        private ICustomItemRepository _customItemRepository;
        private IRegionNavigationService _navigationService;

        public HardwareViewModel(IHardwareRepository repository, IHardwareTypeRepository typeRepository, ICustomItemRepository customItemRepository,
            IRegionManager regionManager, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            //TODO: Czy można zrobić tu refactor? Może jakiś wzorzec kompozyt? Aby wrzucić repo w jedno miejsce
            _regionManager = regionManager;
            _repository = repository;
            _typeRepository = typeRepository;
            _customItemRepository = customItemRepository;

            HardwareCustomItems = new ObservableCollection<HwCustomItem>();
            HardwareTypes= new ObservableCollection<HardwareType>();

            PropertyChanged += HardwareViewModel_PropertyChanged;

            GoBackCommand = new DelegateCommand(OnGoBackExecute);
            GoForwardCommand = new DelegateCommand(OnGoForwardExecute, OnGoForwardCanExecute);
        }

        private void HardwareViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedHardwareType" )
            {
                LoadCustomItems();
            }
        }

        private void LoadCustomItems()
        {
            var list = _customItemRepository.GetCustomItemsByHardwareType(SelectedHardwareType.Id);
        }

        private bool OnGoForwardCanExecute()
        {
            return true;
        }

        private void OnGoForwardExecute()
        {
            var parameters = new NavigationParameters();
            parameters.Add("ID", Guid.Empty);
            parameters.Add("Wizard", true);
            parameters.Add("Customer", Customer);

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
            InitializeHwCustomItems();
            InitializeHardwareTypes();

        }

        private async void InitializeHardwareTypes()
        {
            HardwareTypes.Clear();
            var types = await _typeRepository.GetAllAsync();            

            if(types != null)
                HardwareTypes.AddRange(types);
        }

        private void InitializeHwCustomItems()
        {
            HardwareCustomItems.Clear();
            foreach (var item in Hardware.Model.HardwareCustomItems)
            {
                HardwareCustomItems.Add(item);
            }
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
