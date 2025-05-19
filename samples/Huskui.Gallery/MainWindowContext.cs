using System;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Huskui.Avalonia;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Models;
using Huskui.Gallery.Views;

namespace Huskui.Gallery;

public partial class MainWindowContext : ObservableObject
{
    public Frame Delegate { get; set; } = null!;

    public AvaloniaList<EntryModel> Entries { get; } =
    [
        new EntryModel { Display = "Buttons", Page = typeof(ButtonsView) },
        new EntryModel { Display = "Buttons", Page = null },
        new EntryModel { Display = "Buttons", Page = null }
    ];

    #region Properties

    [ObservableProperty]
    public partial EntryModel? SelectedEntry { get; set; }

    partial void OnSelectedEntryChanged(EntryModel? value)
    {
        if (value is not null)
            Delegate.Navigate(value.Page, null, null);
    }

    #endregion

    private static int activatorErrorCount;

    public static object? Activate(Type view, object? parameter)
    {
        try
        {
            if (!view.IsAssignableTo(typeof(Page)))
                throw new ArgumentOutOfRangeException(nameof(view), view, "Parameter view must be derived from Page");

            var name = view.FullName!.Replace("View", "ViewModel", StringComparison.Ordinal);
            var type = Type.GetType(name);

            var page = Activator.CreateInstance(view) as Page;

            if (page != null && type is not null)
            {
                if (!type.IsAssignableTo(typeof(ObservableObject)))
                    throw new ArgumentOutOfRangeException(nameof(type),
                                                          type,
                                                          $"{view.Name} was bound to a view model which is not derived from ObservableObject");

                var viewModel = parameter is not null && type.GetConstructor([parameter.GetType()]) != null
                                    ? Activator.CreateInstance(type, parameter)
                                    : Activator.CreateInstance(type);

                page.DataContext = viewModel;

                if (viewModel is IPageModel pageModel)
                    page.Model = pageModel;
            }

            activatorErrorCount = 0;
            return page;
        }
        catch (Exception ex)
        {
            // 避免又产生异常而导致无限循环
            if (activatorErrorCount++ < 3)
                return Activate(typeof(ExceptionView), ex);

            throw;
        }
    }
}