using System;
using System.Reflection;
using ERService.Infrastructure.Constants;
using ERService.Views;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Application.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private IRegionManager _regionManager;

        private readonly string _applicationName;
        public string ApplicationName { get { return _applicationName; } }

        public ShellViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            var assembly = Assembly.GetEntryAssembly();
            _applicationName = assembly.GetName().Name;

            ShowLoginWindow();
        }

        private void ShowLoginWindow()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(LoginWindowView));
        }
    }
}
