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
        private readonly ISettingsManager _settingsManager;
        private readonly IRegionManager _regionManager;

        public OrderWizardViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IMessageDialogService messageDialogService,
            IOrderContext orderWizardContext, ISettingsManager settingsManager)
            : base(eventAggregator, messageDialogService)
        {
            _regionManager = regionManager;
            _orderWizardContext = orderWizardContext;
            _settingsManager = settingsManager;

            WizardMode = true;

            GoForwardCommand = new DelegateCommand(OnGoForwardExecute, OnGoForwardCanExecute);
            GoBackwardCommand = new DelegateCommand(OnGoBackExecute, OnGoBackwardCanExecute);            
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

            _navigationContext = navigationContext;
            _regionManager.RequestNavigate(RegionNames.OrderWizardStageRegion, ViewNames.OrderWizardCustomerView);

            SetTitle(Current);

            _regionManager.Regions[RegionNames.OrderWizardStageRegion].NavigationService.Navigated += (s, a) =>
            {
                Current = a.Uri.ToString();

                SetTitle(Current);

                GoForwardCommand.RaiseCanExecuteChanged();
                GoBackwardCommand.RaiseCanExecuteChanged();
            };            
        }

        private void SetTitle(string currentView)
        {
            switch (currentView)
            {
                case ViewNames.OrderWizardCustomerView:
                    Title = "1. Nowa naprawa | Klient...";
                    break;
                case ViewNames.OrderWizardHardwareView:
                    Title = "2. Nowa naprawa | Sprzęt...";
                    break;
                case ViewNames.OrderWizardOrderView:
                    Title = "3. Nowa naprawa | Zgłoszenie...";
                    break;
            }

            RaiseDetailOpenedEvent(Guid.Empty, Title);
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

            _canNavigateAfterSave = true;
            _orderWizardContext.Save();
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

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
            _regionManager.Regions[RegionNames.OrderWizardStageRegion].RemoveAll();
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