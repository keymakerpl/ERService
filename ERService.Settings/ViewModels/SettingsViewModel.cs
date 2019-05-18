using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Settings.ViewModels
{
    public class SettingsViewModel : BindableBase, INavigationAware
    {              
        public SettingsViewModel()
        {
           
        }
        
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
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
