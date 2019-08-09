using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace ERService.Infrastructure.Dialogs
{
    //TODO: Może przerobić to na Fabrykę, Strategię?
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

        public async Task<DialogResult> ShowInformationMessageAsync(object context, string title, string message)
        {
            return await _dialogCoordinator.ShowMessageAsync(context, title, message, MessageDialogStyle.Affirmative) == MessageDialogResult.Affirmative
                ? DialogResult.OK : DialogResult.Cancel;
        }
    }

    public enum DialogResult
    {
        OK,
        Cancel
    }
}
