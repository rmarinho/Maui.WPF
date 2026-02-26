# Platform.Maui.WPF — Backend Implementation Checklist

Implementation status of the .NET MAUI backend for Windows using WPF. Adapted from the generic MAUI backend checklist.

> ✅ = Implemented  |  ⚠️ = Partial  |  ❌ = Not implemented  |  N/A = Not applicable to WPF

---

## 1. Core Infrastructure

### Platform Abstractions
- [x] **Platform View Type** — `FrameworkElement` as the native view abstraction via `WPFViewHandler<TVirtualView, TPlatformView>`
- [x] **Platform Context** — `IMauiContext` with DI scope, handler factory, window scope management
- [x] **Dispatcher** — `WPFDispatcherProvider` using WPF's `System.Windows.Threading.Dispatcher`
- [x] **Event System** — WPF routed events for user interactions (clicks, input, gestures)
- [x] **Handler Factory Integration** — All handlers registered via `AddMauiControlsHandlers()` in `AppHostBuilderExtensions`
- [x] **App Host Builder Extension** — `UseMauiAppWPF<T>()` wires up all handlers and services

### Rendering Pipeline
- [x] **View Renderer** — `WPFViewHandler` base traverses virtual view tree, creates/connects WPF controls
- [x] **Property Change Propagation** — `IPropertyMapper` re-maps views when `IView` property changes fire
- [x] **Child Synchronization** — `LayoutPanel` handles add/remove/reorder of child views
- [x] **Style/Attribute Application** — Direct property setting on WPF controls + ViewMapper overrides for base properties

### WPF Interop
- [x] **Routed Events** — WPF routed event handling for Click, TextChanged, etc.
- [ ] **Keyboard Event Handling** — Entry/Editor key events for ReturnType
- [x] **Scroll Handling** — `System.Windows.Controls.ScrollViewer` for scroll containers

### Known Issues
- ⚠️ `LayoutPanel` previously had hardcoded 1000x1000 dimensions — fixed, but complex nested layouts may still have sizing edge cases
- ⚠️ Type ambiguity between `System.Windows` and `Microsoft.Maui` namespaces (CornerRadius, Thickness, Color, etc.) — must use `global::` or `using` aliases

---

## 2. Application & Window

| Control | Status | Notes |
|---------|--------|-------|
| ✅ **Application** | Done | App lifecycle via `MauiWPFApplication`, `System.Windows.Application` integration |
| ✅ **Window** | Done | Title, Width/Height, X/Y, MinWidth/MinHeight, MaxWidth/MaxHeight mapping |

### Known Issues
- ⚠️ Window hidden/disabled flags reported by DevFlow — may be WPF window state mapping issue

---

## 3. Pages

| Page | Status | Notes |
|------|--------|-------|
| ✅ **ContentPage** | Done | Content rendering, Title (updates WPF window title), Background |
| ❌ **NavigationPage** | Not implemented | No handler — using page-swapping workaround (`Application.Current.Windows[0].Page = ...`) |
| ❌ **TabbedPage** | Not implemented | |
| ❌ **FlyoutPage** | Not implemented | |
| ❌ **Shell** | Not implemented | No flyout, tab bars, or shell navigation |

### Known Issues
- ⚠️ Navigation uses page-swapping instead of proper NavigationPage stack — no back button, no animation

---

## 4. Layouts

| Layout | Status | Notes |
|--------|--------|-------|
| ✅ **VerticalStackLayout** | Done | `LayoutPanel` with `CrossPlatformMeasure`/`CrossPlatformArrange` |
| ✅ **HorizontalStackLayout** | Done | `LayoutPanel` with horizontal layout |
| ✅ **Grid** | Done | Row/column definitions, spans, spacing — MAUI layout engine handles positioning |
| ✅ **FlexLayout** | Done | Handled by generic `LayoutHandler` — MAUI cross-platform layout engine |
| ✅ **AbsoluteLayout** | Done | Handled by generic `LayoutHandler` |
| ✅ **ScrollView** | Done | `System.Windows.Controls.ScrollViewer` with orientation, `ScrollBarVisibility`, `ScrollToAsync`, `Scrolled` event |
| ✅ **ContentView** | Done | `ContentPanel` wrapper with cross-platform measure/arrange |
| ✅ **Border** | Done | Stroke, StrokeThickness, StrokeShape (RoundRectangle → CornerRadius), Background, Padding |
| ❌ **Frame** | Not implemented | Legacy — could map to `BorderHandler` |

