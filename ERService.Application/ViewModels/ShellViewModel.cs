using System;
using System.Reflection;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Events;
using ERService.Views;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace ERService.Application.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly string _applicationName;
        public string ApplicationName { get { return _applicationName; } }

        public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<AfterLogedoutEvent>().Subscribe(OnUserLogedout, true);

            var assembly = Assembly.GetEntryAssembly();
            _applicationName = $"{assembly.GetName().Name}";

            ShowLoginWindow();
        }

        private void OnUserLogedout(UserAuthorizationEventArgs obj)
        {
            _regionManager.Regions[RegionNames.ContentRegion].RemoveAll();
            ShowLoginWindow();
        }

        private void ShowLoginWindow()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(LoginView));
        }
    }
}
