# .NET MAUI Backend for WPF

[![NuGet](https://img.shields.io/nuget/v/Platform.Maui.WPF.svg?label=Platform.Maui.WPF)](https://www.nuget.org/packages/Platform.Maui.WPF/)
[![NuGet](https://img.shields.io/nuget/v/Platform.Maui.Essentials.WPF.svg?label=Platform.Maui.Essentials.WPF)](https://www.nuget.org/packages/Platform.Maui.Essentials.WPF/)
[![Build](https://github.com/rmarinho/maui.wpf/actions/workflows/build.yml/badge.svg)](https://github.com/rmarinho/maui.wpf/actions/workflows/build.yml)

Custom .NET MAUI backend targeting WPF (Windows Presentation Foundation) — an alternative to the official WinUI backend.

This backend uses the platform-agnostic MAUI NuGet packages (`net10.0` fallback assemblies) and provides custom handler implementations that bridge MAUI's layout/rendering system to WPF controls.

> **Inspiration:** This project follows the patterns established by [mauiplatforms](https://github.com/Redth/mauiplatforms) (macOS/tvOS backends) and [Maui.Gtk](https://github.com/AathifMahir/Maui.Gtk).

## Quick Start — Setting Up a WPF MAUI App

### 1. Create the project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <OutputType>WinExe</OutputType>
    <UseMaui>true</UseMaui>
    <UseWPF>true</UseWPF>
    <EnableDefaultXamlItems>false</EnableDefaultXamlItems>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Maui.Controls" Version="10.0.31" />
    <PackageReference Include="Platform.Maui.WPF" Version="*" />
    <PackageReference Include="Platform.Maui.Essentials.WPF" Version="*" />
  </ItemGroup>
</Project>
```

### 2. App.xaml

```xml
<Platform.Maui.WPF:MauiWPFApplication
    x:Class="MyApp.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:Platform.Maui.WPF="clr-namespace:Microsoft.Maui.Platform.WPF;assembly=Platform.Maui.WPF">
</Platform.Maui.WPF:MauiWPFApplication>
```

### 3. App.xaml.cs

```csharp
using Microsoft.Maui.Platform.WPF;

namespace MyApp;

public partial class App : MauiWPFApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

### 4. MauiProgram.cs

```csharp
using Microsoft.Maui.Platform.WPF.Hosting;
using Microsoft.Maui.Essentials.WPF;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiAppWPF<App>()
            .UseWPFEssentials()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        return builder.Build();
    }
}
```

### 5. App class

```csharp
public class App : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(new MainPage());
}
```

## Samples

See the `samples/` directory for working examples:
- **Maui.Controls.Sample** — Full control gallery exercising all implemented handlers
- **Maui.Controls.Sample.Blazor** — Blazor Hybrid WebView on WPF
- **Maui.Sample** — Multi-platform sample with WPF platform folder

## Handlers Implemented

### Controls

| Control | WPF Native Control | Status |
|---|---|---|
| Label | TextBlock | ✅ Full (Text, FormattedText, Font, Color, Alignment, Decorations, LineHeight, MaxLines, Padding) |
| Button | Button | ✅ Full (Text, Font, TextColor, Background, Stroke, Clicked/Pressed/Released) |
| Entry | TextBox | ✅ Full (Text, Font, Color, Placeholder, MaxLength, IsReadOnly, Keyboard, Alignment) |
| Editor | TextBox (multiline) | ✅ Full (Text, Font, Color, MaxLength, IsReadOnly, Alignment) |
| Image | Image | ✅ (Source, Aspect) |
| CheckBox | CheckBox | ✅ (IsChecked, Color) |
| Switch | ToggleButton (custom template) | ✅ (IsToggled, OnColor, ThumbColor — animated thumb) |
| Slider | Slider | ✅ (Value, Min, Max, MinimumTrackColor, MaximumTrackColor) |
| ProgressBar | ProgressBar | ✅ (Progress, ProgressColor) |
| ActivityIndicator | ProgressBar (indeterminate) | ✅ (IsRunning, Color) |
| Picker | ComboBox | ✅ (Items, SelectedIndex, Title, Font, Background, Alignment) |
| DatePicker | DatePicker | ✅ (Date, MinDate, MaxDate, Font, TextColor) |
| TimePicker | TextBox | ✅ (Time, Font, CharacterSpacing) |
| Stepper | StackPanel (−/+/label) | ✅ (Value, Min, Max, Increment) |
| SearchBar | TextBox | ✅ (Text, Placeholder, Font, MaxLength, Alignment) |
| Border | Border | ✅ (Stroke, StrokeThickness, Background, Padding, Content) |
| BoxView | Border | ✅ (Color, CornerRadius, Background) |
| ScrollView | ScrollViewer | ✅ (Content, Orientation) |
| ContentView | ContentPanel | ✅ (Content) |
| ShapeView | Canvas shapes | ⚠️ Basic |
| BlazorWebView | WebView2 | ✅ (via AspNetCore.Components.WebView.Wpf) |

### Pages & Navigation

| Page | Status | Notes |
|---|---|---|
| ContentPage | ✅ | Full support |
| NavigationPage | ❌ | Not yet — use `Application.Current.Windows[0].Page = new Page()` |
| TabbedPage | ❌ | Planned |
| FlyoutPage | ❌ | Planned |
| Shell | ❌ | Planned |
| Modal pages | ❌ | Planned |

### Layouts

All layouts work: `VerticalStackLayout`, `HorizontalStackLayout`, `Grid`, `FlexLayout`, `AbsoluteLayout`, `ScrollView`, `ContentView`, `Border`.

### Gestures

| Gesture | Status |
|---|---|
| Tap | ❌ Planned |
| Pan | ❌ Planned |
| Swipe | ❌ Planned |
| Pinch | ❌ Planned |
| Pointer (Hover) | ❌ Planned |

### Infrastructure

| Component | Status | Notes |
|---|---|---|
| Application | ✅ | MauiWPFApplication base class |
| Window | ✅ | Title, Width, Height, Position, Min/Max sizes |
| Dispatcher | ✅ | WPF Dispatcher |
| DispatcherTimer | ✅ | |
| Dialogs (Alert, Confirm, Prompt) | ❌ | Planned — will use WPF MessageBox/custom dialogs |
| Font Management | ⚠️ | System fonts only — custom font loading planned |
| Dark/Light Mode | ❌ | Planned |
| Animations | ❌ | Planned — PlatformTicker needed |
| FormattedText / Spans | ✅ | NSAttributedString-equivalent via TextBlock.Inlines |
| Transforms | ❌ | Planned — RenderTransform mapping |
| Shadow / Clip | ❌ | Planned |
| MenuBar | ❌ | Planned — WPF native menu support |
| AutomationId | ❌ | Planned |

### Essentials

| API | Status | Notes |
|---|---|---|
| AppInfo | ✅ | Package name, version, build, theme |
| DeviceInfo | ✅ | Model, manufacturer, OS, platform, idiom |
| Connectivity | ✅ | Network status, profiles, change events |
| DeviceDisplay | ✅ | Screen dimensions, density, orientation |
| FileSystem | ✅ | Cache/app data directories, package files |
| Preferences | ✅ | Key/value via registry |
| SecureStorage | ✅ | DPAPI-encrypted storage |
| Clipboard | ✅ | WPF Clipboard |
| Browser | ✅ | Process.Start URL |
| Launcher | ✅ | Process.Start |
| Share | ✅ | WPF sharing |
| FilePicker | ✅ | Win32 OpenFileDialog |
| Email | ✅ | mailto: protocol |
| Map | ✅ | Bing Maps URL |
| VersionTracking | ✅ | Preferences-backed |
| Battery | ❌ | Stub |
| Vibration | ❌ | N/A (desktop) |
| Accelerometer | ❌ | N/A (desktop) |
| Gyroscope | ❌ | N/A (desktop) |
| Compass | ❌ | N/A (desktop) |
| Geolocation | ❌ | Stub |
| Geocoding | ❌ | Stub |
| TextToSpeech | ❌ | Planned (System.Speech) |
| Haptics | ❌ | N/A (desktop) |
| Flashlight | ❌ | N/A (desktop) |
| Barometer | ❌ | N/A (desktop) |

## Project Structure

```
src/
  Platform.Maui.WPF/              # WPF backend library (net10.0-windows)
  Platform.Maui.Essentials.WPF/   # WPF Essentials library
samples/
  Maui.Controls.Sample/           # WPF Control Gallery
  Maui.Controls.Sample.Blazor/    # Blazor Hybrid sample
  Maui.Sample/                    # Multi-platform sample
```

## Prerequisites

### .NET 10 SDK

Install the latest .NET 10 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/10.0).

### Workloads

```bash
dotnet workload install maui-windows
dotnet workload list   # verify
```

## Building & Running

```bash
# Build libraries only (for NuGet packaging)
dotnet build build.slnf -property:Configuration=Release

# Build sample
dotnet build samples\Maui.Controls.Sample\Maui.Controls.Sample.csproj

# Run sample
dotnet run --project samples\Maui.Controls.Sample\Maui.Controls.Sample.csproj
```

> **Note:** Build projects individually or use `build.slnf`. The full solution includes samples that may have additional dependencies.

## Key Technical Notes

- MAUI NuGet packages resolve to the `net10.0` (platform-agnostic) assembly. `ToPlatform()` returns `object` — custom handler base class casts to the WPF native view type.
- The platform-agnostic `ViewHandler` has no-op `PlatformArrange` and returns `Size.Zero`. `WPFViewHandler` overrides these to bridge MAUI layout to WPF `Measure`/`Arrange`.
- WPF `System.Windows.Controls` and MAUI `Microsoft.Maui.Controls` share many type names — every handler file uses `using` aliases to disambiguate.
- Sample apps use XAML pages. The `MauiWPFApplication` base class in `App.xaml` bootstraps the MAUI runtime within a WPF `Application`.
- WPF default Button `ControlTemplate` has system chrome that overrides `Background` — some visual properties require custom templates for full fidelity.

## Dialogs

Dialogs (`DisplayAlertAsync`, `DisplayPromptAsync`) are **not yet supported**. This is a P0 priority item — implementation will use WPF `MessageBox` for alerts and custom WPF `Window` for prompts.

## TODO

See [BACKEND_IMPLEMENTATION_CHECKLIST.md](BACKEND_IMPLEMENTATION_CHECKLIST.md) for the full implementation status.

### P0 — Critical
- NavigationPage handler (push/pop stack, back button, toolbar)
- DisplayAlert / DisplayActionSheet / DisplayPromptAsync
- TapGestureRecognizer
- CollectionView / ListView
- PlatformTicker + Animations

### P1 — Important
- Shell navigation (flyout, tabs, routing)
- TabbedPage / FlyoutPage
- All gesture recognizers (Pointer, Pan, Swipe, Pinch)
- IFontManager / IFontRegistrar (custom fonts)
- MenuBar (WPF native menu)
- Transforms (TranslationX/Y, Scale, Rotation) via RenderTransform
- Shadow / Clip
- WebView via WebView2

### Broader Goals
- NuGet packaging (CI/CD via GitHub Actions)
- Multi-window support
- Accessibility / AutomationId
- Drag and drop

## License

MIT
