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
| [x] **IPlatformApplication** | ✅ | `WPFPlatformApplication` implements `IPlatformApplication` with App/Services |

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
| [x] **Transform Conversion** | ✅ | `UpdateTransformGroup()` maps Scale/Rotate/Translate → `TransformGroup`, `RenderTransformOrigin` for anchors |

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
| [x] **ThemeChanged** | ✅ | `ThemeManager` detects dark/light via registry, `SystemEvents.UserPreferenceChanged` fires theme changes |
| [x] **OpenWindow / CloseWindow** | ✅ | `ApplicationHandler.MapOpenWindow` creates new `System.Windows.Window`, `MapCloseWindow` calls `.Close()` |

### Window

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **WindowHandler** | ✅ | `WindowHandler.cs` — full implementation |
| [x] **Title** | ✅ | Maps `IWindow.Title` → `System.Windows.Window.Title` |
| [x] **Width / Height** | ✅ | Maps to WPF `Width`/`Height` |
| [x] **X / Y** | ✅ | Maps to WPF `Left`/`Top` |
| [x] **MinimumWidth / MinimumHeight** | ✅ | Mapped |
| [x] **MaximumWidth / MaximumHeight** | ✅ | Mapped |
| [x] **Page (Content)** | ✅ | `PageHandler` sets content via `ContentPanel`, tracks page changes through handler mappers |
| [x] **MenuBar** | ✅ | `WindowHandler.SetupMenuBar()` creates WPF `Menu` + `MenuItem` hierarchy from MAUI MenuBar |
| [x] **Multi-window** | ✅ | `ApplicationHandler.MapOpenWindow` creates additional WPF windows |

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
| [x] **Shell** | ✅ | `ShellHandler` with flyout panel, tab control, URI-based navigation, shell item routing |
| [x] **ModalPage** | ✅ | `ModalNavigationManager` with animated overlay push/pop |

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
| [x] **Frame** | ✅ | Registered as `BorderHandler` alias in `AddMauiControlsHandlers()` |

> **Key Concept:** MAUI's layout engine (StackLayout, Grid, FlexLayout, AbsoluteLayout) is entirely cross-platform. The `LayoutHandler` just needs to create a native container, add/remove children, and call `Measure`/`Arrange` using MAUI-computed bounds.

---

## 5. Basic Controls

| Control | Status | Notes |
|---------|--------|-------|
| [x] **Label** | ✅ | Full: `Text`, `TextColor`, `FontSize`, `FontFamily`, `FontAttributes`, `HorizontalTextAlignment`, `VerticalTextAlignment`, `MaxLines`, `LineBreakMode`, `TextDecorations`, `CharacterSpacing`, `Padding`, `FormattedText` |
| [x] **Button** | ✅ | `Text`, `TextColor`, `FontSize`, `Background`, `Padding`, `CornerRadius` (stub), `Command`; WPF chrome overrides Background on some system themes |
| [x] **Image** | ✅ | `FileImageSource`, `UriImageSource`, `StreamImageSource` loading; `Aspect` (Fill, AspectFit, AspectFill) via `Stretch` |
| [x] **ImageButton** | ✅ | WPF `Button` with `Image` content; `Source`, `Aspect`, `Padding`, `BorderWidth`, `BorderColor` |
| [x] **Entry** | ✅ | `Text`, `TextColor`, `FontSize`, `IsPassword`, `Placeholder`, `MaxLength`, `IsReadOnly`, `Keyboard`, `ReturnType`; missing `PlaceholderColor` watermark |
| [x] **Editor** | ✅ | `Text`, `TextColor`, `FontSize`, `Placeholder`, `MaxLength`, `IsReadOnly`; missing placeholder watermark |
| [x] **Switch** | ✅ | Custom WPF `ControlTemplate` with animated toggle; `IsToggled`, `OnColor`, `TrackColor` (stub), `ThumbColor` (stub) |
| [x] **CheckBox** | ✅ | WPF `CheckBox`; `IsChecked`, `Foreground` (via `SolidPaint`) |
| [x] **Slider** | ✅ | WPF `Slider`; `Value`, `Minimum`, `Maximum`, `MinimumTrackColor`, `MaximumTrackColor`; `ThumbColor` not mapped |
| [x] **Stepper** | ✅ | Custom WPF panel with ▲/▼ buttons; `Value`, `Minimum`, `Maximum`, `Increment` |
| [x] **ProgressBar** | ✅ | WPF `ProgressBar`; `Progress` (0-1 → 0-100) |
| [x] **ActivityIndicator** | ✅ | Custom rotating arc via `DispatcherTimer`; `IsRunning`, `Color` |
| [x] **BoxView** | ✅ | Rendered via `ShapeViewHandler` as filled rectangle |
| [x] **RadioButton** | ✅ | WPF `RadioButton`; `IsChecked`, `Content`, `TextColor`, `FontSize`, `GroupName` |

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
| [x] **CollectionView** | ✅ | WPF `ListBox` with `MauiDataTemplateSelector` + `MauiContentPresenter`; `ItemsSource`, `SelectedItem`, `SelectionMode`, `EmptyView` |
| [x] **ListView** | ✅ | `ListViewHandler` using WPF `ListBox` with MAUI template bridge |
| [x] **CarouselView** | ✅ | `CarouselViewHandler` with horizontal ListBox + arrow buttons |
| [x] **IndicatorView** | ✅ | `IndicatorViewHandler` renders dot indicators as Ellipses |
| [x] **TableView** | ✅ | `TableViewHandler` with grouped sections, TextCell, SwitchCell, EntryCell |
| [x] **SwipeView** | ✅ | `SwipeViewHandler` approximated via context menu |
| [x] **RefreshView** | ✅ | `RefreshViewHandler` with progress bar indicator |

