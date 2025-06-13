# Huskui.Avalonia

Huskui.Avalonia is a modern, elegant UI component library for [Avalonia UI](https://avaloniaui.net/), designed to
provide a comprehensive set of customizable controls for building beautiful cross-platform desktop applications.
Inspired by [ParkUI](https://park-ui.com/) and using the [Radix Colors](https://www.radix-ui.com/colors) palette.

## Features

- **Rich Component Library**: Includes a wide range of UI components like AppWindow, Card, InfoBar, Tag, IconLabel, and
  more
- **Consistent Design Language**: All components follow a cohesive design system with shared colors, animations, and
  behaviors
- **Theming Support**: Built-in support for light and dark themes
- **Fluent Icons Integration**: Uses FluentIcons.Avalonia for consistent iconography
- **Modern UI Elements**: Includes modern UI patterns like overlays, notifications, and dialogs
- **Customizable**: Easily customize the appearance of components through XAML styles and themes

## Components

Huskui.Avalonia includes the following components:

- **AppWindow**: Enhanced window with built-in support for overlays, toasts, modals, and notifications
- **Card**: Container for grouping related content with consistent styling
- **InfoBar**: Informational message bars with different severity levels
- **Tag**: Compact labels for categorization and metadata
- **IconLabel**: Combined icon and text label with FluentIcons integration
- **Frame**: Navigation container with transition animations
- **HighlightBlock**: Text highlighting for code snippets and keyboard shortcuts
- **NotificationHost** and **NotificationItem**: Toast notification system
- **OverlayHost** and **Modals/Dialogs/Drawers/Toasts**: Overlay management system
- **SkeletonContainer**: Loading placeholder for content
- **BusyContainer**: Container with loading state management
- **LazyContainer**: Component for deferred loading of content
- And many more...

## Getting Started

### Prerequisites

- .NET 9.0 or later
- Avalonia UI 11.3.0 or later

### Installation

`dotnet add package Huskui.Avalonia`

### Basic Usage

1. Add the Huskui.Avalonia namespace to your XAML:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:husk="https://github.com/d3ara1n/Huskui.Avalonia"
             x:Class="YourApp.App">
    <Application.Styles>
        <FluentTheme />
        <husk:HuskuiTheme Accent="Lime" />
    </Application.Styles>
</Application>
```

2. Use Huskui components in your views:

```xml
<husk:AppWindow xmlns="https://github.com/avaloniaui"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                xmlns:husk="https://github.com/d3ara1n/Huskui.Avalonia"
                x:Class="YourApp.MainWindow"
                Title="Your App">
    <Grid RowDefinitions="Auto,*" Margin="24">
        <husk:Card Grid.Row="0" Margin="0,0,0,12">
            <husk:InfoBar Header="Welcome" Content="This is a sample application using Huskui.Avalonia" />
        </husk:Card>

        <StackPanel Grid.Row="1" Spacing="12">
            <husk:IconLabel Icon="Home" Text="Home" />

            <Button Content="Standard Button" />
            <Button Classes="Primary" Content="Primary Button" />
            <Button Classes="Success" Content="Success Button" />

            <husk:Tag Content="Sample Tag" />
        </StackPanel>
    </Grid>
</husk:AppWindow>
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the [MIT License](LICENSE).