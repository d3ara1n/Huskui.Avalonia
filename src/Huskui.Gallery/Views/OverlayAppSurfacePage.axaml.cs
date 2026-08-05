using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class OverlayAppSurfacePage : ControlPage
{
    private const string DocumentMarkdown = """
        # Overlay system architecture

        `AppSurface` is the root container of Huskui's overlay and floating-layer system. It owns all hosts used to present floating UI above the main content.

        ## Host responsibilities

        `AppSurface` contains multiple host types with different responsibilities:

        - `OverlayHost`: the shared host for **modal overlay controls**
          - `Dialog`
          - `Modal`
          - `Sidebar`
          - `Toast`
        - `GrowlHost`: the dedicated host for **non-modal growl notifications**
        - `DrawerHost`: the dedicated host for **non-modal drawers**

        In other words:

        - modal overlays are hosted by `OverlayHost`
        - non-modal floating controls use their own specialized hosts
        - `AppSurface` is the root that coordinates all of them

        Modal overlays share a common host because they temporarily obstruct the current interaction flow. Non-modal floating controls do not participate in the same interaction model, so they use specialized hosts with behavior tailored to their own UX patterns.

        If a view is not inside an `AppSurface`, calling `PopDialog(...)`, `PopModal(...)`, `PopGrowl(...)`, or similar APIs has nowhere to render.

        ## Desktop: use `AppWindow`

        On desktop, the recommended root is `AppWindow`. Its template already contains an `AppSurface`, so your page tree automatically gets overlay support.

        ```xml
        <husk:AppWindow
            xmlns="https://github.com/avaloniaui"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:gallery="clr-namespace:Huskui.Gallery;assembly=Huskui.Gallery"
            xmlns:husk="https://github.com/d3ara1n/Huskui.Avalonia">
            <gallery:AppView />
        </husk:AppWindow>
        ```

        Use this when you are running with `IClassicDesktopStyleApplicationLifetime` and want window chrome, desktop drag behavior, and built-in overlay hosting.

        ## Browser and mobile: make `AppSurface` the root view

        Browser and mobile shells usually run with `ISingleViewApplicationLifetime` or `IActivityApplicationLifetime`. In those environments there is no `AppWindow`, so the root view itself must be an `AppSurface`.

        ```xml
        <husk:AppSurface
            x:Class="MyApp.MainView"
            xmlns="https://github.com/avaloniaui"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:local="clr-namespace:MyApp"
            xmlns:husk="https://github.com/d3ara1n/Huskui.Avalonia">
            <local:AppView />
        </husk:AppSurface>
        ```

        This is the same pattern used by the Gallery for browser and mobile hosts.

        ## Showing an overlay from a page or control

        Resolve the nearest surface from the visual tree, then pop the floating control from that surface.

        ```csharp
        using Huskui.Avalonia.Controls;

        private void ShowDialog()
        {
            var appSurface = AppSurface.GetAppSurface(this);
            if (appSurface is null)
            {
                return;
            }

            appSurface.PopDialog(new Dialog
            {
                Title = "Delete file?",
                Content = "This action cannot be undone.",
            });
        }
        ```

        `AppSurface.GetAppSurface(control)` first walks up the visual tree looking for a local `AppSurface`. If it does not find one, it falls back to the `AppWindow.AppSurface` of the current top-level window.

        ## Recommended composition rules

        - Desktop window root: `AppWindow`.
        - Browser/mobile root: `AppSurface`.
        - Place your routed content, shell, or main layout *inside* that root.
        - Trigger overlays from controls that live under the same root surface.
        - Create one overlay root per top-level host. If you open another desktop window, that window should also use its own `AppWindow` or `AppSurface`.

        ## Common mistakes

        - Using a plain `Window` on desktop, then expecting Huskui overlays to appear.
        - Returning a plain `UserControl` as the root view in browser/mobile.
        - Calling overlay APIs before the control is attached to the visual tree.
        - Holding a stale `AppSurface` reference after moving content to another window or host.

        ## Quick rule of thumb

        If you want Huskui floating layers to work, make sure the visible app tree is hosted by `AppSurface`.

        - Desktop: `AppWindow` gives you that automatically.
        - Browser/mobile: wrap the root view in `AppSurface` yourself.
        """;

    public OverlayAppSurfacePage()
    {
        InitializeComponent();
        Viewer.Markdown = DocumentMarkdown;
    }
}
