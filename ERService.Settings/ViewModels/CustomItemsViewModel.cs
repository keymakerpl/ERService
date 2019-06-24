using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERService.Settings.ViewModels
{
    public class CustomItemsViewModel : BindableBase
    {
        public string Title { get { return "Custom Items Settings"; } }

        public string Content { get { return "CustomItems Settings"; } }

        public CustomItemsViewModel()
        {

        }        
    }
}
