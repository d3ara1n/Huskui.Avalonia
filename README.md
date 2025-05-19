# Huskui.Avalonia

Huskui.Avalonia is a modern, elegant UI component library for [Avalonia UI](https://avaloniaui.net/), designed to provide a comprehensive set of customizable controls for building beautiful cross-platform desktop applications.

## Features

- **Rich Component Library**: Includes a wide range of UI components like AppWindow, Card, InfoBar, Tag, IconLabel, and more
- **Consistent Design Language**: All components follow a cohesive design system with shared colors, animations, and behaviors
- **Theming Support**: Built-in support for light and dark themes
- **Fluent Icons Integration**: Uses FluentIcons.Avalonia for consistent iconography
- **Modern UI Elements**: Includes modern UI patterns like overlays, notifications, and dialogs
- **Customizable**: Easily customize the appearance of components through XAML styles and themes

## Components

Huskui.Avalonia includes the following components:

- **AppWindow**: Enhanced window with built-in support for overlays, toasts, modals, and notifications
- **Button**: Styled buttons with multiple variants (normal, outline, ghost) and states (primary, success, warning, danger)
- **Card**: Container for grouping related content
- **InfoBar**: Informational message bars with different severity levels
- **Tag**: Compact labels for categorization and metadata
- **IconLabel**: Combined icon and text label
- **TextBox**: Enhanced text input with support for inner content
- **ComboBox**: Dropdown selection control
- **Dialog**: Modal dialog boxes
- **Divider**: Line separators for content sections
- **Frame**: Navigation container with transition animations
- **Page**: Content pages for use with Frame
- **ProgressBar** and **ProgressRing**: Loading indicators
- **SkeletonContainer**: Loading placeholder for content
- **NotificationHost** and **NotificationItem**: Toast notification system
- **OverlayHost** and **OverlayItem**: Overlay management system
- And many more...

## Getting Started

### Prerequisites

- .NET 9.0 or later
- Avalonia UI 11.3.0 or later

### Installation

1. Add a reference to the Huskui.Avalonia project in your solution, or
2. Install the package from NuGet (when available)

### Basic Usage

1. Add the Huskui.Avalonia namespace to your XAML:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:husk="https://github.com/d3ara1n/Huskui.Avalonia"
             x:Class="YourApp.App">
    <Application.Styles>
        <FluentTheme />
        <StyleInclude Source="avares://Huskui.Avalonia/Prelude.axaml" />
    </Application.Styles>
</Application>
```

2. Use Huskui components in your views:

```xml
<husk:AppWindow xmlns="https://github.com/avaloniaui"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                xmlns:husk="https://github.com/d3ara1n/Huskui.Avalonia"
                xmlns:fi="clr-namespace:FluentIcons.Avalonia;assembly=FluentIcons.Avalonia"
                x:Class="YourApp.MainWindow"
                Title="Your App">
    <Grid RowDefinitions="Auto,*" Margin="24">
        <husk:Card Grid.Row="0" Margin="0,0,0,12">
            <husk:InfoBar Header="Welcome" Content="This is a sample application using Huskui.Avalonia" />
        </husk:Card>

        <StackPanel Grid.Row="1" Spacing="12">
            <husk:IconLabel Icon="Home" Text="Home" />

            <husk:Button Content="Standard Button" />
            <husk:Button Classes="Primary" Content="Primary Button" />
            <husk:Button Classes="Success" Content="Success Button" />

            <husk:Tag Content="Sample Tag" />
        </StackPanel>
    </Grid>
</husk:AppWindow>
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the [MIT License](LICENSE).