using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Models;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class GrowlsPage : ControlPage
{
    public GrowlsPage() => InitializeComponent();

    private AppWindow? GetAppWindow() => TopLevel.GetTopLevel(this) as AppWindow;

    private void OnShowInfoGrowlClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Information,
            Title = "Information",
            Content = "This is an informational notification message."
        };

        appWindow.PopGrowl(notification);
    }

    private void OnShowSuccessGrowlClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Success, Title = "Success", Content = "Operation completed successfully!"
        };

        appWindow.PopGrowl(notification);
    }

    private void OnShowWarningGrowlClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Warning,
            Title = "Warning",
            Content = "Please review your settings before proceeding."
        };

        appWindow.PopGrowl(notification);
    }

    private void OnShowDangerGrowlClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Danger, Title = "Error", Content = "An error occurred while processing your request."
        };

        appWindow.PopGrowl(notification);
    }

    private void OnShowActionGrowlClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Information,
            Title = "Action Required",
            Content = "Your session will expire in 5 minutes."
        };

        notification.Actions.Add(new() { Text = "Extend" });
        notification.Actions.Add(new() { Text = "Logout" });

        appWindow.PopGrowl(notification);
    }

    private void OnShowProgressGrowlClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Information,
            Title = "File Download",
            Content = "Downloading update.zip... 35% complete",
            IsProgressBarVisible = true,
            Progress = 35
        };

        notification.Actions.Add(new() { Text = "Cancel" });

        appWindow.PopGrowl(notification);
    }

    private void OnShowRichGrowlClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Success,
            Title = "New Message",
            Content = "John Doe: Hey, are you available for a quick call?"
        };

        notification.Actions.Add(new() { Text = "Reply" });
        notification.Actions.Add(new() { Text = "Call" });

        appWindow.PopGrowl(notification);
    }

    private void OnClearAllGrowlsClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Information,
            Title = "Growls Cleared",
            Content = "All notifications have been dismissed."
        };

        appWindow.PopGrowl(notification);
    }

    private void OnShowFileOperationClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Success, Title = "File Copied", Content = "document.pdf copied to Documents folder"
        };

        notification.Actions.Add(new() { Text = "Open" });
        notification.Actions.Add(new() { Text = "Show in Folder" });

        appWindow.PopGrowl(notification);
    }

    private void OnShowSystemStatusClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Warning, Title = "Low Battery", Content = "Battery level is low (15% remaining)"
        };

        notification.Actions.Add(new() { Text = "Power Settings" });

        appWindow.PopGrowl(notification);
    }

    private void OnShowUserActionClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Success,
            Title = "Document Saved",
            Content = "Your changes have been saved automatically at " + DateTime.Now.ToString("HH:mm:ss")
        };

        appWindow.PopGrowl(notification);
    }

    private void OnShowBackgroundTaskClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        var notification = new GrowlItem
        {
            Level = GrowlLevel.Information,
            Title = "Backup Complete",
            Content = "Cloud backup completed successfully. 1,247 files backed up (2.3 GB)"
        };

        notification.Actions.Add(new() { Text = "View Details" });

        appWindow.PopGrowl(notification);
    }
}
