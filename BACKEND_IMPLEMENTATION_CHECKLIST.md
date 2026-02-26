# Platform.Maui.WPF — Backend Implementation Checklist

> A comprehensive checklist for implementing .NET MAUI on WPF, modeled after the [Backend Implementation Checklist Template](https://gist.github.com/Redth/1b673814c06b0d4ea3b9c1229498230a).

---

## 1. Core Infrastructure

### Platform Abstractions

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **Base View Handler** | ✅ | `WPFViewHandler<TVirtualView, TPlatformView>` in `ViewHandlerOfT.Windows.cs` — bridges MAUI layout to WPF `Measure`/`Arrange` |
| [x] **IMauiContext** | ✅ | `WPFMauiContext` wraps WPF `System.Windows.Window` |
| [x] **IDispatcher / IDispatcherProvider** | ✅ | `WPFDispatcherProvider` uses `System.Windows.Threading.Dispatcher` |
| [x] **Handler Factory** | ✅ | Standard MAUI `ConfigureMauiHandlers` in `AppHostBuilderExtensions` |
| [x] **App Host Builder** | ✅ | `UseMauiAppWPF<TApp>()` extension method |
| [ ] **IPlatformApplication** | ❌ | Not yet abstracted — `MauiWPFApplication` serves as both WPF `Application` and MAUI bridge |

### Rendering Pipeline

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **Property Change Propagation** | ✅ | Property mappers per handler + `ViewMapper` overrides for base `IView` properties |
| [x] **Child View Sync** | ✅ | `LayoutPanel` custom WPF `Panel` syncs MAUI children to WPF visual tree |
| [x] **Measurement** | ✅ | `GetDesiredSize()` in base handler bridges MAUI `IView.Measure()` → WPF `Measure()` |
| [x] **Arrangement** | ✅ | `PlatformArrange()` calls WPF `Arrange()` with MAUI-computed bounds |

### Native Interop

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **Color Conversion** | ✅ | `Microsoft.Maui.Graphics.Color` → `System.Windows.Media.SolidColorBrush` helper in every handler |
| [x] **Thickness Conversion** | ✅ | `Microsoft.Maui.Thickness` → `System.Windows.Thickness` |
| [ ] **Transform Conversion** | ❌ | `TranslationX/Y`, `Scale`, `Rotation` → WPF `RenderTransform` not yet wired |

> **MAUI Source Reference:**
> - Core handler infrastructure: [`src/Core/src/Handlers/`](https://github.com/dotnet/maui/tree/main/src/Core/src/Handlers)
>
> **WPF Implementation Note:**
> Type ambiguity between `System.Windows.Controls.*` and `Microsoft.Maui.Controls.*` is pervasive. Every handler file MUST use `using` aliases (e.g., `WThickness = System.Windows.Thickness`).

---

## 2. Application & Window

### Application

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **ApplicationHandler** | ✅ | `ApplicationHandler.cs` — maps `IApplication` to WPF `Application` |
| [x] **CreateWindow** | ✅ | Creates WPF `System.Windows.Window` from `IApplication.CreateWindow()` |
| [ ] **ThemeChanged** | ❌ | No system theme detection yet |
| [ ] **OpenWindow / CloseWindow** | ❌ | Multi-window not supported |

### Window

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **WindowHandler** | ✅ | `WindowHandler.cs` — full implementation |
| [x] **Title** | ✅ | Maps `IWindow.Title` → `System.Windows.Window.Title` |
| [x] **Width / Height** | ✅ | Maps to WPF `Width`/`Height` |
| [x] **X / Y** | ✅ | Maps to WPF `Left`/`Top` |
| [x] **MinimumWidth / MinimumHeight** | ✅ | Mapped |
| [x] **MaximumWidth / MaximumHeight** | ✅ | Mapped |
| [ ] **Page (Content)** | ⚠️ | Sets content but doesn't track page changes dynamically |
| [ ] **MenuBar** | ❌ | Not implemented |
| [ ] **Multi-window** | ❌ | Single window only |

> **MAUI Source Reference:**
> - [`WindowHandler`](https://github.com/dotnet/maui/blob/main/src/Core/src/Handlers/Window/WindowHandler.cs)

---

## 3. Pages

| Page Type | Status | Notes |
|-----------|--------|-------|
| [x] **ContentPage** | ✅ | `PageHandler` — maps `IContentView.PresentedContent` to WPF `ContentControl` |
| [x] **NavigationPage** | ✅ | `NavigationViewHandler` — `DockPanel` with toolbar (back button, title, toolbar items) + `ContentControl` content area |
| [x] **TabbedPage** | ✅ | `TabbedViewHandler` — uses WPF `TabControl` with auto-generated `TabItem`s |
| [x] **FlyoutPage** | ✅ | `FlyoutViewHandler` — WPF `Grid` with 3 columns (flyout \| `GridSplitter` \| detail) |
| [ ] **Shell** | ❌ | Major gap — no flyout/tabs/URI navigation from Shell |
| [ ] **ModalPage** | ❌ | `PushModalAsync`/`PopModalAsync` not hooked |

> **MAUI Source Reference:**
> - [`NavigationViewHandler`](https://github.com/dotnet/maui/blob/main/src/Controls/src/Core/Handlers/NavigationPage/)
> - [`TabbedViewHandler`](https://github.com/dotnet/maui/blob/main/src/Controls/src/Core/Handlers/TabbedPage/)
> - [`FlyoutViewHandler`](https://github.com/dotnet/maui/blob/main/src/Controls/src/Core/Handlers/FlyoutPage/)
>
> **Existing Implementations:**
> - macOS: `NSTabViewController`, custom `NSSplitViewController`
> - GTK4: `Gtk.Notebook`, `Gtk.Paned`

---

## 4. Layouts

| Layout | Status | Notes |
|--------|--------|-------|
| [x] **VerticalStackLayout** | ✅ | Via `LayoutHandler` + `LayoutPanel` |
| [x] **HorizontalStackLayout** | ✅ | Via `LayoutHandler` + `LayoutPanel` |
| [x] **Grid** | ✅ | Cross-platform MAUI layout engine — `LayoutPanel` respects computed bounds |
| [x] **FlexLayout** | ✅ | Cross-platform MAUI layout engine |
| [x] **AbsoluteLayout** | ✅ | Cross-platform MAUI layout engine |
| [x] **StackLayout** | ✅ | Legacy — routed through same `LayoutHandler` |
| [x] **ScrollView** | ✅ | `ScrollViewHandler` — WPF `ScrollViewer` wrapping child content |
| [x] **ContentView** | ✅ | `ContentViewHandler` — WPF `ContentControl` |
| [x] **Border** | ✅ | `BorderHandler` — WPF `Border` with `CornerRadius`, `BorderBrush`, `BorderThickness` |
| [ ] **Frame** | ⚠️ | Not registered separately — could reuse `BorderHandler` |

> **Key Concept:** MAUI's layout engine (StackLayout, Grid, FlexLayout, AbsoluteLayout) is entirely cross-platform. The `LayoutHandler` just needs to create a native container, add/remove children, and call `Measure`/`Arrange` using MAUI-computed bounds.

---

## 5. Basic Controls

| Control | Status | Notes |
|---------|--------|-------|
| [x] **Label** | ✅ | Full: `Text`, `TextColor`, `FontSize`, `FontFamily`, `FontAttributes`, `HorizontalTextAlignment`, `VerticalTextAlignment`, `MaxLines`, `LineBreakMode`, `TextDecorations`, `CharacterSpacing`, `Padding`, `FormattedText` |
| [x] **Button** | ✅ | `Text`, `TextColor`, `FontSize`, `Background`, `Padding`, `CornerRadius` (stub), `Command`; WPF chrome overrides Background on some system themes |
| [x] **Image** | ✅ | `FileImageSource` basic loading; `Aspect` (Fill, AspectFit, AspectFill) via `Stretch` |
| [ ] **ImageButton** | ❌ | Not implemented |
| [x] **Entry** | ✅ | `Text`, `TextColor`, `FontSize`, `IsPassword`, `Placeholder`, `MaxLength`, `IsReadOnly`, `Keyboard`, `ReturnType`; missing `PlaceholderColor` watermark |
| [x] **Editor** | ✅ | `Text`, `TextColor`, `FontSize`, `Placeholder`, `MaxLength`, `IsReadOnly`; missing placeholder watermark |
| [x] **Switch** | ✅ | Custom WPF `ControlTemplate` with animated toggle; `IsToggled`, `OnColor`, `TrackColor` (stub), `ThumbColor` (stub) |
| [x] **CheckBox** | ✅ | WPF `CheckBox`; `IsChecked`, `Foreground` (via `SolidPaint`) |
| [x] **Slider** | ✅ | WPF `Slider`; `Value`, `Minimum`, `Maximum`, `MinimumTrackColor`, `MaximumTrackColor`; `ThumbColor` not mapped |
| [x] **Stepper** | ✅ | Custom WPF panel with ▲/▼ buttons; `Value`, `Minimum`, `Maximum`, `Increment` |
| [x] **ProgressBar** | ✅ | WPF `ProgressBar`; `Progress` (0-1 → 0-100) |
| [x] **ActivityIndicator** | ✅ | Custom rotating arc via `DispatcherTimer`; `IsRunning`, `Color` |
| [x] **BoxView** | ✅ | Rendered via `ShapeViewHandler` as filled rectangle |
| [ ] **RadioButton** | ❌ | Not implemented |

> **MAUI Source Reference:**
> - [`LabelHandler`](https://github.com/dotnet/maui/blob/main/src/Core/src/Handlers/Label/LabelHandler.cs)
> - [`ButtonHandler`](https://github.com/dotnet/maui/blob/main/src/Core/src/Handlers/Button/ButtonHandler.cs)

---

## 6. Input & Selection Controls

| Control | Status | Notes |
|---------|--------|-------|
| [x] **Picker** | ✅ | WPF `ComboBox`; `SelectedIndex`, `Items`, `Title`, `TextColor` |
| [x] **DatePicker** | ✅ | WPF `DatePicker`; `Date`, `MinimumDate`, `MaximumDate`, `Format` |
| [x] **TimePicker** | ✅ | WPF `ComboBox` with time items; `Time` (nullable in MAUI 10), `Format` |
| [x] **SearchBar** | ✅ | WPF `TextBox` + `Button`; `Text`, `Placeholder`, `TextColor`; missing search icon and clear button |

---

## 7. Collection Controls

| Control | Status | Notes |
|---------|--------|-------|
| [ ] **CollectionView** | ❌ | **P0 gap** — data-driven list |
| [ ] **ListView** | ❌ | Legacy list control |
| [ ] **CarouselView** | ❌ | Horizontal swipeable collection |
| [ ] **IndicatorView** | ❌ | Page indicator dots |
| [ ] **TableView** | ❌ | Settings-style grouped list |
| [ ] **SwipeView** | ❌ | Swipe-to-reveal actions |
| [ ] **RefreshView** | ❌ | Pull-to-refresh wrapper |

> **Priority:** CollectionView is the most critical missing control. WPF's `ListBox`/`ListView`/`ItemsControl` with `DataTemplate` is a natural fit.

---

## 8. Navigation & Routing

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **Push/Pop** | ✅ | `NavigationViewHandler` handles `IStackNavigationView.RequestNavigation` → stack management → `NavigationFinished()` |
| [x] **Back Button** | ✅ | Auto-shown when navigation stack depth > 1 |
| [x] **Title Bar** | ✅ | `TextBlock` in `DockPanel` toolbar showing current page title |
| [x] **Toolbar Items** | ✅ | Rendered as `Button`s in toolbar `StackPanel` |
| [ ] **Modal Navigation** | ❌ | `PushModalAsync`/`PopModalAsync` not implemented |
| [ ] **Shell Navigation** | ❌ | Full Shell handler not implemented |
| [ ] **URI-based Navigation** | ❌ | Requires Shell |
| [ ] **Animated Transitions** | ❌ | No page push/pop animations |

> **MAUI Source Reference:**
> - [`IStackNavigationView`](https://github.com/dotnet/maui/blob/main/src/Core/src/Handlers/NavigationPage/IStackNavigationView.cs)

---

## 9. Alerts & Dialogs

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **DisplayAlert** | ✅ | `MessageBox.Show()` via `WPFAlertManagerSubscription` `DispatchProxy` |
| [x] **DisplayActionSheet** | ✅ | Custom WPF `Window` with button list |
| [x] **DisplayPromptAsync** | ✅ | Custom WPF `Window` with `TextBox` + OK/Cancel |

> **⚠️ Known Extensibility Gap:** `AlertManager` and `IAlertManagerSubscription` are internal. We use `DispatchProxy` + reflection to intercept dialog requests. See [dotnet/maui#34104](https://github.com/dotnet/maui/issues/34104).

---

## 10. Gesture Recognizers

| Gesture | Status | Notes |
|---------|--------|-------|
| [x] **TapGestureRecognizer** | ✅ | `GestureManager` — `MouseLeftButtonUp` with reflection for `SendTapped` |
| [x] **PointerGestureRecognizer** | ✅ | `GestureManager` — `MouseEnter`/`Leave`/`Move` with reflection for `SendPointerEntered`/`Exited`/`Moved` |
| [ ] **PanGestureRecognizer** | ❌ | Mouse drag tracking |
| [ ] **SwipeGestureRecognizer** | ❌ | Mouse swipe/flick detection |
| [ ] **PinchGestureRecognizer** | ❌ | Multi-touch / scroll wheel zoom |
| [ ] **DragGestureRecognizer** | ❌ | Drag source for drag-and-drop |
| [ ] **DropGestureRecognizer** | ❌ | Drop target for drag-and-drop |
| [ ] **LongPressGestureRecognizer** | ❌ | Long press/hold detection |

> **MAUI Source Reference:**
> - Gesture platform managers: [`src/Controls/src/Core/Platform/GestureManager/`](https://github.com/dotnet/maui/tree/main/src/Controls/src/Core/Platform/GestureManager)
>
> **WPF Implementation Note:** WPF supports `MouseLeftButtonDown`/`Up`, `MouseMove`, `MouseWheel`, `DragDrop` natively. PinchGesture could map to scroll wheel with Ctrl key.

---

## 11. Graphics & Shapes

### Microsoft.Maui.Graphics

| Feature | Status | Notes |
|---------|--------|-------|
| [ ] **GraphicsView** | ❌ | Platform drawing surface with `IDrawable` rendering |
| [ ] **Canvas Operations** | ❌ | DrawLine, DrawRect, DrawEllipse, DrawPath, DrawString, Fill operations |
| [ ] **Canvas State** | ❌ | SaveState/RestoreState, transforms |
| [ ] **Brushes** | ⚠️ | `SolidColorBrush` mapped; `LinearGradientBrush`/`RadialGradientBrush` not mapped |

### Shapes

| Shape | Status | Notes |
|-------|--------|-------|
| [x] **ShapeViewHandler** | ✅ | Renders shapes via WPF `System.Windows.Shapes.*` |
| [x] **Rectangle** | ✅ | Via `ShapeViewHandler` |
| [x] **RoundRectangle** | ✅ | Via `ShapeViewHandler` |
| [x] **Ellipse** | ✅ | Via `ShapeViewHandler` |
| [x] **Line** | ✅ | Via `ShapeViewHandler` |
| [ ] **Path** | ❌ | Complex path geometry |
| [ ] **Polygon** | ❌ | Closed multi-point shape |
| [ ] **Polyline** | ❌ | Open multi-point shape |
| [x] **Fill & Stroke** | ✅ | `Fill` brush and `Stroke` properties mapped |

> **WPF Implementation Note:** WPF has excellent built-in shape support via `System.Windows.Shapes` namespace. `Path` can use `StreamGeometry` or `PathGeometry` for complex MAUI paths.

---

## 12. Common View Properties (Base Handler)

Every handler inherits these property mappings from `RemapForControls()` in `AppHostBuilderExtensions.cs`:

### Visibility & State

| Property | Status | Notes |
|----------|--------|-------|
| [x] **Opacity** | ✅ | `UIElement.Opacity` |
| [x] **IsVisible** | ✅ | `UIElement.Visibility` (Visible/Collapsed/Hidden) |
| [x] **IsEnabled** | ✅ | `UIElement.IsEnabled` |
| [ ] **InputTransparent** | ❌ | `IsHitTestVisible` not mapped |

### Sizing

| Property | Status | Notes |
|----------|--------|-------|
| [x] **WidthRequest / HeightRequest** | ✅ | `FrameworkElement.Width`/`Height` (only set when ≥ 0) |
| [x] **MinimumWidthRequest / MinimumHeightRequest** | ✅ | `FrameworkElement.MinWidth`/`MinHeight` |
| [x] **MaximumWidthRequest / MaximumHeightRequest** | ✅ | `FrameworkElement.MaxWidth`/`MaxHeight` (guards for NaN/Infinity) |

### Layout

| Property | Status | Notes |
|----------|--------|-------|
| [x] **HorizontalOptions / VerticalOptions** | ✅ | Handled by MAUI cross-platform layout engine |
| [x] **Margin** | ✅ | `FrameworkElement.Margin` mapped via `ViewMapper` |
| [x] **Padding** | ✅ | Mapped per handler (Entry, Editor, Button, Label, etc.) |
| [ ] **FlowDirection** | ❌ | `FrameworkElement.FlowDirection` not mapped |
| [ ] **ZIndex** | ❌ | `Panel.ZIndex` not mapped |

### Appearance

| Property | Status | Notes |
|----------|--------|-------|
| [x] **BackgroundColor / Background** | ✅ | `SolidPaint` → `SolidColorBrush` on `Control.Background` or `Panel.Background` |
| [ ] **LinearGradientBrush** | ❌ | Not yet mapped to WPF `LinearGradientBrush` |
| [ ] **RadialGradientBrush** | ❌ | Not yet mapped to WPF `RadialGradientBrush` |

### Transforms

| Property | Status | Notes |
|----------|--------|-------|
| [ ] **TranslationX / TranslationY** | ❌ | → WPF `TranslateTransform` |
| [ ] **Rotation / RotationX / RotationY** | ❌ | → WPF `RotateTransform` / `Viewport3D` |
| [ ] **Scale / ScaleX / ScaleY** | ❌ | → WPF `ScaleTransform` |
| [ ] **AnchorX / AnchorY** | ❌ | → WPF `RenderTransformOrigin` |

### Effects

| Property | Status | Notes |
|----------|--------|-------|
| [ ] **Shadow** | ❌ | → WPF `DropShadowEffect` |
| [ ] **Clip** | ❌ | → WPF `UIElement.Clip` with `RectangleGeometry`/`EllipseGeometry`/`PathGeometry` |

### Automation

| Property | Status | Notes |
|----------|--------|-------|
| [ ] **AutomationId** | ❌ | → `AutomationProperties.AutomationId` |
| [ ] **Semantic properties** | ❌ | → `AutomationProperties.Name`/`HelpText` |

### Interactivity Attachments

| Property | Status | Notes |
|----------|--------|-------|
| [ ] **ToolTip** | ❌ | `ToolTipProperties.Text` → WPF `FrameworkElement.ToolTip` |
| [ ] **ContextFlyout** | ❌ | → WPF `FrameworkElement.ContextMenu` |

---

## 13. VisualStateManager & Triggers

| Feature | Status | Notes |
|---------|--------|-------|
| [ ] **VisualStateManager** | ⚠️ | Cross-platform MAUI feature — may need platform hooks for hover/pressed/focus states |
| [x] **PropertyTrigger** | ✅ | Cross-platform — no platform handler needed |
| [x] **DataTrigger** | ✅ | Cross-platform — no platform handler needed |
| [x] **MultiTrigger** | ✅ | Cross-platform — no platform handler needed |
| [x] **EventTrigger** | ✅ | Cross-platform — no platform handler needed |
| [x] **Behaviors** | ✅ | Cross-platform — no platform handler needed |

> **WPF Implementation Note:** WPF has native VisualStateManager support. Platform hooks needed:
> - **PointerOver** — `MouseEnter`/`MouseLeave` already in `GestureManager`
> - **Pressed** — `MouseLeftButtonDown`/`Up`
> - **Focused** — `GotFocus`/`LostFocus`

---

## 14. Font Management

| Feature | Status | Notes |
|---------|--------|-------|
| [ ] **IFontManager** | ❌ | Resolves `Font` → WPF `System.Windows.Media.FontFamily` + size/weight/style |
| [ ] **IFontRegistrar** | ❌ | Registers embedded fonts with aliases |
| [ ] **IEmbeddedFontLoader** | ❌ | Loads font files from assembly resources into WPF |
| [ ] **Native Font Loading** | ❌ | WPF supports `pack://` URI fonts from resources |
| [ ] **IFontNamedSizeService** | ❌ | Maps `NamedSize` enum to platform point sizes |
| [x] **Font properties** | ⚠️ | `FontSize`, `FontFamily`, `FontAttributes` mapped per handler but no central font manager |
| [ ] **FontImageSource** | ❌ | Render font glyphs to images |

> **MAUI Source Reference:**
> - [`IFontManager`](https://github.com/dotnet/maui/blob/main/src/Core/src/Fonts/IFontManager.cs)
>
> **WPF Implementation Note:** WPF has rich font support. Embedded fonts can be loaded via `pack://application:,,,/Fonts/#FontName` URIs. `FontFamily`, `FontSize`, `FontWeight`, `FontStyle` are native WPF properties.

---

## 15. Essentials / Platform Services

| Service | Interface | Status | Notes |
|---------|-----------|--------|-------|
| [x] **App Info** | `IAppInfo` | ✅ | Assembly-based name, version, package; `RequestedTheme` returns `Unspecified` |
| [ ] **Battery** | `IBattery` | ❌ | Stub (desktop — limited relevance) |
| [x] **Browser** | `IBrowser` | ✅ | `Process.Start()` with UseShellExecute |
| [x] **Clipboard** | `IClipboard` | ✅ | WPF `System.Windows.Clipboard` |
| [x] **Connectivity** | `IConnectivity` | ✅ | `NetworkInterface.GetIsNetworkAvailable()` |
| [x] **Device Display** | `IDeviceDisplay` | ✅ | `SystemParameters.PrimaryScreenWidth/Height` with DPI |
| [x] **Device Info** | `IDeviceInfo` | ✅ | `Environment.OSVersion`, `DeviceIdiom.Desktop` |
| [ ] **Email** | `IEmail` | ⚠️ | Stub — `mailto:` URI not wired |
| [x] **File Picker** | `IFilePicker` | ⚠️ | Stub registered — needs WPF `OpenFileDialog` implementation |
| [x] **File System** | `IFileSystem` | ✅ | `Environment.SpecialFolder` paths |
| [ ] **Geolocation** | `IGeolocation` | ❌ | Stub (desktop — low priority) |
| [ ] **Haptic Feedback** | `IHapticFeedback` | ❌ | N/A for desktop |
| [x] **Launcher** | `ILauncher` | ✅ | `Process.Start()` with UseShellExecute |
| [ ] **Map** | `IMap` | ❌ | Stub |
| [ ] **Media Picker** | `IMediaPicker` | ❌ | Stub |
| [x] **Preferences** | `IPreferences` | ✅ | `IsolatedStorageSettings` or registry-based |
| [ ] **Screenshot** | `IScreenshot` | ❌ | Could use `RenderTargetBitmap` (MauiDevFlow already does this) |
| [x] **Secure Storage** | `ISecureStorage` | ✅ | `ProtectedData` (DPAPI) |
| [ ] **Semantic Screen Reader** | `ISemanticScreenReader` | ❌ | Stub |
| [x] **Share** | `IShare` | ⚠️ | Stub — no native share dialog on Windows desktop |
| [ ] **Text-to-Speech** | `ITextToSpeech` | ❌ | Could use `System.Speech.Synthesis.SpeechSynthesizer` |
| [x] **Version Tracking** | `IVersionTracking` | ✅ | Cross-platform — uses `IPreferences` + `IAppInfo` |
| [ ] **Vibration** | `IVibration` | ❌ | N/A for desktop |
| [ ] **Sensors** | Various | ❌ | N/A for desktop |

> **⚠️ Known Extensibility Gap:** Essentials use internal `SetDefault()` methods. Custom backends must use reflection. See [dotnet/maui#34100](https://github.com/dotnet/maui/issues/34100).

---

## 16. Styling Infrastructure

| Feature | Status | Notes |
|---------|--------|-------|
| [ ] **Border style mapping** | ⚠️ | `Stroke`, `StrokeThickness` mapped on `BorderHandler`; `StrokeLineCap`, `StrokeLineJoin`, `StrokeDashPattern` not mapped |
| [x] **View state mapping** | ✅ | `IsVisible`, `IsEnabled`, `Opacity` mapped in base `ViewMapper` |
| [ ] **Automation mapping** | ❌ | `AutomationId` → `AutomationProperties.AutomationId` not mapped |

---

## 17. WebView

| Feature | Status | Notes |
|---------|--------|-------|
| [ ] **URL loading** | ❌ | WPF has `WebView2` (Chromium) — not yet wrapped |
| [ ] **HTML content** | ❌ | |
| [ ] **JavaScript execution** | ❌ | `EvaluateJavaScriptAsync` |
| [ ] **Navigation commands** | ❌ | GoBack, GoForward, Reload |
| [ ] **Navigation events** | ❌ | Navigating, Navigated |
| [ ] **User Agent** | ❌ | Custom user agent string |

> **WPF Implementation Note:** Use `Microsoft.Web.WebView2.Wpf.WebView2` control. Already available as a NuGet package and used by BlazorWebView.

---

## 18. BlazorWebView (Blazor Hybrid)

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **BlazorWebViewHandler** | ✅ | Uses `Microsoft.AspNetCore.Components.WebView.Wpf.BlazorWebView` |
| [x] **JavaScript Bridge** | ✅ | Built into `AspNetCore.Components.WebView.Wpf` |
| [x] **Static Asset Serving** | ✅ | Built-in |
| [x] **Blazor Dispatcher** | ✅ | Built-in |
| [x] **Host Page** | ✅ | Configurable |
| [x] **StartPath** | ✅ | Configurable |
| [x] **Root Components** | ✅ | Registration works |

> **Note:** BlazorWebView for WPF is an official Microsoft package (`Microsoft.AspNetCore.Components.WebView.Wpf`), so most features work out of the box. Our handler wraps it for MAUI integration.

---

## 19. Label — FormattedText Detail

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **FormattedText rendering** | ✅ | Builds `TextBlock` with multiple `Run`/`Span` elements from `FormattedString.Spans` |
| [x] **Span.Text** | ✅ | Text content per span |
| [x] **Span.TextColor** | ✅ | `Run.Foreground` = `SolidColorBrush` |
| [x] **Span.BackgroundColor** | ✅ | `Run.Background` = `SolidColorBrush` (WPF `Inline` supports this) |
| [x] **Span.FontSize** | ✅ | `Run.FontSize` |
| [x] **Span.FontFamily** | ✅ | `Run.FontFamily` |
| [x] **Span.FontAttributes** | ✅ | `Run.FontWeight` (Bold) / `Run.FontStyle` (Italic) |
| [x] **Span.TextDecorations** | ✅ | `Run.TextDecorations` (Underline/Strikethrough) |
| [ ] **Span.CharacterSpacing** | ❌ | WPF doesn't have a direct kerning property on `Run` |

---

## 20. MenuBar (Desktop)

| Feature | Status | Notes |
|---------|--------|-------|
| [ ] **MenuBarItem** | ❌ | → WPF `Menu` + `MenuItem` (top-level) |
| [ ] **MenuFlyoutItem** | ❌ | → `MenuItem` with `Command`, `InputGestureText` for accelerators |
| [ ] **MenuFlyoutSubItem** | ❌ | → Nested `MenuItem` (recursive) |
| [ ] **MenuFlyoutSeparator** | ❌ | → `Separator` |
| [ ] **Default Menus** | ❌ | Standard Edit/Window menus |

> **WPF Implementation Note:** WPF `Menu` control is a perfect fit. `DockPanel.Dock="Top"` in `WindowHandler` with `MenuItem` children. Keyboard accelerators via `InputGestureText` and `RoutedCommand` bindings.

---

## 21. Animations

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **Platform Ticker** | ✅ | `WPFTicker` — `DispatcherTimer` at `DispatcherPriority.Render` for ~60fps |
| [ ] **TranslateTo** | ❌ | Needs `TranslationX`/`TranslationY` → `TranslateTransform` mapping |
| [ ] **FadeTo** | ⚠️ | `Opacity` mapped — animation system drives it via ticker |
| [ ] **ScaleTo** | ❌ | Needs `Scale` → `ScaleTransform` mapping |
| [ ] **RotateTo** | ❌ | Needs `Rotation` → `RotateTransform` mapping |
| [ ] **LayoutTo** | ⚠️ | Layout system works — animation drives bounds changes |
| [x] **Easing functions** | ✅ | Cross-platform MAUI — no platform code needed |
| [x] **Animation class** | ✅ | Cross-platform MAUI — no platform code needed |

> **Key Concept:** MAUI's animation system is fully cross-platform. It uses `IAnimationManager` + `ITicker` to drive frame updates. Our `WPFTicker` provides the main-thread-safe timer.

---

## 22. ControlTemplate & ContentPresenter

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **ControlTemplate** | ✅ | Cross-platform MAUI feature — template inflation via ContentPresenter |
| [x] **ContentPresenter** | ✅ | Cross-platform — dynamically instantiates template content |
| [x] **TemplatedView** | ✅ | Cross-platform — base class for controls with ControlTemplate support |

> These are fully cross-platform MAUI features that require no platform-specific code.

---

## 23. Image Source Types

| Source Type | Status | Notes |
|-------------|--------|-------|
| [x] **FileImageSource** | ⚠️ | Basic loading via `BitmapImage` — needs improved resource resolution |
| [ ] **UriImageSource** | ❌ | Async HTTP loading → `BitmapImage` with `UriSource` |
| [ ] **StreamImageSource** | ❌ | Stream → `BitmapImage` via `BeginInit`/`StreamSource`/`EndInit` |
| [ ] **FontImageSource** | ❌ | Render font glyphs via `FormattedText` → `DrawingVisual` → `RenderTargetBitmap` |

> **WPF Implementation Note:** WPF `BitmapImage` natively supports URI sources (http/https), file paths, and streams. `UriImageSource` should be straightforward.

---

## 24. Lifecycle Events

| Event | Status | Notes |
|-------|--------|-------|
| [x] **App Launched** | ✅ | `MauiWPFApplication.OnStartup` |
| [ ] **App Activated** | ❌ | → `System.Windows.Application.Activated` |
| [ ] **App Deactivated** | ❌ | → `System.Windows.Application.Deactivated` |
| [ ] **App Terminating** | ❌ | → `System.Windows.Application.Exit` |
| [ ] **Window Created** | ❌ | Platform lifecycle event |
| [ ] **Window Activated** | ❌ | → `System.Windows.Window.Activated` |
| [ ] **Window Deactivated** | ❌ | → `System.Windows.Window.Deactivated` |
| [ ] **Window Closing** | ❌ | → `System.Windows.Window.Closing` |

---

## 25. App Theme / Dark Mode

| Feature | Status | Notes |
|---------|--------|-------|
| [ ] **System theme detection** | ❌ | Read Windows registry `AppsUseLightTheme` or `SystemParameters.HighContrast` |
| [ ] **UserAppTheme** | ❌ | Programmatic theme switching |
| [ ] **RequestedThemeChanged** | ❌ | Event when system or app theme changes |
| [x] **AppThemeBinding** | ✅ | Cross-platform MAUI feature — works via property binding system |

> **WPF Implementation Note:** Detect Windows 10+ dark mode via registry key `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize\AppsUseLightTheme`. Listen for `SystemEvents.UserPreferenceChanged` to detect theme changes.

---

## 26. Build System & Resizetizer Integration

| Item Type | Status | Notes |
|-----------|--------|-------|
| [ ] **MauiIcon** | ❌ | Convert to `.ico` format for WPF |
| [ ] **MauiImage** | ❌ | Resize to multiple DPI scales; copy to output |
| [ ] **MauiFont** | ❌ | Copy to output; register via `pack://` URI |
| [ ] **MauiAsset** | ❌ | Copy to output with `LogicalName` |
| [ ] **MauiSplashScreen** | ❌ | Convert to WPF splash screen |

> **WPF Implementation Note:** Use `AfterTargets="ResizetizeImages"` to hook processed images. See [dotnet/maui#34222](https://github.com/dotnet/maui/issues/34222).

---

## 27. Known MAUI Extensibility Gaps & Workarounds

### Gap 1: Essentials Static `Default` Properties ([#34100](https://github.com/dotnet/maui/issues/34100))

**Status:** ⚠️ Using reflection workaround in `EssentialsExtensions.cs`

```csharp
var setDefault = typeof(Preferences).GetMethod("SetDefault",
    BindingFlags.Static | BindingFlags.NonPublic);
setDefault?.Invoke(null, new object[] { new WPFPreferences() });
```

### Gap 2: `MainThread.BeginInvokeOnMainThread` ([#34101](https://github.com/dotnet/maui/issues/34101))

**Status:** ⚠️ Documented — app developers should use `Dispatcher.Dispatch()` instead.

### Gap 3: Resizetizer Extensibility ([#34102](https://github.com/dotnet/maui/issues/34102), [#34222](https://github.com/dotnet/maui/issues/34222))

**Status:** ❌ Not yet addressed — no custom build targets.

### Gap 4: BlazorWebView Registration ([#34103](https://github.com/dotnet/maui/issues/34103))

**Status:** ✅ Worked around — we use `AspNetCore.Components.WebView.Wpf` directly with a custom `BlazorWebViewHandler`.

### Gap 5: Alert/Dialog System ([#34104](https://github.com/dotnet/maui/issues/34104))

**Status:** ✅ Implemented via `WPFAlertManagerSubscription` using `DispatchProxy` + reflection.

---

## 28. Project Structure

```
D:\repos\rmarinho\maui.wpf\
├── BACKEND_IMPLEMENTATION_CHECKLIST.md
├── Directory.Build.props          # Centralized: BaseTargetFramework=net10.0, MauiVersion=10.0.31
├── Maui.WPF.sln
├── README.md
├── build.slnf                     # Solution filter for CI (libraries only)
├── .github/
│   ├── workflows/build.yml        # CI: windows-latest, .NET 10, build + NuGet publish
│   └── copilot-instructions.md
├── samples/
│   ├── Maui.Controls.Sample/      # ControlGallery — comprehensive demo
│   ├── Maui.Controls.Sample.Blazor/  # Blazor Hybrid sample
│   └── Maui.Sample/               # Basic sample
└── src/
    ├── Platform.Maui.WPF/         # Main handler library
    │   ├── Handlers/               # 27 handlers (one file per control)
    │   ├── Hosting/                # AppHostBuilderExtensions, MauiWPFApplication, etc.
    │   ├── Platform/               # GestureManager, WPFTicker, WPFAlertManagerSubscription
    │   └── DevFlow/                # MauiDevFlow agent integration
    └── Platform.Maui.Essentials.WPF/  # Essentials implementations
```

---

## 29. Implementation Priority Order

### Phase 1: Foundation ~~(Get a window with "Hello World")~~ ✅ COMPLETE
1. ~~Core infrastructure (base handler, dispatcher, context, host builder)~~ ✅
2. ~~Application + Window handlers~~ ✅
3. ~~ContentPage handler~~ ✅
4. ~~LayoutHandler (VerticalStackLayout, HorizontalStackLayout)~~ ✅
5. ~~Label handler~~ ✅
6. ~~Basic essentials (AppInfo, DeviceInfo, FileSystem, Preferences)~~ ✅

### Phase 2: Basic Controls ~~(Interactive app)~~ ✅ COMPLETE
7. ~~Button, Entry, Editor handlers~~ ✅
8. ~~Image handler (FileImageSource first)~~ ✅
9. ~~Switch, CheckBox, Slider, ProgressBar, ActivityIndicator~~ ✅
10. ~~ScrollView handler~~ ✅
11. ~~Border handler~~ ✅
12. Font management (IFontManager, IEmbeddedFontLoader) ❌
13. ~~Gesture recognizers (Tap)~~ ✅ + Pointer ✅; Pan ❌

### Phase 3: Navigation ~~(Multi-page app)~~ ✅ COMPLETE
14. ~~NavigationPage handler (push/pop)~~ ✅
15. ~~TabbedPage handler~~ ✅
16. ~~FlyoutPage handler~~ ✅
17. ~~Alert/Dialog system (DisplayAlert, DisplayActionSheet, DisplayPromptAsync)~~ ✅
18. ~~Animations (ITicker)~~ ✅

### Phase 4: Advanced Controls 🔴 IN PROGRESS
19. **CollectionView** / ListView handlers ❌ **P0**
20. ~~Picker, DatePicker, TimePicker handlers~~ ✅
21. ~~SearchBar handler~~ ✅
22. RadioButton ❌, ~~Stepper~~ ✅
23. CarouselView, IndicatorView ❌
24. TableView, SwipeView, RefreshView ❌
25. GraphicsView + ~~ShapeViewHandler~~ ✅

### Phase 5: Rich Features 🔴 NOT STARTED
26. Shell handler ❌
27. WebView handler ❌
28. ~~BlazorWebView handler~~ ✅
29. MenuBar ❌
30. ~~FormattedText (Label spans)~~ ✅
31. Image source types (URI ❌, Stream ❌, FontImage ❌)
32. Remaining gesture recognizers (Pan, Swipe, Pinch, Drag, Drop, LongPress) ❌
33. Remaining essentials ❌
34. App Theme / Dark Mode ❌
35. Lifecycle events ❌
36. Build targets / Resizetizer integration ❌

---

## 30. Summary Statistics

| Category | Implemented | Total | Coverage |
|----------|-------------|-------|----------|
| **Core Infrastructure** | 9 of 11 | 11 | 82% |
| **Application & Window** | 8 of 12 | 12 | 67% |
| **Pages** | 4 of 6 | 6 | 67% |
| **Layouts** | 9 of 10 | 10 | 90% |
| **Basic Controls** | 12 of 14 | 14 | 86% |
| **Input Controls** | 4 of 4 | 4 | 100% |
| **Collection Controls** | 0 of 7 | 7 | 0% |
| **Navigation** | 4 of 8 | 8 | 50% |
| **Alerts & Dialogs** | 3 of 3 | 3 | 100% |
| **Gesture Recognizers** | 2 of 8 | 8 | 25% |
| **Graphics & Shapes** | 5 of 10 | 10 | 50% |
| **Base View Properties** | 11 of 22 | 22 | 50% |
| **Font Services** | 1 of 7 | 7 | 14% |
| **Essentials** | 11 of 25 | 25 | 44% |
| **WebView** | 0 of 6 | 6 | 0% |
| **BlazorWebView** | 7 of 7 | 7 | 100% |
| **FormattedText/Spans** | 8 of 9 | 9 | 89% |
| **MenuBar** | 0 of 5 | 5 | 0% |
| **Animations** | 4 of 8 | 8 | 50% |
| **VSM & Triggers** | 5 of 6 | 6 | 83% |
| **ControlTemplate** | 3 of 3 | 3 | 100% |
| **Lifecycle Events** | 1 of 8 | 8 | 13% |
| **Image Source Types** | 1 of 4 | 4 | 25% |
| **App Theme** | 1 of 4 | 4 | 25% |
| **Build/Resizetizer** | 0 of 5 | 5 | 0% |
| | | | |
| **TOTAL** | **113 of 222** | **222** | **51%** |

---

## 31. MauiDevFlow Integration

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **WpfDevFlowAgentService** | ✅ | In `DevFlow/` — visual tree walking, property inspection |
| [x] **Screenshot capture** | ✅ | `RenderTargetBitmap` → PNG |
| [x] **Element tapping** | ✅ | `ButtonAutomationPeer.Invoke()` for button automation |
| [ ] **Full element interaction** | ❌ | Only button tap — needs text input, scroll, etc. |
| [ ] **Visual tree diff** | ❌ | Compare tree snapshots for change detection |

---

## Known Issues & Bugs

| Issue | Severity | Notes |
|-------|----------|-------|
| Button VirtualView null crash | Medium | Patched with try/catch guard |
| WPF Button chrome overrides Background | Low | System theme overrides custom background on classic theme |
| Entry/Editor lack placeholder watermark | Low | Placeholder text not visually distinct |
| NavigationPage: no animated transitions | Low | Pages swap instantly |
| Slider: ThumbColor not mapped | Low | |
| Switch: TrackColor/ThumbColor stubs | Low | Custom template doesn't respond to color properties |
| SearchBar: no search icon or clear button | Low | Plain TextBox + Button |
| Frame not registered | Low | Could alias to BorderHandler |

---

*Last updated: 2026-02-26*
*Branch: `more-controls`*
*Target Framework: `net10.0` | MAUI Version: `10.0.31`*