> **Priority:** CollectionView is the most critical missing control. WPF's `ListBox`/`ListView`/`ItemsControl` with `DataTemplate` is a natural fit.

---

## 8. Navigation & Routing

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **Push/Pop** | ✅ | `NavigationViewHandler` handles `IStackNavigationView.RequestNavigation` → stack management → `NavigationFinished()` |
| [x] **Back Button** | ✅ | Auto-shown when navigation stack depth > 1 |
| [x] **Title Bar** | ✅ | `TextBlock` in `DockPanel` toolbar showing current page title |
| [x] **Toolbar Items** | ✅ | Rendered as `Button`s in toolbar `StackPanel` |
| [x] **Modal Navigation** | ✅ | `ModalNavigationManager.PushModal()`/`PopModal()` with animated overlay |
| [x] **Shell Navigation** | ✅ | `ShellHandler` routes shell items, flyout, tabs |
| [x] **URI-based Navigation** | ✅ | Via Shell routing and GoToAsync |
| [x] **Animated Transitions** | ✅ | `PageTransitionHelper.AnimatePageIn()`/`AnimatePageOut()` with slide+fade |

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
| [x] **PanGestureRecognizer** | ✅ | `GestureManager` — mouse capture + delta tracking, PanStarted/Running/Completed |
| [x] **SwipeGestureRecognizer** | ✅ | `GestureManager` — threshold-based direction detection |
| [x] **PinchGestureRecognizer** | ✅ | Ctrl+MouseWheel zoom via `GestureManager` |
| [x] **DragGestureRecognizer** | ✅ | `GestureManager` — WPF `DragDrop.DoDragDrop` |
| [x] **DropGestureRecognizer** | ✅ | `GestureManager` — `AllowDrop` + `DragOver`/`Drop` events |
| [x] **LongPressGestureRecognizer** | ✅ | `GestureManager` — `DispatcherTimer` 500ms hold detection |

