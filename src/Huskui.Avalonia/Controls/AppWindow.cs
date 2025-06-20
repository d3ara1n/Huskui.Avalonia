﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ToastHost, typeof(OverlayHost))]
[TemplatePart(PART_ModalHost, typeof(OverlayHost))]
[TemplatePart(PART_DialogHost, typeof(OverlayHost))]
[TemplatePart(PART_NotificationHost, typeof(NotificationHost))]
[PseudoClasses(":obstructed")]
public class AppWindow : Window
{
    public const string PART_ToastHost = nameof(PART_ToastHost);
    public const string PART_ModalHost = nameof(PART_ModalHost);
    public const string PART_DialogHost = nameof(PART_DialogHost);
    public const string PART_NotificationHost = nameof(PART_NotificationHost);

    public static readonly DirectProperty<AppWindow, bool> IsMaximizedProperty =
        AvaloniaProperty.RegisterDirect<AppWindow, bool>(nameof(IsMaximized),
                                                         o => o.IsMaximized,
                                                         (o, v) => o.IsMaximized = v);

    private OverlayHost? _dialogHost;
    private OverlayHost? _modalHost;
    private NotificationHost? _notificationHost;
    private OverlayHost? _toastHost;

    protected override Type StyleKeyOverride => typeof(AppWindow);

    public bool IsMaximized
    {
        get;
        set => SetAndRaise(IsMaximizedProperty, ref field, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == WindowStateProperty)
            IsMaximized = WindowState == WindowState.Maximized;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_toastHost != null)
            _toastHost.IsPresentChanged -= UpdateObstructed;
        if (_modalHost != null)
            _modalHost.IsPresentChanged -= UpdateObstructed;
        if (_dialogHost != null)
            _dialogHost.IsPresentChanged -= UpdateObstructed;

        _notificationHost = e.NameScope.Find<NotificationHost>(PART_NotificationHost);
        _toastHost = e.NameScope.Find<OverlayHost>(PART_ToastHost);
        _modalHost = e.NameScope.Find<OverlayHost>(PART_ModalHost);
        _dialogHost = e.NameScope.Find<OverlayHost>(PART_DialogHost);

        ArgumentNullException.ThrowIfNull(_toastHost);
        ArgumentNullException.ThrowIfNull(_modalHost);
        ArgumentNullException.ThrowIfNull(_dialogHost);

        _toastHost.IsPresentChanged += UpdateObstructed;
        _modalHost.IsPresentChanged += UpdateObstructed;
        _dialogHost.IsPresentChanged += UpdateObstructed;

        if (_toastHost is not null)
            LogicalChildren.Add(_toastHost);
        if (_modalHost is not null)
            LogicalChildren.Add(_modalHost);
        if (_dialogHost is not null)
            LogicalChildren.Add(_dialogHost);
        if (_notificationHost is not null)
            LogicalChildren.Add(_notificationHost);
    }

    private void UpdateObstructed(object? _, PropertyChangedRoutedEventArgs<bool> __)
    {
        ArgumentNullException.ThrowIfNull(_toastHost);
        ArgumentNullException.ThrowIfNull(_modalHost);
        ArgumentNullException.ThrowIfNull(_dialogHost);

        PseudoClasses.Set(":obstructed", _toastHost.IsPresent || _modalHost.IsPresent || _dialogHost.IsPresent);
    }

    public void PopToast(Toast toast)
    {
        ArgumentNullException.ThrowIfNull(_toastHost);
        _toastHost.Pop(toast);
    }

    public void PopDialog(Dialog dialog)
    {
        ArgumentNullException.ThrowIfNull(_dialogHost);
        _dialogHost.Pop(dialog);
    }

    public void PopModal(Modal modal)
    {
        ArgumentNullException.ThrowIfNull(_modalHost);
        _modalHost.Pop(modal);
    }

    public void PopNotification(NotificationItem notification)
    {
        ArgumentNullException.ThrowIfNull(_notificationHost);
        _notificationHost.Pop(notification);
    }
}