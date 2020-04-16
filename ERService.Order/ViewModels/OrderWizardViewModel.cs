using ERService.Business;
using ERService.CustomerModule.Wrapper;
using ERService.Infrastructure.Base;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Dialogs;
using ERService.Infrastructure.Interfaces;
using ERService.Infrastructure.Notifications.ToastNotifications;
using ERService.TemplateEditor.Data.Repository;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ERService.OrderModule.ViewModels
{
    public class OrderWizardViewModel : DetailViewModelBase
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private NavigationContext _navigationContext;
        
        private readonly IOrderContext _orderWizardContext;
        private readonly IPrintTemplateRepository _printTemplateRepository;
        private readonly ISettingsManager _settingsManager;
        private readonly IRegionManager _regionManager;

        public OrderWizardViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            IOrderContext orderWizardContext, IPrintTemplateRepository printTemplateRepository, ISettingsManager settingsManager)
            : base(eventAggregator, messageDialogService)
        {
            _regionManager = regionManager;
            _orderWizardContext = orderWizardContext;
            _printTemplateRepository = printTemplateRepository;
            _settingsManager = settingsManager;

            WizardMode = true;

            PrintTemplates = new ObservableCollection<PrintTemplate>();

            GoForwardCommand = new DelegateCommand(OnGoForwardExecute, OnGoForwardCanExecute);
            GoBackwardCommand = new DelegateCommand(OnGoBackExecute, OnGoBackwardCanExecute);
            PrintCommand = new DelegateCommand<object>(OnPrintExecute);
        }

        private async void OnPrintExecute(object parameter)
        {
            var template = parameter as PrintTemplate;
            if (template != null)
            {
                var companyConfig = await _settingsManager.GetConfigAsync(ConfigNames.CompanyInfoConfig);
                var parameters = new NavigationParameters();
                parameters.Add("ID", template.Id);
                parameters.Add("IsReadOnly", true);
                parameters.Add("IsToolbarVisible", false);
                parameters.Add("ModelWrappers", new object[]
                {
                    _orderWizardContext.Customer,
                    _orderWizardContext.Hardware,
                    _orderWizardContext.Order,
                    companyConfig,
                    new AddressWrapper(_orderWizardContext.Customer.Model.CustomerAddresses.FirstOrDefault())
                });

                _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.PrintTemplateEditorView, parameters);
            }
        }

        public DelegateCommand GoBackwardCommand { get; }
        public DelegateCommand GoForwardCommand { get; }
        public DelegateCommand<object> PrintCommand { get; }

        public ObservableCollection<PrintTemplate> PrintTemplates { get; }

        public bool WizardMode { get; }
        private string Current { get; set; } = ViewNames.OrderWizardCustomerView;

        private bool _canNavigateAfterSave;
        public override async void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (!_canNavigateAfterSave)
            {
                var dialogResult = await _messageDialogService.ShowConfirmationMessageAsync(this, "Nowa naprawa...", "Anulować dodawanie nowej naprawy?");
                _canNavigateAfterSave = dialogResult == DialogResult.OK;
            }

            continuationCallback(_canNavigateAfterSave);
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            InitializeRegionContext();
            InitializePrintTemplates();

            _navigationContext = navigationContext;
            _regionManager.RequestNavigate(RegionNames.OrderWizardStageRegion, ViewNames.OrderWizardCustomerView);

            _regionManager.Regions[RegionNames.OrderWizardStageRegion].NavigationService.Navigated += (s, a) =>
            {
                Current = a.Uri.ToString();

                GoForwardCommand.RaiseCanExecuteChanged();
                GoBackwardCommand.RaiseCanExecuteChanged();
            };
        }

        private async void InitializePrintTemplates()
        {
            PrintTemplates.Clear();
            var templates = await _printTemplateRepository.GetAllAsync();
            foreach (var template in templates)
            {
                PrintTemplates.Add(template);
            }
        }

        protected override void OnCancelEditExecute()
        {
            _navigationContext.NavigationService.Journal.GoBack();
        }

        protected override bool OnSaveCanExecute()
        {
            return true;
        }

        protected override void OnSaveExecute()
        {
            if (!TryValidate())
                return;

            _orderWizardContext.Save();
            _canNavigateAfterSave = true;
            _navigationContext.NavigationService.Journal.GoBack();
        }
        
        private void OnGoBackExecute()
        {
            _regionManager.Regions[RegionNames.OrderWizardStageRegion].NavigationService.Journal.GoBack();
        }

        private bool OnGoBackwardCanExecute()
        {
            return Current != ViewNames.OrderWizardCustomerView;
        }

        private bool OnGoForwardCanExecute()
        {
            return Current != ViewNames.OrderWizardOrderView;
        }

        private void OnGoForwardExecute()
        {
            switch (Current)
            {
                case ViewNames.OrderWizardCustomerView:
                    _regionManager.RequestNavigate(RegionNames.OrderWizardStageRegion, ViewNames.OrderWizardHardwareView);
                    break;

                case ViewNames.OrderWizardHardwareView:
                    _regionManager.RequestNavigate(RegionNames.OrderWizardStageRegion, ViewNames.OrderWizardOrderView);
                    break;

                default:
                    _regionManager.RequestNavigate(RegionNames.OrderWizardStageRegion, ViewNames.OrderWizardCustomerView);
                    break;
            }
        }

        private void InitializeRegionContext()
        {
            _regionManager.Regions[RegionNames.OrderWizardStageRegion].Context = _orderWizardContext;
        }

        private void ShowError(string error)
        {
            _messageDialogService.ShowInsideContainer("Błąd", error, NotificationTypes.Error);
        }

        private bool TryValidate()
        {
            try
            {
                if (_orderWizardContext.HasErrors)
                {
                    var customerErrors = _orderWizardContext.Customer.GetAllErrors().Cast<string>();
                    var hardwareErrors = _orderWizardContext.Hardware.GetAllErrors().Cast<string>();
                    var orderErrors = _orderWizardContext.Order.GetAllErrors().Cast<string>();

                    foreach (var error in customerErrors)
                    {
                        ShowError(error);
                    }

                    foreach (var error in hardwareErrors)
                    {
                        ShowError(error);
                    }

                    foreach (var error in orderErrors)
                    {
                        ShowError(error);
                    }

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
    }
}