---

## 5. Basic Controls

| Control | Status | Notes |
|---------|--------|-------|
| ✅ **Label** | Done | Text, TextColor, Font (Size/Weight/Style/Family), HorizontalTextAlignment, VerticalTextAlignment, TextDecorations (Underline/Strikethrough), Padding, LineHeight, MaxLines, FormattedText with per-Span styling |
| ✅ **Button** | Done | Text, TextColor, Font, Padding, Background, StrokeColor, StrokeThickness, CornerRadius (stub), Clicked event |
| ❌ **ImageButton** | Not implemented | |
| ✅ **Entry** | Done | Text, TextColor, Font, IsReadOnly, MaxLength, HorizontalTextAlignment, Background, TextChanged |
| ✅ **Editor** | Done | Text, TextColor, Font, IsReadOnly, MaxLength, HorizontalTextAlignment, AcceptsReturn, TextWrapping |
| ✅ **Switch** | Done | Custom `ToggleButton` template with green/gray track, white thumb, animated thumb position |
| ✅ **CheckBox** | Done | `System.Windows.Controls.CheckBox` — IsChecked, Foreground color |
| ❌ **RadioButton** | Not implemented | |
| ✅ **Slider** | Done | `System.Windows.Controls.Slider` — Value, Min, Max, MinimumTrackColor, MaximumTrackColor |
| ✅ **Stepper** | Done | Custom StackPanel with ±buttons — Value, Min, Max, Increment, value label |
| ✅ **ProgressBar** | Done | `System.Windows.Controls.ProgressBar` — Progress (0.0–1.0 mapped to 0–100), ProgressColor |
| ✅ **ActivityIndicator** | Done | `System.Windows.Controls.ProgressBar` (IsIndeterminate) — IsRunning, Color |
| ✅ **BoxView** | Done | `System.Windows.Controls.Border` — Color (via Fill/Background), CornerRadius |
| ✅ **Image** | Done | Source (URL/file), Aspect (via Stretch) |

### Known Issues
- ⚠️ Button: WPF default chrome overrides Background when using system theme — `BackgroundColor="Transparent"` doesn't look fully transparent
- ⚠️ Button: `CornerRadius` requires custom ControlTemplate (currently a stub)
- ⚠️ Entry/Editor: No native placeholder text support — WPF TextBox lacks a built-in Watermark property
- ⚠️ Entry: `IsPassword` not implemented — would need `PasswordBox` swap
- ⚠️ Slider: `ThumbColor` not directly available on WPF Slider without restyling
- ⚠️ Switch: `TrackColor`/`ThumbColor` require ControlTemplate modification (stubs)

---

## 6. Input & Selection Controls

| Control | Status | Notes |
|---------|--------|-------|
| ✅ **Picker** | Done | `System.Windows.Controls.ComboBox` — ItemsSource, SelectedIndex, Title, TextColor, Font, Background |
| ✅ **DatePicker** | Done | `System.Windows.Controls.DatePicker` — Date, MinimumDate, MaximumDate, TextColor, Font |
| ✅ **TimePicker** | Done | Custom TextBox — Time (nullable TimeSpan in MAUI 10), TextColor, Font, Format |
| ✅ **SearchBar** | Done | `System.Windows.Controls.TextBox` — Text, TextColor, Font, MaxLength, HorizontalTextAlignment, Enter key for SearchButtonPressed |

### Known Issues
- ⚠️ DatePicker/TimePicker: `Format` property partially supported
- ⚠️ SearchBar: No native search icon or clear button
- ⚠️ Picker: `Title` as placeholder when no selection (basic implementation)

---

## 7. Collection Controls

| Control | Status | Notes |
|---------|--------|-------|
| ❌ **CollectionView** | Not implemented | |
| ❌ **ListView** | Not implemented | |
| ❌ **CarouselView** | Not implemented | |
| ❌ **IndicatorView** | Not implemented | |
| ❌ **TableView** | Not implemented | |
| ❌ **SwipeView** | Not implemented | |
| ❌ **RefreshView** | Not implemented | |

