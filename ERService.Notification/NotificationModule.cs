using ERService.Infrastructure.Constants;
using ERService.Notification.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.Notification
{
    public class NotificationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
 
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation(typeof(NotificationListView), ViewNames.NotificationView);
        }
    }
}