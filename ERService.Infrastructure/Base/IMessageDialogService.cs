using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERService.Infrastructure.Base
{
    public interface IMessageDialogService
    {
        Task<MessageDialogResult> ShowOkCancelDialog(string text, string title);
        void ShowInfoDialog(string text);
    }

    public enum MessageDialogResult
    {
        OK,
        Cancel
    }
}
