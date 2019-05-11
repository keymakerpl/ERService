using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERService.StartPage.ViewModels
{
    public class StartPageViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public StartPageViewModel()
        {
            Message = "View StarPage from your Prism Module";
        }
    }
}