---

## 8. Navigation & Routing

| Feature | Status | Notes |
|---------|--------|-------|
| ❌ **NavigationPage stack** | Not implemented | Using page-swapping workaround |
| ❌ **Shell navigation** | Not implemented | |
| ❌ **Back button** | Not implemented | |
| ❌ **ToolbarItems** | Not implemented | |
| N/A **URL/route-based routing** | N/A | Desktop-specific concept |

---

## 9. Alerts & Dialogs

| Dialog | Status | Notes |
|--------|--------|-------|
| ❌ **DisplayAlert** | Not implemented | Could use `System.Windows.MessageBox` or custom WPF Window |
| ❌ **DisplayActionSheet** | Not implemented | |
| ❌ **DisplayPromptAsync** | Not implemented | |

---

## 10. Gesture Recognizers

| Gesture | Status | Notes |
|---------|--------|-------|
| ❌ **TapGestureRecognizer** | Not implemented | Could use WPF `MouseLeftButtonDown` |
| ❌ **PanGestureRecognizer** | Not implemented | Could use WPF `MouseMove` + `MouseDown` |
| ❌ **SwipeGestureRecognizer** | Not implemented | |
| ❌ **PinchGestureRecognizer** | Not implemented | Could use WPF `ManipulationDelta` for touch |
| ❌ **PointerGestureRecognizer** | Not implemented | Could use WPF `MouseEnter`/`MouseLeave`/`MouseMove` |

---

## 11. Graphics & Shapes

### Microsoft.Maui.Graphics
| Feature | Status | Notes |
|---------|--------|-------|
| ❌ **GraphicsView** | Not implemented | Could use WPF `DrawingVisual` or `WriteableBitmap` |
| ❌ **Canvas Operations** | Not implemented | |
| ❌ **Brushes** | Not implemented | WPF has excellent native brush support |

### Shapes
| Shape | Status | Notes |
|-------|--------|-------|
| ✅ **Rectangle** | Done | Via `ShapeViewHandler` with `Border` + `CornerRadius` |
| ✅ **Ellipse** | Done | Via `ShapeViewHandler` with `Border` + large `CornerRadius` |
| ✅ **Line** | Done | Via `ShapeViewHandler` with `System.Windows.Shapes.Line` |
| ❌ **Path** | Not implemented | WPF has native `System.Windows.Shapes.Path` |
| ❌ **Polygon** | Not implemented | WPF has native `System.Windows.Shapes.Polygon` |
| ❌ **Polyline** | Not implemented | WPF has native `System.Windows.Shapes.Polyline` |
| ⚠️ **Fill & Stroke** | Partial | Basic SolidColorBrush fill/stroke. Missing: dash patterns, line cap/join |

---

## 12. Common View Properties (Base Handler)

Every handler must support these properties mapped from the base `IView`:

### Visibility & State
- [x] Opacity — ViewMapper override → `UIElement.Opacity`
- [x] IsVisible — ViewMapper override → `UIElement.Visibility` (Visible/Collapsed/Hidden)
- [x] IsEnabled — ViewMapper override → `UIElement.IsEnabled`
- [ ] InputTransparent — Not mapped

### Sizing
- [x] WidthRequest / HeightRequest — ViewMapper override → `FrameworkElement.Width/Height`
- [x] MinimumWidthRequest / MinimumHeightRequest — ViewMapper override → `FrameworkElement.MinWidth/MinHeight`
- [x] MaximumWidthRequest / MaximumHeightRequest — ViewMapper override → `FrameworkElement.MaxWidth/MaxHeight`

### Layout
- [x] HorizontalOptions / VerticalOptions — Handled by MAUI cross-platform layout
- [x] Margin — ViewMapper override → `FrameworkElement.Margin`
- [x] Padding — Mapped on individual handlers (Entry, Editor, Button, Border, Label)
- [ ] FlowDirection (LTR, RTL) — Not mapped
- [ ] ZIndex — Not mapped

### Appearance
- [x] BackgroundColor / Background — ViewMapper override for `Control.Background` and `Panel.Background`

