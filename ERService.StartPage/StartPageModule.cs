using System;
using ERService.Infrastructure.Constants;
using ERService.Infrastructure.Events;
using ERService.StartPage.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.StartPage
{
    public class StartPageModule : IModule
    {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;

        public StartPageModule(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _eventAggregator.GetEvent<AfterUserLoggedinEvent>().Subscribe(ContinueInitialization, true);
        }

        private void ContinueInitialization(UserAuthorizationEventArgs obj)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.StartPageView);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<StartPageView>(ViewNames.StartPageView);
        }
    }
}