using System;
using System.Threading.Tasks;
using ERService.Infrastructure.Notifications.ToastNotifications;
using MahApps.Metro.Controls.Dialogs;

namespace ERService.Infrastructure.Dialogs
{
    public class MessageDialogService : IMessageDialogService
    {
        private IDialogCoordinator _dialogCoordinator;
        private MetroDialogSettings _confirmDialogSettings;
        private readonly IToastNotificationService _toastNotificationService;

        public MessageDialogService(IDialogCoordinator dialogCoordinator, IToastNotificationService toastNotificationService)
        {
            _confirmDialogSettings = new MetroDialogSettings { AffirmativeButtonText = "Tak", NegativeButtonText = "Anuluj" };

            _dialogCoordinator = dialogCoordinator;
            _toastNotificationService = toastNotificationService;
        }

        public async Task<DialogResult> ShowConfirmationMessageAsync(object context, string title, string message)
        {
            return await _dialogCoordinator
                .ShowMessageAsync(context, title, message, MessageDialogStyle.AffirmativeAndNegative, _confirmDialogSettings) == MessageDialogResult.Affirmative ?
                DialogResult.OK : DialogResult.Cancel;
        }

        public async Task<string> ShowInputMessageAsync(object context, string title, string message)
        {
            return await _dialogCoordinator.ShowInputAsync(context, title, message, _confirmDialogSettings);
        }

        public async Task ShowInformationMessageAsync(object context, string title, string message)
        {
            await _dialogCoordinator.ShowMessageAsync(context, title, message);
        }

        public async Task ShowAccessDeniedMessageAsync(object context, string title = null, string message = null)
        {
            var dialogTitle = title != null ? title : "Brak uprawnień...";
            var dialogMessage = message != null ? message : "Nie masz uprawnień do wykonania tej czynności.";

            await _dialogCoordinator.ShowMessageAsync(context, dialogTitle, dialogMessage);
        }

        public void ShowInsideContainer(string title, string message, NotificationTypes notificationType, string areaName = "WindowArea", Action onClick = null, Action onClose = null)
        {
            _toastNotificationService.ShowInsideContainer(title, message, notificationType, areaName, onClick, onClose);
        }

        public void ShowOverTaskBar(string title, string message, NotificationTypes notificationType, Action onClick = null, Action onClose = null)
        {
            _toastNotificationService.ShowOverTaskBar(title, message, notificationType, onClick, onClose);
        }
    }

    public enum DialogResult
    {
        OK,
        Cancel
    }
}
