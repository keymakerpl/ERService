using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERService.Settings.ViewModels
{
    public class GeneralSettingsViewModel : BindableBase, INavigationAware
    {
        public string Title { get { return "Ogólne"; } }

        public string Content { get { return "Ustawienia ogólne"; } }

        public GeneralSettingsViewModel()
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
