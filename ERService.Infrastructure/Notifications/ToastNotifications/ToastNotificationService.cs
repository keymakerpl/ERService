using Notifications.Wpf;
using System;

namespace ERService.Infrastructure.Notifications.ToastNotifications
{
    public class ToastNotificationService : IToastNotificationService
    {
        private NotificationManager _notificationManager;

        public ToastNotificationService()
        {
            _notificationManager = new NotificationManager();
        }

        public void ShowOverTaskBar(string title, string message, NotificationTypes notificationType, Action onClick = null, Action onClose = null)
        {
            var content = GetContent(title, message, notificationType);
            _notificationManager.Show(content, onClick: onClick, onClose: onClose);
        }

        public void ShowInsideContainer(string title, string message, NotificationTypes notificationType, string areaName = "WindowArea", Action onClick = null, Action onClose = null)
        {
            var content = GetContent(title, message, notificationType);
            _notificationManager.Show(content, areaName: areaName, onClick: onClick, onClose: onClose);
        }

        private static NotificationContent GetContent(string title, string message, NotificationTypes notificationType)
        {
            var content = new NotificationContent();
            content.Title = title;
            content.Message = message;
            content.Type = (NotificationType)notificationType;

            return content;
        }        
    }

    public enum NotificationTypes
    {
        Information = NotificationType.Information,
        Warning = NotificationType.Warning,
        Error = NotificationType.Error,
        Success = NotificationType.Success
    }
}
