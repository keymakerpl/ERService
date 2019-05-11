using Prism.Mvvm;

namespace ERService.StatusBar.ViewModels
{
    public class StatusBarViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public StatusBarViewModel()
        {
            Message = "Status Bar";
        }
    }
}