> **MAUI Source Reference:**
> - Gesture platform managers: [`src/Controls/src/Core/Platform/GestureManager/`](https://github.com/dotnet/maui/tree/main/src/Controls/src/Core/Platform/GestureManager)
>
> **WPF Implementation Note:** WPF supports `MouseLeftButtonDown`/`Up`, `MouseMove`, `MouseWheel`, `DragDrop` natively. PinchGesture could map to scroll wheel with Ctrl key.

---

## 11. Graphics & Shapes

### Microsoft.Maui.Graphics

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **GraphicsView** | ✅ | `GraphicsViewHandler` + `WpfCanvas` ICanvas implementation via DrawingContext |
| [x] **Canvas Operations** | ✅ | DrawLine, DrawRect, DrawEllipse, DrawRoundedRect, DrawString, DrawPath, Fill operations |
| [x] **Canvas State** | ✅ | SaveState/RestoreState, Translate, Scale, Rotate, ClipRect |
| [x] **Brushes** | ✅ | `ConvertPaintToBrush()` handles Solid, LinearGradient, RadialGradient paints |

### Shapes

| Shape | Status | Notes |
|-------|--------|-------|
| [x] **ShapeViewHandler** | ✅ | Renders shapes via WPF `System.Windows.Shapes.*` |
| [x] **Rectangle** | ✅ | Via `ShapeViewHandler` |
| [x] **RoundRectangle** | ✅ | Via `ShapeViewHandler` |
| [x] **Ellipse** | ✅ | Via `ShapeViewHandler` |
| [x] **Line** | ✅ | Via `ShapeViewHandler` |
| [x] **Path** | ✅ | MAUI Geometry → WPF `StreamGeometry` via `Geometry.Parse` |
| [x] **Polygon** | ✅ | MAUI `PointCollection` → WPF `Polygon` |
| [x] **Polyline** | ✅ | MAUI `PointCollection` → WPF `Polyline` |
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
| [x] **InputTransparent** | ✅ | `UIElement.IsHitTestVisible` |

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
| [x] **FlowDirection** | ✅ | `FrameworkElement.FlowDirection` mapped |
| [x] **ZIndex** | ✅ | `Panel.SetZIndex()` in ViewMapper |

### Appearance

| Property | Status | Notes |
|----------|--------|-------|
| [x] **BackgroundColor / Background** | ✅ | `SolidPaint` → `SolidColorBrush` on `Control.Background` or `Panel.Background` |
| [x] **LinearGradientBrush** | ✅ | `ConvertPaintToBrush()` maps MAUI `LinearGradientPaint` → WPF `LinearGradientBrush` |
| [x] **RadialGradientBrush** | ✅ | `ConvertPaintToBrush()` maps MAUI `RadialGradientPaint` → WPF `RadialGradientBrush` |

### Transforms

| Property | Status | Notes |
|----------|--------|-------|
| [x] **TranslationX / TranslationY** | ✅ | → WPF `TranslateTransform` via `TransformGroup` |
| [x] **Rotation / RotationX / RotationY** | ✅ | → WPF `RotateTransform` |
| [x] **Scale / ScaleX / ScaleY** | ✅ | → WPF `ScaleTransform` |
| [x] **AnchorX / AnchorY** | ✅ | → WPF `RenderTransformOrigin` |

### Effects

| Property | Status | Notes |
|----------|--------|-------|
| [x] **Shadow** | ✅ | → WPF `DropShadowEffect` with radius, offset, opacity |
| [x] **Clip** | ✅ | → WPF `UIElement.Clip` via `Geometry.Parse` |

### Automation

| Property | Status | Notes |
|----------|--------|-------|
| [x] **AutomationId** | ✅ | → `AutomationProperties.AutomationId` |
| [x] **Semantic properties** | ✅ | → `AutomationProperties.Name`/`HelpText` |

### Interactivity Attachments

| Property | Status | Notes |
|----------|--------|-------|
| [x] **ToolTip** | ✅ | `ToolTipProperties.Text` → WPF `FrameworkElement.ToolTip` |
| [x] **ContextFlyout** | ✅ | `FlyoutBase.GetContextFlyout()` → WPF `ContextMenu` with `MenuItem` hierarchy |

---

## 13. VisualStateManager & Triggers

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **VisualStateManager** | ✅ | `VisualStateManagerHooks` wires PointerOver, Pressed, Focused, Disabled states |
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
| [x] **IFontManager** | ✅ | `WPFFontManager` — resolves `Font` → WPF `FontFamily` + size with embedded font support |
| [x] **IFontRegistrar** | ✅ | `WPFFontRegistrar` — manages font alias registration |
| [x] **IEmbeddedFontLoader** | ✅ | `WPFEmbeddedFontLoader` — extracts fonts from assembly resources to temp dir |
| [x] **Native Font Loading** | ✅ | `WPFEmbeddedFontLoader` extracts to temp dir, `WPFFontRegistrar` manages cache |
| [x] **IFontNamedSizeService** | ✅ | `WPFFontNamedSizeService` maps Micro→10, Small→12, Medium→14, Large→18, Header→24, Title→28 |
| [x] **Font properties** | ✅ | `FontSize`, `FontFamily`, `FontAttributes` mapped per handler + central `IFontManager` |
| [x] **FontImageSource** | ✅ | `FontImageSourceHelper.RenderGlyph()` renders font glyphs via `FormattedText` → `DrawingVisual` → `RenderTargetBitmap` |

> **MAUI Source Reference:**
> - [`IFontManager`](https://github.com/dotnet/maui/blob/main/src/Core/src/Fonts/IFontManager.cs)
>
> **WPF Implementation Note:** WPF has rich font support. Embedded fonts can be loaded via `pack://application:,,,/Fonts/#FontName` URIs. `FontFamily`, `FontSize`, `FontWeight`, `FontStyle` are native WPF properties.

---

## 15. Essentials / Platform Services

| Service | Interface | Status | Notes |
|---------|-----------|--------|-------|
| [x] **App Info** | `IAppInfo` | ✅ | Assembly-based name, version, package; `RequestedTheme` returns `Unspecified` |
| [x] **Battery** | `IBattery` | ✅ | `WPFBattery` — returns Full/AC for desktop |
| [x] **Browser** | `IBrowser` | ✅ | `Process.Start()` with UseShellExecute |
| [x] **Clipboard** | `IClipboard` | ✅ | WPF `System.Windows.Clipboard` |
| [x] **Connectivity** | `IConnectivity` | ✅ | `NetworkInterface.GetIsNetworkAvailable()` |
| [x] **Device Display** | `IDeviceDisplay` | ✅ | `SystemParameters.PrimaryScreenWidth/Height` with DPI |
| [x] **Device Info** | `IDeviceInfo` | ✅ | `Environment.OSVersion`, `DeviceIdiom.Desktop` |
| [x] **Email** | `IEmail` | ✅ | `WPFEmail` opens `mailto:` URI via `Process.Start` |
| [x] **File Picker** | `IFilePicker` | ⚠️ | Stub registered — needs WPF `OpenFileDialog` implementation |
| [x] **File System** | `IFileSystem` | ✅ | `Environment.SpecialFolder` paths |
| [x] **Geolocation** | `IGeolocation` | ✅ | Stub (desktop — limited relevance) |
| [x] **Haptic Feedback** | `IHapticFeedback` | ✅ | Stub (N/A for desktop) |
| [x] **Launcher** | `ILauncher` | ✅ | `Process.Start()` with UseShellExecute |
| [x] **Map** | `IMap` | ✅ | `WPFMap` opens Bing Maps URL via `Process.Start` |
| [x] **Media Picker** | `IMediaPicker` | ✅ | Stub (camera N/A for desktop) |
| [x] **Preferences** | `IPreferences` | ✅ | `IsolatedStorageSettings` or registry-based |
| [x] **Screenshot** | `IScreenshot` | ✅ | `WPFScreenshot` uses `RenderTargetBitmap` to capture window |
| [x] **Secure Storage** | `ISecureStorage` | ✅ | `ProtectedData` (DPAPI) |
| [x] **Semantic Screen Reader** | `ISemanticScreenReader` | ✅ | Stub — WPF uses UIA natively |
| [x] **Share** | `IShare` | ⚠️ | Stub — no native share dialog on Windows desktop |
| [x] **Text-to-Speech** | `ITextToSpeech` | ✅ | `WPFTextToSpeech` uses PowerShell `System.Speech.Synthesis` |
| [x] **Version Tracking** | `IVersionTracking` | ✅ | Cross-platform — uses `IPreferences` + `IAppInfo` |
| [x] **Vibration** | `IVibration` | ✅ | Stub (N/A for desktop) |
| [x] **Sensors** | Various | ✅ | Stubs for all sensors (Accelerometer, Barometer, Compass, Gyroscope, etc.) — N/A for desktop |

> **⚠️ Known Extensibility Gap:** Essentials use internal `SetDefault()` methods. Custom backends must use reflection. See [dotnet/maui#34100](https://github.com/dotnet/maui/issues/34100).

---

## 16. Styling Infrastructure

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **Border style mapping** | ✅ | `StrokeDashPattern`, `StrokeLineCap`, `StrokeLineJoin` mappers added to `BorderHandler` |
| [x] **View state mapping** | ✅ | `IsVisible`, `IsEnabled`, `Opacity` mapped in base `ViewMapper` |
| [x] **Automation mapping** | ✅ | `AutomationId` → `AutomationProperties.SetAutomationId()`, Semantics → Name/HelpText |

---

## 17. WebView

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **URL loading** | ✅ | `WebViewHandler` — WebView2 with `UrlWebViewSource` |
| [x] **HTML content** | ✅ | `NavigateToString()` via `HtmlWebViewSource` |
| [x] **JavaScript execution** | ✅ | `ExecuteScriptAsync` via `EvaluateJavaScriptAsync` |
| [x] **Navigation commands** | ✅ | GoBack, GoForward, Reload mapped |
| [x] **Navigation events** | ✅ | `NavigationStarting`/`NavigationCompleted` from WebView2 |
| [x] **User Agent** | ✅ | `WebViewHandler.MapUserAgent` sets custom user agent via WebView2 settings |

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
| [x] **Span.CharacterSpacing** | ⚠️ | WPF limitation — no direct CharacterSpacing API on TextBlock/Run. Mapper registered but no-op. |

---

## 20. MenuBar (Desktop)

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **MenuBarItem** | ✅ | `WindowHandler.SetupMenuBar()` creates WPF `MenuItem` from MAUI `MenuBarItem` |
| [x] **MenuFlyoutItem** | ✅ | Maps to WPF `MenuItem` with Command + InputGestureText for accelerators |
| [x] **MenuFlyoutSubItem** | ✅ | Recursive nested `MenuItem` children |
| [x] **MenuFlyoutSeparator** | ✅ | Maps to WPF `Separator` |
| [x] **Default Menus** | ⚠️ | App-defined menus work; standard Edit/Window menus not auto-generated |

> **WPF Implementation Note:** WPF `Menu` control is a perfect fit. `DockPanel.Dock="Top"` in `WindowHandler` with `MenuItem` children. Keyboard accelerators via `InputGestureText` and `RoutedCommand` bindings.

---

## 21. Animations

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **Platform Ticker** | ✅ | `WPFTicker` — `DispatcherTimer` at `DispatcherPriority.Render` for ~60fps |
| [x] **TranslateTo** | ✅ | `TranslationX`/`TranslationY` → `TranslateTransform` mapping wired |
| [x] **FadeTo** | ✅ | `Opacity` mapped — animation system drives it via ticker |
| [x] **ScaleTo** | ✅ | `Scale` → `ScaleTransform` mapping wired |
| [x] **RotateTo** | ✅ | `Rotation` → `RotateTransform` mapping wired |
| [x] **LayoutTo** | ✅ | Layout system works — animation drives bounds changes |
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
| [x] **FileImageSource** | ✅ | `BitmapImage` with improved resolution (app dir, Resources/Images fallback) |
| [x] **UriImageSource** | ✅ | `BitmapImage` with `UriSource` + `CacheOption.OnLoad` |
| [x] **StreamImageSource** | ✅ | Async stream → `BitmapImage` via `BeginInit`/`StreamSource`/`EndInit` + `Freeze` |
| [x] **FontImageSource** | ✅ | `FontImageSourceHelper.RenderGlyph()` via FormattedText → DrawingVisual → RenderTargetBitmap |

> **WPF Implementation Note:** WPF `BitmapImage` natively supports URI sources (http/https), file paths, and streams. `UriImageSource` should be straightforward.

---

## 24. Lifecycle Events

| Event | Status | Notes |
|-------|--------|-------|
| [x] **App Launched** | ✅ | `MauiWPFApplication.OnStartup` |
| [x] **App Activated** | ✅ | `LifecycleManager` → `Application.Activated` → `Window.OnActivated` |
| [x] **App Deactivated** | ✅ | `LifecycleManager` → `Application.Deactivated` → `Window.OnDeactivated` |
| [x] **App Terminating** | ✅ | `LifecycleManager` → `Application.Exit` → `Window.OnStopped` + `ThemeManager.Shutdown` |
| [x] **Window Created** | ✅ | Via `ApplicationHandler.CreatePlatformElement` |
| [x] **Window Activated** | ✅ | `LifecycleManager.RegisterWindowLifecycleEvents` → `Window.Activated` |
| [x] **Window Deactivated** | ✅ | `LifecycleManager` → `Window.Deactivated` |
| [x] **Window Closing** | ✅ | `LifecycleManager` → `Window.Closing` → `OnBackgrounding` + `Window.Closed` → `OnDestroying` |

---

## 25. App Theme / Dark Mode

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **System theme detection** | ✅ | `ThemeManager` reads `AppsUseLightTheme` registry key |
| [x] **UserAppTheme** | ✅ | Via `ThemeManager` + `AppInfo.RequestedTheme` |
| [x] **RequestedThemeChanged** | ✅ | `SystemEvents.UserPreferenceChanged` → `ThemeManager.OnUserPreferenceChanged` |
| [x] **AppThemeBinding** | ✅ | Cross-platform MAUI feature — works via property binding system |

> **WPF Implementation Note:** Detect Windows 10+ dark mode via registry key `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize\AppsUseLightTheme`. Listen for `SystemEvents.UserPreferenceChanged` to detect theme changes.

---

## 26. Build System & Resizetizer Integration

| Item Type | Status | Notes |
|-----------|--------|-------|
| [x] **MauiIcon** | ✅ | MSBuild target copies icon to `Resources/` in output directory |
| [x] **MauiImage** | ✅ | MSBuild target copies images to `Resources/Images/` |
| [x] **MauiFont** | ✅ | MSBuild target copies fonts to `Resources/Fonts/` |
| [x] **MauiAsset** | ✅ | MSBuild target copies raw assets to `Resources/Raw/` |
| [x] **MauiSplashScreen** | ✅ | MSBuild target copies splash image to `Resources/` |

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
19. ~~CollectionView~~ ✅ / ListView handlers ❌
20. ~~Picker, DatePicker, TimePicker handlers~~ ✅
21. ~~SearchBar handler~~ ✅
22. ~~RadioButton~~ ✅, ~~Stepper~~ ✅
23. CarouselView, IndicatorView ❌
24. TableView, SwipeView, RefreshView ❌
25. GraphicsView ❌ + ~~ShapeViewHandler~~ ✅ (now with Path/Polygon/Polyline)

### Phase 5: Rich Features 🟡 IN PROGRESS
26. Shell handler ❌
27. ~~WebView handler~~ ✅
28. ~~BlazorWebView handler~~ ✅
29. MenuBar (static helper implemented, not wired to WindowHandler) ⚠️
30. ~~FormattedText (Label spans)~~ ✅
31. ~~Image source types~~ ✅ (File, URI, Stream); FontImage ❌
32. ~~Gesture recognizers (Pan, Swipe, Drag, Drop, LongPress)~~ ✅; Pinch ❌
33. Remaining essentials ❌
34. ~~App Theme / Dark Mode~~ ✅
35. ~~Lifecycle events~~ ✅
36. Build targets / Resizetizer integration ❌

---

## 30. Summary Statistics

| Category | Implemented | Total | Coverage |
|----------|-------------|-------|----------|
| **Core Infrastructure** | 9 of 11 | 11 | 82% |
| **Application & Window** | 8 of 12 | 12 | 67% |
| **Pages** | 4 of 6 | 6 | 67% |
| **Layouts** | 9 of 10 | 10 | 90% |
| **Basic Controls** | 14 of 14 | 14 | 100% |
| **Input Controls** | 4 of 4 | 4 | 100% |
| **Collection Controls** | 1 of 7 | 7 | 14% |
| **Navigation** | 4 of 8 | 8 | 50% |
| **Alerts & Dialogs** | 3 of 3 | 3 | 100% |
| **Gesture Recognizers** | 7 of 8 | 8 | 88% |
| **Graphics & Shapes** | 8 of 10 | 10 | 80% |
| **Base View Properties** | 19 of 22 | 22 | 86% |
| **Font Services** | 4 of 7 | 7 | 57% |
| **Essentials** | 11 of 25 | 25 | 44% |
| **WebView** | 5 of 6 | 6 | 83% |
| **BlazorWebView** | 7 of 7 | 7 | 100% |
| **FormattedText/Spans** | 8 of 9 | 9 | 89% |
| **MenuBar** | 0 of 5 | 5 | 0% |
| **Animations** | 8 of 8 | 8 | 100% |
| **VSM & Triggers** | 5 of 6 | 6 | 83% |
| **ControlTemplate** | 3 of 3 | 3 | 100% |
| **Lifecycle Events** | 8 of 8 | 8 | 100% |
| **Image Source Types** | 3 of 4 | 4 | 75% |
| **App Theme** | 4 of 4 | 4 | 100% |
| **Build/Resizetizer** | 0 of 5 | 5 | 0% |
| | | | |
| **TOTAL** | **160 of 222** | **222** | **72%** |

---

## 31. MauiDevFlow Integration

| Feature | Status | Notes |
|---------|--------|-------|
| [x] **WpfDevFlowAgentService** | ✅ | In `DevFlow/` — visual tree walking, property inspection |
| [x] **Screenshot capture** | ✅ | `RenderTargetBitmap` → PNG |
| [x] **Element tapping** | ✅ | `ButtonAutomationPeer.Invoke()` for button automation |
| [x] **Full element interaction** | ⚠️ | Basic DevFlow agent registered; full element interaction via accessibility APIs planned |
| [x] **Visual tree diff** | ⚠️ | Basic support via MauiDevFlow agent; full diff planned |

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


