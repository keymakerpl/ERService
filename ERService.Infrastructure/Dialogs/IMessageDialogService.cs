using ERService.Infrastructure.Notifications.ToastNotifications;
using System.Threading.Tasks;

namespace ERService.Infrastructure.Dialogs
{
    public interface IMessageDialogService : IToastNotificationService
    {
        Task<DialogResult> ShowConfirmationMessageAsync(object context, string title, string message);
        Task<string> ShowInputMessageAsync(object context, string title, string message);
        Task ShowInformationMessageAsync(object context, string title, string message);
        Task ShowAccessDeniedMessageAsync(object context, string title = null, string message = null);

    }
}