using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERService.Settings.ViewModels
{
    public class GeneralSettingsViewModel : BindableBase
    {
        public string Title { get { return "Ogólne"; } }

        public string Content { get { return "Ustawienia ogólne"; } }

        public GeneralSettingsViewModel()
        {

        }
    }
}
