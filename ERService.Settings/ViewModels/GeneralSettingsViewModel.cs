using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERService.Settings.ViewModels
{
    public class GeneralSettingsViewModel : BindableBase
    {
        public string Title { get { return "General Settings"; } }

        public string Content { get { return "General Settings"; } }

        public GeneralSettingsViewModel()
        {

        }
    }
}
