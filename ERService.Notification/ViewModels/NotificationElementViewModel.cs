using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using Prism.Events;
using Prism.Regions;

namespace ERService.Notification.ViewModels
{
    public class NotificationElementViewModel : DetailViewModelBase
    {

        private string _customerName;

        private string _fault;

        private string _orderNumber;
        private readonly IRegionManager _regionManager;

        public NotificationElementViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
            _regionManager = regionManager;
        }

        public string CustomerName
        {
            get { return _customerName; }
            set { SetProperty(ref _customerName, value); }
        }

        public string Fault
        {
            get { return _fault; }
            set { SetProperty(ref _fault, value); }
        }

        public string OrderNumber
        {
            get { return _orderNumber; }
            set { SetProperty(ref _orderNumber, value); }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            var customerName = navigationContext.Parameters.GetValue<string>("CustomerName");
            if (customerName != null)
                CustomerName = customerName;

            var fault = navigationContext.Parameters.GetValue<string>("Fault");
            if (fault != null)
                Fault = fault;

            var orderNumber = navigationContext.Parameters.GetValue<string>("OrderNumber");
            if (orderNumber != null)
                OrderNumber = orderNumber;
        }
    }
}