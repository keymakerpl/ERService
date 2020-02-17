using ERService.Infrastructure.Constants;
using ERService.Notification.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ERService.Notification
{
    public class NotificationModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public NotificationModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.NotificationRegion, ViewNames.NotificationListView);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NotificationListView>(ViewNames.NotificationListView);            
            containerRegistry.RegisterForNavigation<NotificationElementView>(ViewNames.NotificationElementView);
        }
    }
}