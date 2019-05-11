using Prism.Mvvm;

namespace ERService.Toolbar.ViewModels
{
    public class ToolBarViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public ToolBarViewModel()
        {
            Message = "ToolBar";
        }
    }
}
