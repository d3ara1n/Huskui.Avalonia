# Project Structure

## Solution Organization
```
Huskui.Avalonia/
├── src/
│   ├── Huskui.Avalonia/          # Main UI component library
│   └── Huskui.Gallery/           # Demo/documentation application
├── .kiro/                        # Kiro AI assistant configuration
├── .github/                      # GitHub workflows and templates
└── [config files]               # Root-level configuration
```

## Library Project (src/Huskui.Avalonia/)
```
Huskui.Avalonia/
├── Controls/                     # UI control implementations
├── Converters/                   # Value converters for data binding
├── Models/                       # Data models and DTOs
├── Properties/                   # Assembly properties
├── Themes/                       # XAML theme resources
├── Transitions/                  # Animation and transition effects
├── AccentColor.cs               # Color system definitions
├── CornerStyle.cs               # Style enumerations
├── HuskuiTheme.axaml            # Main theme resource dictionary
├── HuskuiTheme.axaml.cs         # Theme code-behind
└── IPageModel.cs                # Interface definitions
```

## Gallery Project (src/Huskui.Gallery/)
```
Huskui.Gallery/
├── Assets/                       # Images, icons, and resources
├── Controls/                     # Custom gallery-specific controls
├── Dialogs/                      # Dialog implementations
├── Modals/                       # Modal window implementations
├── Models/                       # Gallery data models
├── Services/                     # Business logic services
├── Styles/                       # Gallery-specific styles
├── Themes/                       # Gallery theme overrides
├── Toasts/                       # Toast notification implementations
├── ViewModels/                   # MVVM view models
├── Views/                        # XAML views and pages
├── App.axaml                     # Application definition
├── App.axaml.cs                  # Application code-behind
└── Program.cs                    # Application entry point
```

## Architecture Patterns
- **Library**: Pure UI component library with minimal dependencies
- **Gallery**: MVVM pattern with dependency injection
- **Separation**: Clear separation between library and demo code
- **Theming**: Centralized theme system with AXAML resource dictionaries
- **Controls**: Each control typically has its own folder with implementation and styles

## File Naming Conventions
- **AXAML files**: PascalCase with .axaml extension
- **Code-behind**: Matching .axaml.cs files
- **ViewModels**: Suffix with "ViewModel"
- **Pages**: Suffix with "Page" for gallery views
- **Services**: Descriptive names ending in "Service"