using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERService.Application.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        public string Message { get; set; }

        public ShellViewModel()
        {
            Message = "Shell";
        }
    }
}
