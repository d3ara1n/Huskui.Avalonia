# Technology Stack

## Framework & Runtime
- **.NET 9.0** - Target framework for both library and gallery
- **C# Preview Language Version** - Uses latest C# features
- **Avalonia UI 11.3.0+** - Cross-platform UI framework

## Key Dependencies
- **FluentIcons.Avalonia** - Icon system integration
- **CommunityToolkit.Mvvm** - MVVM patterns (Gallery only)
- **Microsoft.Extensions.DependencyInjection** - DI container (Gallery only)
- **DynamicData** - Reactive data handling (Gallery only)

## Development Tools
- **GitVersion** - Semantic versioning
- **git-cliff** - Changelog generation using conventional commits
- **HotAvalonia** - Hot reload for development (Debug only)
- **JetBrains.Annotations** - Code analysis annotations
- **Microsoft.SourceLink.GitHub** - Source debugging support

## Build & Package Configuration
- **NuGet Package**: Huskui.Avalonia with comprehensive metadata
- **Compiled Bindings**: Enabled by default for performance
- **Nullable Reference Types**: Enabled
- **Implicit Usings**: Enabled

## Common Commands

### Build
```bash
dotnet build
```

### Run Gallery Application
```bash
dotnet run --project src/Huskui.Gallery
```

### Package Library
```bash
dotnet pack src/Huskui.Avalonia
```

### Generate Changelog
```bash
git cliff
```

## Development Environment
- Uses **app.manifest** for Windows-specific settings
- **HotAvalonia** enabled for debug builds
- **Avalonia.Diagnostics** available in debug mode