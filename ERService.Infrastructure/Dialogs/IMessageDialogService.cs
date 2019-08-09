using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace ERService.Infrastructure.Dialogs
{
    public interface IMessageDialogService
    {
        Task<DialogResult> ShowConfirmationMessageAsync(object context, string title, string message);
        Task<string> ShowInputMessageAsync(object context, string title, string message);
        Task<DialogResult> ShowInformationMessageAsync(object context, string title, string message);
    }
}