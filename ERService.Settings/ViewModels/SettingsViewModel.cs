using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Settings.ViewModels
{
    public class SettingsViewModel : BindableBase, INavigationAware
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private int _counter;
        public int Counter
        {
            get { return _counter; }
            set { SetProperty(ref _counter, value); RaisePropertyChanged(); }
        }        

        public SettingsViewModel()
        {
            Message = "View Settings from your Prism Module";
        }
        
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Counter++;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