### Transforms
- [ ] TranslationX / TranslationY — Not mapped (WPF has `RenderTransform`)
- [ ] Rotation / RotationX / RotationY — Not mapped
- [ ] Scale / ScaleX / ScaleY — Not mapped
- [ ] AnchorX / AnchorY — Not mapped

### Effects
- [ ] Shadow — Not mapped (WPF has `DropShadowEffect`)
- [ ] Clip — Not mapped (WPF has `UIElement.Clip`)

### Automation
- [ ] AutomationId — Not mapped (WPF has `AutomationProperties.AutomationId`)
- [ ] Semantic properties — Not mapped (WPF has `AutomationProperties`)

### Interactivity Attachments
- [ ] **ToolTip** — Not mapped (WPF has `ToolTipService.ToolTip`)
- [ ] **ContextFlyout** — Not mapped (WPF has `ContextMenu`)

---

## 13. VisualStateManager & Triggers

| Feature | Status | Notes |
|---------|--------|-------|
| ⚠️ **VisualStateManager** | Partial | Cross-platform MAUI feature — works without platform code, but hover/pressed states may not trigger without gesture support |
| ✅ **PropertyTrigger** | Done | Cross-platform MAUI feature |
| ✅ **DataTrigger** | Done | Cross-platform MAUI feature |
| ✅ **MultiTrigger** | Done | Cross-platform MAUI feature |
| ✅ **EventTrigger** | Done | Cross-platform MAUI feature |
| ✅ **Behaviors** | Done | Cross-platform MAUI feature |

---

## 14. Font Management

| Feature | Status | Notes |
|---------|--------|-------|
| ❌ **IFontManager** | Not implemented | Need `WPFFontManager` to resolve `Font` → `System.Windows.Media.FontFamily` |
| ❌ **IFontRegistrar** | Not implemented | Need font registration from embedded resources |
| ❌ **IEmbeddedFontLoader** | Not implemented | |
| ⚠️ **Font properties** | Partial | FontSize, FontWeight, FontStyle, FontFamily applied per-handler but no central font management |
| ❌ **IFontNamedSizeService** | Not implemented | Maps `NamedSize` enum to WPF point sizes |
| ❌ **FontImageSource** | Not implemented | Font glyph rendering as images |

---

## 15. Essentials / Platform Services

| Service | Interface | Status | Notes |
|---------|-----------|--------|-------|
| ✅ **App Info** | `IAppInfo` | Done | Assembly-based name, version, package info |
| ✅ **Browser** | `IBrowser` | Done | `Process.Start` for URL opening |
| ✅ **Clipboard** | `IClipboard` | Done | `System.Windows.Clipboard` |
| ✅ **Connectivity** | `IConnectivity` | Done | `System.Net.NetworkInformation` |
| ✅ **Device Display** | `IDeviceDisplay` | Done | `SystemParameters` screen dimensions |
| ✅ **Device Info** | `IDeviceInfo` | Done | Windows platform info |
| ✅ **Email** | `IEmail` | Done | `mailto:` protocol |
| ✅ **File Picker** | `IFilePicker` | Done | `Microsoft.Win32.OpenFileDialog` |
| ✅ **File System** | `IFileSystem` | Done | `Environment.SpecialFolder` paths |
| ✅ **Launcher** | `ILauncher` | Done | `Process.Start` for URI launching |
| ✅ **Map** | `IMap` | Done | Opens Bing Maps via URL |
| ✅ **Preferences** | `IPreferences` | Done | `IsolatedStorageFile`-based key-value |
| ✅ **Secure Storage** | `ISecureStorage` | Done | `ProtectedData` (DPAPI) encrypted storage |
| ✅ **Share** | `IShare` | Done | Clipboard-based sharing (desktop) |
| ✅ **Version Tracking** | `IVersionTracking` | Done | Preferences-based tracking |
| ❌ **Geolocation** | `IGeolocation` | Stub | Not applicable to most desktop scenarios |
| ❌ **Media Picker** | `IMediaPicker` | Stub | Basic file picker only |
| ❌ **Screenshot** | `IScreenshot` | Stub | Could use `RenderTargetBitmap` |
| ❌ **Semantic Screen Reader** | `ISemanticScreenReader` | Stub | Could use UI Automation |
| ❌ **Text-to-Speech** | `ITextToSpeech` | Stub | Could use `System.Speech.Synthesis` |
| ❌ **Vibration** | `IVibration` | Stub | N/A for desktop |
| ❌ **Sensors** | Various | Stub | Accelerometer, Barometer, Compass, Gyroscope — N/A for desktop |
| ❌ **Phone Dialer** | `IPhoneDialer` | Stub | N/A for desktop |
| ❌ **SMS** | `ISms` | Stub | N/A for desktop |
| ❌ **Battery** | `IBattery` | Stub | Could use `SystemInformation.PowerStatus` |

