using ERService.Business;
using ERService.CustomerModule.Wrapper;
using ERService.HardwareModule;
using ERService.HardwareModule.Data.Repository;
using ERService.Infrastructure.Attributes;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Events;
using ERService.Infrastructure.Helpers;
using ERService.Infrastructure.Interfaces;
using ERService.OrderModule.Data.Repository;
using ERService.OrderModule.Repository;
using ERService.OrderModule.Wrapper;
using ERService.RBAC;
using ERService.TemplateEditor.Data.Repository;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ERService.OrderModule.ViewModels
{
    public class CompanyLogo
    {
        [Interpreter(Name = "Logo firmy", Pattern = "[%o_CompanyLogo%]")]
        public byte[] ImageSource { get; set; }
    }

    public class OrderViewModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IHardwareTypeRepository _hardwareTypesRepository;
        private readonly IRBACManager _rBACManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IPrintTemplateRepository _templateRepository;        
        private CustomerWrapper _customer;        
        private HardwareWrapper _hardware;
        private IRegionNavigationService _navigationService;
        private INumerationRepository _numerationRepository;
        private OrderWrapper _order;
        private IOrderRepository _orderRepository;
        private IRegionManager _regionManager;
        private Blob _selectedAttachment;
        private HardwareType _selectedHardwareType;
        private OrderStatus _selectedOrderStatus;
        private OrderType _selectedOrderType;
        private IOrderStatusRepository _statusRepository;
        private IOrderTypeRepository _typeRepository;
        private readonly IImagesCollection _imagesCollection;

        //TODO: Za duży konstruktor? Dodać IOrderContext? UnitOfWork?
        public OrderViewModel(IRegionManager regionManager, IOrderRepository orderRepository, IOrderTypeRepository typeRepository,
            IOrderStatusRepository statusRepository, IEventAggregator eventAggregator, IHardwareTypeRepository hardwareTypesRepository,
            INumerationRepository numerationRepository, IMessageDialogService messageDialogService, IRBACManager rBACManager,
            IPrintTemplateRepository templateRepository, ISettingsManager settingsManager, IImagesCollection imagesCollection) : base(eventAggregator, messageDialogService)
        {
            _orderRepository = orderRepository;
            _typeRepository = typeRepository;
            _statusRepository = statusRepository;
            _hardwareTypesRepository = hardwareTypesRepository;
            _numerationRepository = numerationRepository;
            _rBACManager = rBACManager;
            _templateRepository = templateRepository;
            _settingsManager = settingsManager;
            _regionManager = regionManager;
            _imagesCollection = imagesCollection;

            OrderStatuses = new ObservableCollection<OrderStatus>();
            OrderTypes = new ObservableCollection<OrderType>();
            HardwareTypes = new ObservableCollection<HardwareType>();
            Attachments = new ObservableCollection<Blob>();
            PrintTemplates = new ObservableCollection<PrintTemplate>();

            AddAttachmentCommand = new DelegateCommand(OnAddAttachmentExecute);
            RemoveAttachmentCommand = new DelegateCommand(OnRemoveAttachmentExecute, OnRemoveAttachmentCanExecute);
            PrintCommand = new DelegateCommand<object>(OnPrintExecute);
            ShowAttachmentCommand = new DelegateCommand<Guid?>(OnShowAttachmentCommand);
            ShowHardwareDetailFlyoutCommand = new DelegateCommand(OnShowHardwareFlyoutExecute);
            ShowCustomerDetailFlyoutCommand = new DelegateCommand(OnShowCustomerFlyoutExecute);
        }

        public DelegateCommand AddAttachmentCommand { get; }

        public ObservableCollection<Blob> Attachments { get; }

        public CustomerWrapper Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }

        public DelegateCommand GoBackCommand { get; }

        public HardwareWrapper Hardware
        {
            get { return _hardware; }
            set { SetProperty(ref _hardware, value); }
        }

        public OrderWrapper Order { get => _order; set { SetProperty(ref _order, value); } }

        public ObservableCollection<HardwareType> HardwareTypes { get; }

        public ObservableCollection<OrderStatus> OrderStatuses { get; }

        public ObservableCollection<OrderType> OrderTypes { get; }

        public ObservableCollection<PrintTemplate> PrintTemplates { get; }

        public DelegateCommand<object> PrintCommand { get; }

        public DelegateCommand RemoveAttachmentCommand { get; }

        public Blob SelectedAttachment
        {
            get { return _selectedAttachment; }
            set { SetProperty(ref _selectedAttachment, value); RemoveAttachmentCommand.RaiseCanExecuteChanged(); }
        }

        public HardwareType SelectedHardwareType
        {
            get { return _selectedHardwareType; }
            set { SetProperty(ref _selectedHardwareType, value); }
        }

        public OrderStatus SelectedOrderStatus
        {
            get { return _selectedOrderStatus; }
            set
            {
                SetProperty(ref _selectedOrderStatus, value);
                Order.OrderStatusId = value?.Id;
            }
        }

        public OrderType SelectedOrderType
        {
            get { return _selectedOrderType; }
            set
            {
                SetProperty(ref _selectedOrderType, value);
                Order.OrderTypeId = value?.Id;
            }
        }

        public DelegateCommand ShowCustomerDetailFlyoutCommand { get; }

        public DelegateCommand ShowHardwareDetailFlyoutCommand { get; }

        public DelegateCommand<Guid?> ShowAttachmentCommand { get; }

        private void RaiseSideMenuExpandToggled(SideFlyouts flyout, Guid detailID, string viewName)
        {
            _eventAggregator
                                        .GetEvent<AfterSideMenuExpandToggled>()
                                        .Publish(new AfterSideMenuExpandToggledArgs
                                        {
                                            Flyout = flyout,
                                            DetailID = detailID,
                                            ViewName = viewName,
                                            IsReadOnly = true
                                        });
        }

        public void OnShowAttachmentCommand(Guid? id)
        {
            if (!id.HasValue && id == Guid.Empty)
                return;

            try
            {
                var att = Attachments.FirstOrDefault(a => a.Id == id);
                if (att != null)
                {
                    var path = Path.GetTempPath() + att.FileName;
                    File.WriteAllBytes(path, att.Data);
                    Process.Start(path);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _logger.Debug(ex);
            }
        }

        #region Navigation

        public override bool KeepAlive => false;

        private string GoBackView { get; set; }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;            

            var id = navigationContext.Parameters.GetValue<Guid>("ID");
            if (id != null && id != Guid.Empty)
            {
                try
                {
                    await LoadAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    _logger.Debug(ex);
                }
            }

            GoBackView = navigationContext.Parameters.GetValue<string>("GoBackView");

            if (!_rBACManager.LoggedUserHasPermission(AclVerbNames.CanEditOrder))
                IsReadOnly = true;
        }        

        #endregion Navigation

        #region Overrides

        public override async Task LoadAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            
            ID = id;

            await InitializeComboBoxes();
            await InitializePrintTemplates();
            InitializeOrder(order);
            InitializeHardware();
            InitializeCustomer();
            InitializeAttachments();
        }

        protected override void OnCancelEditExecute()
        {
            if (!String.IsNullOrEmpty(GoBackView))
            {
                _navigationService.Journal.Clear();
                _regionManager.RequestNavigate(RegionNames.ContentRegion, GoBackView);
            }
            else
            {
                _navigationService.Journal.GoBack();
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return Order != null && !Order.HasErrors && HasChanges;
        }

        protected async override void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_orderRepository.SaveAsync, () =>
            {
                HasChanges = _orderRepository.HasChanges();
                ID = Order.Id;

                _navigationService.Journal.GoBack();
            });
        }

        #endregion Overrides

        private void OnAddAttachmentExecute()
        {
            //TODO: Make open file dialog service
            var openFileDialog = new OpenFileDialog();
            var attachment = new Blob();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileBinary = FileUtils.GetFileBinary(openFileDialog.FileName);
                attachment.Data = fileBinary;
                attachment.FileName = openFileDialog.SafeFileName;
                attachment.Size = fileBinary.Length;
                attachment.Description = $"File attachment for order: {Order.Number}";
                attachment.Checksum = Cryptography.CalculateMD5(openFileDialog.FileName);

                Attachments.Add(attachment);
                Order.Model.Attachments.Add(attachment);                
            }
        }

        private async void OnPrintExecute(object parameter)
        {
            var template = parameter as PrintTemplate;
            if (template != null)
            {
                var logo = new CompanyLogo()
                {
                    ImageSource = _imagesCollection["logo"].ImageData
                };

                var companyConfig = await _settingsManager.GetConfigAsync(ConfigNames.CompanyInfoConfig);
                var parameters = new NavigationParameters();
                parameters.Add("ID", template.Id);
                parameters.Add("IsReadOnly", true);
                parameters.Add("IsToolbarVisible", false);
                parameters.Add("ModelWrappers", new object[]
                {
                    Customer,
                    Hardware,
                    Order,
                    companyConfig,
                    logo,
                    new AddressWrapper(Customer.Model.CustomerAddresses.FirstOrDefault())
                });

                _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.PrintTemplateEditorView, parameters);
            }
        }

        private bool OnRemoveAttachmentCanExecute()
        {
            return SelectedAttachment != null;
        }

        private void OnRemoveAttachmentExecute()
        {
            Order.Model.Attachments.Remove(SelectedAttachment);
            Attachments.Remove(SelectedAttachment);
        }

        private void OnShowCustomerFlyoutExecute()
        {
            RaiseSideMenuExpandToggled(SideFlyouts.DetailFlyout, Customer.Id, ViewNames.CustomerFlyoutDetailView);
        }

        private void OnShowHardwareFlyoutExecute()
        {
            RaiseSideMenuExpandToggled(SideFlyouts.DetailFlyout, Hardware.Id, ViewNames.HardwareFlyoutDetailView);
        }

        private void InitializeCustomer()
        {
            Customer = new CustomerWrapper(Order.Model.Customer);
        }

        private void InitializeHardware()
        {
            var hardware = Order.Model.Hardwares.FirstOrDefault();
            if (hardware != null)
            {
                SelectedHardwareType = hardware.HardwareType;
                Hardware = new HardwareWrapper(hardware);
                Hardware.PropertyChanged += ((sender, args) =>
                {
                    if (!HasChanges)
                    {
                        HasChanges = _orderRepository.HasChanges();
                        CancelCommand.RaiseCanExecuteChanged();
                    }

                    SaveCommand.RaiseCanExecuteChanged();                    
                });
            }
        }

        private void InitializeOrder(Order order)
        {
            _selectedOrderType = OrderTypes.FirstOrDefault(t => t.Id == order.OrderType.Id);
            _selectedOrderStatus = OrderStatuses.FirstOrDefault(s => s.Id == order.OrderStatus.Id);

            Order = new OrderWrapper(order);

            Order.PropertyChanged += ((sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _orderRepository.HasChanges();
                    CancelCommand.RaiseCanExecuteChanged();
                }
                
                if (args.PropertyName == nameof(Order.HasErrors))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }

                SaveCommand.RaiseCanExecuteChanged();                
            });

            if (Order.Id != Guid.Empty)
            {
                Title = $"Naprawa numer: {Order.Number}";
                RaiseDetailOpenedEvent(Order.Id, Title);
            }

            SaveCommand.RaiseCanExecuteChanged();            
        }

        private void InitializeAttachments()
        {
            Attachments.Clear();
            foreach (var att in Order.Model.Attachments)
            {
                Attachments.Add(att);
            }

            Attachments.CollectionChanged += (s, a) => 
            {
                HasChanges = true;
                SaveCommand.RaiseCanExecuteChanged();
            };
        }

        private async Task InitializeComboBoxes()
        {
            await LoadOrderStatusesAsync();
            await LoadOrderTypesAsync();
            await LoadHardwareTypesAsync();
        }        

        private async Task InitializePrintTemplates()
        {
            PrintTemplates.Clear();
            var templates = await _templateRepository.GetAllAsync();
            foreach (var template in templates)
            {
                PrintTemplates.Add(template);
            }
        }

        private async Task LoadHardwareTypesAsync()
        {
            HardwareTypes.Clear();
            var types = await _hardwareTypesRepository.GetAllAsync();
            foreach (var type in types)
            {
                HardwareTypes.Add(type);
            }
        }

        private async Task LoadOrderStatusesAsync()
        {
            OrderStatuses.Clear();
            var statuses = await _statusRepository.GetAllAsync();
            foreach (var status in statuses)
            {
                OrderStatuses.Add(status);
            }
        }

        private async Task LoadOrderTypesAsync()
        {
            OrderTypes.Clear();
            var types = await _typeRepository.GetAllAsync();
            foreach (var type in types)
            {
                OrderTypes.Add(type);
            }
        }                
    }
}