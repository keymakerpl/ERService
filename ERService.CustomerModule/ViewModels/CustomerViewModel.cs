using Prism.Mvvm;

namespace ERService.CustomerModule.ViewModels
{
    public class CustomerViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public CustomerViewModel()
        {
            Message = "View Customer from your Prism Module";
        }
    }
}
