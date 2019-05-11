using Prism.Mvvm;

namespace ERService.Header.ViewModels
{
    public class HeaderViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public HeaderViewModel()
        {
            Message = "Header";
        }
    }
}
