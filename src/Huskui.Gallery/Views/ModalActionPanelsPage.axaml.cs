using System;
using System.Collections.Generic;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class ModalActionPanelsPage : ControlPage
{
    public ModalActionPanelsPage()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>供 Layout 切换器使用的枚举列表。</summary>
    public IReadOnlyList<ModalActionPanel.LayoutMode> LayoutOptions { get; } =
        [.. Enum.GetValues<ModalActionPanel.LayoutMode>()];

    /// <summary>供 PrimaryPlacement 切换器使用的枚举列表。</summary>
    public IReadOnlyList<ModalActionPanel.PrimaryPlacementMode> PrimaryPlacementOptions { get; } =
        [.. Enum.GetValues<ModalActionPanel.PrimaryPlacementMode>()];
}