---

## 16. Styling Infrastructure

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **Border style mapping** | Done | Stroke → `BorderBrush`, StrokeShape → `CornerRadius` |
| ✅ **View state mapping** | Done | IsVisible → Visibility, IsEnabled → IsEnabled, Opacity via ViewMapper |
| ❌ **Automation mapping** | Not implemented | `AutomationId` → `AutomationProperties.AutomationId` |
| ❌ **WPF Theme integration** | Not implemented | System theme detection, dark/light mode |

---

## 17. WebView

| Feature | Status | Notes |
|---------|--------|-------|
| ❌ **URL loading** | Not implemented | Could use `WebView2` (Microsoft Edge WebView2) |
| ❌ **HTML content** | Not implemented | |
| ❌ **JavaScript execution** | Not implemented | |
| ❌ **Navigation events** | Not implemented | |
| ❌ **GoBack / GoForward** | Not implemented | |
| ✅ **BlazorWebView** | Done | Basic BlazorWebView handler exists |

---

## 18. Label — FormattedText Detail

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **Span rendering** | Done | WPF `Inlines` collection with `Run` elements |
| ✅ **Span.Text** | Done | `Run.Text` |
| ✅ **Span.TextColor** | Done | `Run.Foreground` via `SolidColorBrush` |
| ✅ **Span.BackgroundColor** | Done | `Run.Background` via `SolidColorBrush` |
| ✅ **Span.FontSize** | Done | `Run.FontSize` |
| ✅ **Span.FontFamily** | Done | `Run.FontFamily` |
| ✅ **Span.FontAttributes** | Done | `Run.FontWeight` (Bold) / `Run.FontStyle` (Italic) |
| ✅ **Span.TextDecorations** | Done | `Run.TextDecorations` (Underline/Strikethrough) |
| ❌ **Span.CharacterSpacing** | Not available | WPF `Run` lacks character spacing property |

---

## 19. MenuBar (Desktop)

| Feature | Status | Notes |
|---------|--------|-------|
| ❌ **MenuBarItem** | Not implemented | WPF has native `Menu` + `MenuItem` |
| ❌ **MenuFlyoutItem** | Not implemented | |
| ❌ **MenuFlyoutSeparator** | Not implemented | WPF has `Separator` |
| ❌ **Keyboard accelerators** | Not implemented | WPF has `InputGestureCollection` |

---

## 20. Lifecycle Events

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **OnStartup** | Done | `MauiWPFApplication.OnStartup` |
| ❌ **OnActivated / OnDeactivated** | Not implemented | WPF `Application.Activated`/`Deactivated` |
| ❌ **OnExit** | Not implemented | WPF `Application.Exit` |
| ❌ **Window lifecycle** | Not implemented | WPF `Window.Closing`/`Window.Closed` |
| ❌ **ConfigureLifecycleEvents** | Not implemented | No lifecycle event extension methods |

---

## 21. Animations

| Feature | Status | Notes |
|---------|--------|-------|
| ❌ **PlatformTicker** | Not implemented | Need `WPFTicker` using `DispatcherTimer` for ~60fps animation frames |
| ❌ **TranslateTo** | Not implemented | Requires ticker + TranslationX/Y mapping |
| ❌ **FadeTo** | Not implemented | Requires ticker + Opacity mapping (Opacity is mapped) |
| ❌ **ScaleTo** | Not implemented | Requires ticker + Scale mapping via `RenderTransform` |
| ❌ **RotateTo** | Not implemented | Requires ticker + Rotation mapping via `RenderTransform` |
| ❌ **LayoutTo** | Not implemented | |

