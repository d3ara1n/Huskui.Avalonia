using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Models;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views.Overlays
{
    public partial class NotificationsPage : ControlPage
    {
        public NotificationsPage() => InitializeComponent();

        private AppWindow? GetAppWindow() => TopLevel.GetTopLevel(this) as AppWindow;

        private void OnShowInfoNotificationClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Information,
                Title = "Information",
                Content = "This is an informational notification message."
            };

            appWindow.PopNotification(notification);
        }

        private void OnShowSuccessNotificationClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Success, Title = "Success", Content = "Operation completed successfully!"
            };

            appWindow.PopNotification(notification);
        }

        private void OnShowWarningNotificationClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Warning,
                Title = "Warning",
                Content = "Please review your settings before proceeding."
            };

            appWindow.PopNotification(notification);
        }

        private void OnShowDangerNotificationClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Danger,
                Title = "Error",
                Content = "An error occurred while processing your request."
            };

            appWindow.PopNotification(notification);
        }

        private void OnShowActionNotificationClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Information,
                Title = "Action Required",
                Content = "Your session will expire in 5 minutes."
            };

            notification.Actions.Add(new() { Text = "Extend" });
            notification.Actions.Add(new() { Text = "Logout" });

            appWindow.PopNotification(notification);
        }

        private void OnShowProgressNotificationClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Information,
                Title = "File Download",
                Content = "Downloading update.zip... 35% complete",
                IsProgressBarVisible = true,
                Progress = 35
            };

            notification.Actions.Add(new() { Text = "Cancel" });

            appWindow.PopNotification(notification);
        }

        private void OnShowRichNotificationClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Success,
                Title = "New Message",
                Content = "John Doe: Hey, are you available for a quick call?"
            };

            notification.Actions.Add(new() { Text = "Reply" });
            notification.Actions.Add(new() { Text = "Call" });

            appWindow.PopNotification(notification);
        }

        private void OnClearAllNotificationsClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Information,
                Title = "Notifications Cleared",
                Content = "All notifications have been dismissed."
            };

            appWindow.PopNotification(notification);
        }

        private void OnShowFileOperationClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Success,
                Title = "File Copied",
                Content = "document.pdf copied to Documents folder"
            };

            notification.Actions.Add(new() { Text = "Open" });
            notification.Actions.Add(new() { Text = "Show in Folder" });

            appWindow.PopNotification(notification);
        }

        private void OnShowSystemStatusClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Warning,
                Title = "Low Battery",
                Content = "Battery level is low (15% remaining)"
            };

            notification.Actions.Add(new() { Text = "Power Settings" });

            appWindow.PopNotification(notification);
        }

        private void OnShowUserActionClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Success,
                Title = "Document Saved",
                Content = "Your changes have been saved automatically at " + DateTime.Now.ToString("HH:mm:ss")
            };

            appWindow.PopNotification(notification);
        }

        private void OnShowBackgroundTaskClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var notification = new NotificationItem
            {
                Level = NotificationLevel.Information,
                Title = "Backup Complete",
                Content = "Cloud backup completed successfully. 1,247 files backed up (2.3 GB)"
            };

            notification.Actions.Add(new() { Text = "View Details" });

            appWindow.PopNotification(notification);
        }
    }
}