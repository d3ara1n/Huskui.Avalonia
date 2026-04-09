using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Huskui.Avalonia.Controls;

public class TimelineControl : ItemsControl
{
    public static readonly StyledProperty<object?> PastContentProperty = AvaloniaProperty.Register<
        TimelineControl,
        object?
    >(nameof(PastContent));

    public object? PastContent
    {
        get => GetValue(PastContentProperty);
        set => SetValue(PastContentProperty, value);
    }

    public static readonly StyledProperty<object?> NowContentProperty = AvaloniaProperty.Register<
        TimelineControl,
        object?
    >(nameof(NowContent));

    public object? NowContent
    {
        get => GetValue(NowContentProperty);
        set => SetValue(NowContentProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<TimelineControl, IDataTemplate?>(nameof(HeaderTemplate));

    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    protected override bool NeedsContainerOverride(
        object? item,
        int index,
        out object? recycleKey
    ) => NeedsContainer<TimelineItem>(item, out recycleKey);

    protected override void PrepareContainerForItemOverride(
        Control container,
        object? item,
        int index
    )
    {
        // base() 会设置 Header/Content = item，但是会错误的设置 HeaderTemplate = ItemTemplate 和 ItemTemplate = null
        // 因此这里手动设置而不是调用 base() 避免依赖内部方法
        // base.PrepareContainerForItemOverride(container, item, index);

        if (container is TimelineItem timelineItem)
        {
            timelineItem.Header = item;
            timelineItem.Content = item;
            timelineItem.HeaderTemplate = HeaderTemplate;
            timelineItem.ContentTemplate = ItemTemplate;
        }
    }

    protected override Control CreateContainerForItemOverride(
        object? item,
        int index,
        object? recycleKey
    ) => new TimelineItem();

    protected override void ClearContainerForItemOverride(Control container) =>
        base.ClearContainerForItemOverride(container);
}