> Note: WPF has excellent native animation support via `Storyboard` and `DoubleAnimation`. A `WPFTicker` using `DispatcherTimer` on the UI thread would enable MAUI's cross-platform animation system.

---

## 22. ControlTemplate & ContentPresenter

| Feature | Status | Notes |
|---------|--------|-------|
| ✅ **ControlTemplate** | Done | Cross-platform MAUI feature — no platform code needed |
| ✅ **ContentPresenter** | Done | Cross-platform MAUI feature |
| ✅ **TemplatedView** | Done | Cross-platform MAUI feature |

---

## Summary Statistics

| Category | Implemented | Total | Notes |
|----------|-------------|-------|-------|
| **Core Infrastructure** | 5 of 7 | 7 | Missing keyboard events, gesture integration |
| **Pages** | 1 of 5 | 5 | Only ContentPage; NavigationPage, TabbedPage, FlyoutPage, Shell needed |
| **Layouts** | 8 of 9 | 9 | Missing Frame (could reuse BorderHandler) |
| **Basic Controls** | 14 of 16 | 16 | Missing ImageButton, RadioButton |
| **Collection Controls** | 0 of 7 | 7 | Major gap — CollectionView, ListView critical |
| **Input Controls** | 4 of 4 | 4 | All present; Entry/Editor could be improved |
| **Gesture Recognizers** | 0 of 5 | 5 | Major gap — Tap essential for interactivity |
| **Shapes** | 3 of 6 | 6 | Missing Path, Polygon, Polyline |
| **Essentials** | 15 of 26 | 26 | Core services done; sensors/phone N/A |
| **Alerts & Dialogs** | 0 of 3 | 3 | Critical gap for user interaction |
| **Font Services** | 0 of 5 | 5 | Need IFontManager, IFontRegistrar |
| **Animations** | 0 of 5 | 5 | Need PlatformTicker + Transform mapping |
| **MenuBar** | 0 of 4 | 4 | WPF has excellent native menu support |
| **FormattedText** | 8 of 9 | 9 | Missing CharacterSpacing (WPF limitation) |
| **Base View Properties** | 9 of 22 | 22 | Missing transforms, clip, shadow, automation |
| **VSM & Triggers** | 5 of 6 | 6 | VSM partial without gesture/pointer support |
| **Lifecycle** | 1 of 5 | 5 | Only OnStartup; need full lifecycle |
| **WebView** | 1 of 6 | 6 | Only BlazorWebView; WebView2 needed for full WebView |

---

## Priority Implementation Roadmap

### P0 — Critical (needed for basic apps)
1. **NavigationPage** — Push/Pop stack, back button, toolbar
2. **DisplayAlert / DisplayActionSheet** — User feedback
3. **TapGestureRecognizer** — Button-like tappable areas
4. **CollectionView / ListView** — Data-driven lists
5. **PlatformTicker + Animations** — Visual feedback

### P1 — Important (needed for real apps)
6. **Shell** — Full navigation with flyout, tabs
7. **TabbedPage / FlyoutPage** — Multi-page navigation patterns
8. **PointerGestureRecognizer** — Hover effects
9. **PanGestureRecognizer / SwipeGestureRecognizer** — Touch/swipe interactions
10. **IFontManager / IFontRegistrar** — Custom font loading
11. **MenuBar** — Desktop menu integration
12. **Transforms** (TranslationX/Y, Scale, Rotation) — Visual effects
13. **Shadow / Clip** — Visual polish
14. **WebView** (WebView2) — Embedded web content
15. **AutomationId / Semantics** — Accessibility

### P2 — Polish (nice to have)
16. **RadioButton / ImageButton** — Additional controls
17. **Path / Polygon / Polyline** — Advanced shapes
18. **GraphicsView** — Custom drawing
19. **Frame** — Legacy border support
20. **Lifecycle events** — Full app lifecycle
21. **Theme integration** — Dark/light mode
22. **ToolTip / ContextFlyout** — Desktop UX patterns
23. **CarouselView / SwipeView / RefreshView** — Advanced collection patterns
24. **Screenshot / Text-to-Speech** — Additional essentials
25. **Entry placeholder watermark** — Custom TextBox template with placeholder
26. **Button CornerRadius** — Custom Button ControlTemplate
