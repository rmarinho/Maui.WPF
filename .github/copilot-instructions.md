# Copilot Instructions for Platform.Maui.WPF

## Project Overview
This is a custom .NET MAUI backend targeting WPF (Windows Presentation Foundation). It allows MAUI apps to run on WPF instead of WinUI, using the platform-agnostic MAUI NuGet packages (`net10.0` assemblies) with custom handler implementations that bridge MAUI's layout/rendering system to WPF controls.

## Architecture
- **Handler pattern**: Each handler has two files:
  - Shared (`Handler.cs`): PropertyMapper/CommandMapper definitions
  - Platform-specific (`.Windows.cs`): `WPFViewHandler<TVirtualView, TPlatformView>` subclass with CreatePlatformView and Map* methods
- **Base class**: `WPFViewHandler<TVirtualView, TPlatformView>` in `ViewHandlerOfT.Windows.cs`
- **Type aliasing**: WPF and MAUI share type names (Button, CheckBox, etc.) — always use `using` aliases like `WButton = System.Windows.Controls.Button`

## Key Technical Details
- `IButton` has no `.Text` — cast to `IText`; no `.TextColor` — cast to `ITextStyle`
- `IStepper.Interval` (not `.Increment`) for step size
- `ICheckBox.Foreground` returns `Paint` not `Color` — check for `SolidPaint`
- `ITimePicker.Time` is `TimeSpan?` (nullable) in MAUI 10
- `ViewHandler.VirtualView` throws when null — use try/catch, not null-conditional
- WPF `MinHeight`/`MinWidth` cannot be NaN — guard with `!double.IsNaN()`
- Use `global::System.Windows.CornerRadius` to disambiguate from `Microsoft.Maui.CornerRadius`

## Color Conversion Pattern
```csharp
static System.Windows.Media.SolidColorBrush? ToBrush(Microsoft.Maui.Graphics.Color? color)
{
    if (color == null) return null;
    return new System.Windows.Media.SolidColorBrush(
        System.Windows.Media.Color.FromArgb((byte)(color.Alpha * 255),
            (byte)(color.Red * 255), (byte)(color.Green * 255), (byte)(color.Blue * 255)));
}
```

## Building
```bash
dotnet build build.slnf                          # Build libraries only
dotnet build samples\Maui.Controls.Sample\Maui.Controls.Sample.csproj  # Build sample
dotnet run --project samples\Maui.Controls.Sample\Maui.Controls.Sample.csproj  # Run sample
```

## Project Structure
```
src/
  Platform.Maui.WPF/              # Main WPF backend library
  Platform.Maui.Essentials.WPF/   # Essentials services
samples/
  Maui.Controls.Sample/           # WPF Control Gallery
  Maui.Controls.Sample.Blazor/    # Blazor Hybrid sample
  Maui.Sample/                    # Multi-platform sample
```

## When Adding New Handlers
1. Create `HandlerName.cs` with PropertyMapper entries
2. Create `HandlerName.Windows.cs` with WPF implementation extending `WPFViewHandler<T, TPlatform>`
3. Register in `AppHostBuilderExtensions.AddMauiControlsHandlers()`
4. Use type aliases to avoid WPF/MAUI ambiguity
5. Update `BACKEND_IMPLEMENTATION_CHECKLIST.md`
