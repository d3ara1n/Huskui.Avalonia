using Avalonia;
using Avalonia.Threading;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Models;

namespace Huskui.Gallery.Drawers;

public partial class UploadQueueDrawer : Drawer
{
    private const long MinChunkBytes = 60_000;
    private const long MaxChunkBytes = 160_000;

    private readonly IReadOnlyList<ProjectFile> _files;
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(120) };

    public UploadQueueDrawer()
        : this([]) { }

    public UploadQueueDrawer(IReadOnlyList<ProjectFile> files)
    {
        InitializeComponent();

        _files = files;
        QueueList.ItemsSource = files;
        _timer.Tick += OnTimerTick;
        UpdateState();
    }

    public event EventHandler<ProjectFile>? FileUploaded;

    private bool IsFinished => _files.All(it => it.IsCompleted);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (!IsFinished)
        {
            _timer.Start();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _timer.Stop();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var current = _files.FirstOrDefault(it => !it.IsCompleted);
        if (current is null)
        {
            return;
        }

        var chunk = Random.Shared.NextInt64(MinChunkBytes, MaxChunkBytes);
        current.Progress = Math.Min(100d, current.Progress + 100d * chunk / current.Bytes);

        if (current.IsCompleted)
        {
            FileUploaded?.Invoke(this, current);
        }

        UpdateState();
    }

    private void UpdateState()
    {
        var completed = _files.Count(it => it.IsCompleted);

        if (completed == _files.Count)
        {
            _timer.Stop();
            Title = $"{completed} files uploaded";
            IsDismissable = true;
        }
        else
        {
            Title = $"Uploading {completed + 1} of {_files.Count}";
            IsDismissable = false;
        }
    }
}
