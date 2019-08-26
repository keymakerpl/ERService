using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace ERService.Infrastructure.Dialogs
{
    public class MessageDialogService : IMessageDialogService
    {
        private IDialogCoordinator _dialogCoordinator;
        private MetroDialogSettings _confirmDialogSettings;

        public MessageDialogService(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            _confirmDialogSettings = new MetroDialogSettings { AffirmativeButtonText = "Tak", NegativeButtonText = "Anuluj" };
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
    }

    public enum DialogResult
    {
        OK,
        Cancel
    }
}
