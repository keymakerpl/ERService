using System;

namespace ERService.Infrastructure.Notifications.ToastNotifications
{
    public interface IToastNotificationService
    {
        void ShowInsideContainer(string title, string message, NotificationTypes notificationType, string areaName = "WindowArea", Action onClick = null, Action onClose = null);
        void ShowOverTaskBar(string title, string message, NotificationTypes notificationType, Action onClick = null, Action onClose = null);
    }
}