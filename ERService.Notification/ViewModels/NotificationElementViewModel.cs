using ERService.Infrastructure.Base;
using ERService.Infrastructure.Dialogs;
using Prism.Events;

namespace ERService.Notification.ViewModels
{
    public class NotificationElementViewModel : DetailViewModelBase
    {
        private string _customerName;

        private string _fault;

        private string _orderNumber;

        public NotificationElementViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService) : base(eventAggregator, messageDialogService)
        {
        }

        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        public string Fault
        {
            get { return _fault; }
            set { _fault = value; }
        }

        public string OrderNumber
        {
            get { return _orderNumber; }
            set { _orderNumber = value; }
        }
    }
}