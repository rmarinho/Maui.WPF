using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using UITests.Helpers;

namespace UITests;

/// <summary>
/// Captures side-by-side screenshots of WPF and WinUI ControlGallery apps
/// navigating through every page and sub-page, producing comparison images.
/// Both apps are positioned at the same size before capture.
/// 
/// WinUI flyout items lack text in automation tree, so navigation uses
/// SelectionItemPattern by index for flyout pages and mouse clicks for sub-pages.
/// 
/// Usage: dotnet test --filter CompareApps --no-build
/// Output: tests/UITests/Comparisons/*.png
/// </summary>
[Collection("WPF App")]
public class CompareApps
{
    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    const uint SWP_NOZORDER = 0x0004;
    const uint SWP_SHOWWINDOW = 0x0040;

    // Standard window size for comparisons
    const int WinWidth = 1400;
    const int WinHeight = 900;
    const int WinX = 100;
    const int WinY = 50;
    // Move WPF off-screen during WinUI mouse navigation to avoid stealing clicks
    const int OffScreenX = -2000;

    // WinUI flyout page indices (order in Shell)
    static readonly Dictionary<string, int> FlyoutIndex = new()
    {
        ["Home"] = 0, ["Controls"] = 1, ["Layouts"] = 2, ["Features"] = 3
    };

    readonly WpfAppFixture _fixture;
    static readonly string CompareDir;
    static readonly string WinUIExePath = @"D:\repos\davidortinau\ControlGallery\src\ControlGallery\bin\Debug\net10.0-windows10.0.19041.0\win-x64\ControlGallery.exe";

    static CompareApps()
    {
        var repoRoot = FindRepoRoot();
        CompareDir = Path.Combine(repoRoot, "tests", "UITests", "Comparisons");
        Directory.CreateDirectory(CompareDir);
    }

    public CompareApps(WpfAppFixture fixture) => _fixture = fixture;

    // ── Control sub-pages (tiles inside CollectionView on Controls page) ──
    static readonly string[] ControlSubPages = {
        "ActivityIndicator", "Alerts", "Borders", "BoxView", "Buttons",
        "CarouselView", "CheckBoxes", "CollectionView", "Date Picker",
        "Editors", "Entry", "Images", "Labels", "Map", "Picker",
        "Progress Bars", "Radio Buttons", "RefreshView", "SearchBar",
        "Shapes", "Sliders", "Steppers", "Switches", "TimePicker", "WebView"
    };

    // ── Layout sub-pages ──
    static readonly string[] LayoutSubPages = {
        "Basic", "Catalog", "Chessboard", "Proportional", "Combined",
        "CascadeLayout", "Keypad", "Horizontal", "Stylish Header",
        "Bouncing Text", "Login Sample", "Calculator", "Wrapping",
        "Simple Overlay", "Vertical", "Grid Sample", "HorizontalWrapLayout",
        "Achievements Sample", "Margin + Padding",
        "Alignment Sample", "Show Sample", "Photos Sample", "AndExpand",
        "Reading Sample", "Proportional Coordinate Calc", "ColumnLayout",
        "Grow", "Expansion", "Shrink", "Basis", "Simple", "Slider", "Spacing"
    };

    // ── Feature sub-pages ──
    static readonly string[] FeatureSubPages = {
        "Colors", "Gestures", "Clipping", "Behaviors", "Animations",
        "Shadow", "AppThemeBinding", "Triggers",
        "Native Views", "ContextMenu", "HybridWebView", "Tooltips",
        "MenuBar", "FontImageSource"
    };

    // ═══════════════════════════════════════════════════════════════════
    // Compare flyout pages
    // ═══════════════════════════════════════════════════════════════════
    [Theory]
    [InlineData("Home")]
    [InlineData("Controls")]
    [InlineData("Layouts")]
    [InlineData("Features")]
    public void CompareFlyoutPage(string page)
    {
        var winui = EnsureWinUI();
        if (winui == null) { Assert.Fail("WinUI app not available"); return; }

        var wpf = _fixture.GetProcess();

        // Navigate WinUI first (needs foreground for mouse clicks on sub-pages)
        PositionWindow(winui);
        NavigateWinUIFlyout(winui, page);
        Thread.Sleep(1500);

        // Navigate WPF (uses UIAutomation, works without foreground)
        PositionWindow(wpf);
        AutomationHelper.NavigateToPageFresh(wpf, page);
        Thread.Sleep(1000);

        SaveComparison(_fixture.GetProcess(), RefreshWinUI(winui), $"page_{Sanitize(page)}");
    }

    // ═══════════════════════════════════════════════════════════════════
    // Compare control sub-pages
    // ═══════════════════════════════════════════════════════════════════
    [Theory]
    [MemberData(nameof(GetControlSubPages))]
    public void CompareControlSubPage(string control)
    {
        var winui = EnsureWinUI();
        if (winui == null) { Assert.Fail("WinUI app not available"); return; }

        var wpf = _fixture.GetProcess();

        // Move WPF off-screen so WinUI mouse clicks don't land on it
        MoveWindowOffScreen(wpf);
        PositionWindow(winui);

        // Navigate WinUI first (needs foreground for mouse click on sub-page tile)
        NavigateWinUIToSubPage(winui, "Controls", control);
        VerifyWinUINavigation(winui, control);

        // Now navigate WPF (UIAutomation — works without foreground)
        PositionWindow(wpf);
        NavigateWpfToSubPage("Controls", control);

        SaveComparison(_fixture.GetProcess(), RefreshWinUI(winui), $"control_{Sanitize(control)}");
    }

    // ═══════════════════════════════════════════════════════════════════
    // Compare layout sub-pages
    // ═══════════════════════════════════════════════════════════════════
    [Theory]
    [MemberData(nameof(GetLayoutSubPages))]
    public void CompareLayoutSubPage(string layout)
    {
        var winui = EnsureWinUI();
        if (winui == null) { Assert.Fail("WinUI app not available"); return; }

        var wpf = _fixture.GetProcess();
        PositionWindow(wpf);
        PositionWindow(winui);

        NavigateWpfToSubPage("Layouts", layout);
        NavigateWinUIToSubPage(winui, "Layouts", layout);

        SaveComparison(_fixture.GetProcess(), RefreshWinUI(winui), $"layout_{Sanitize(layout)}");
    }

    // ═══════════════════════════════════════════════════════════════════
    // Compare feature sub-pages
    // ═══════════════════════════════════════════════════════════════════
    [Theory]
    [MemberData(nameof(GetFeatureSubPages))]
    public void CompareFeatureSubPage(string feature)
    {
        var winui = EnsureWinUI();
        if (winui == null) { Assert.Fail("WinUI app not available"); return; }

        var wpf = _fixture.GetProcess();
        PositionWindow(wpf);
        PositionWindow(winui);

        NavigateWpfToSubPage("Features", feature);
        NavigateWinUIToSubPage(winui, "Features", feature);

        SaveComparison(_fixture.GetProcess(), RefreshWinUI(winui), $"feature_{Sanitize(feature)}");
    }

    // ═══════════════════════════════════════════════════════════════════
    // Data sources for xUnit
    // ═══════════════════════════════════════════════════════════════════
    public static IEnumerable<object[]> GetControlSubPages() =>
        ControlSubPages.Select(c => new object[] { c });
    public static IEnumerable<object[]> GetLayoutSubPages() =>
        LayoutSubPages.Select(l => new object[] { l });
    public static IEnumerable<object[]> GetFeatureSubPages() =>
        FeatureSubPages.Select(f => new object[] { f });

    // ═══════════════════════════════════════════════════════════════════
    // Window positioning
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Set both WPF and WinUI to the same size and position so captures are comparable.
    /// </summary>
    static void PositionWindow(Process proc)
    {
        proc.Refresh();
        if (proc.MainWindowHandle == IntPtr.Zero) return;
        SetWindowPos(proc.MainWindowHandle, IntPtr.Zero, WinX, WinY, WinWidth, WinHeight, SWP_NOZORDER | SWP_SHOWWINDOW);
        Thread.Sleep(300);
    }

    // ═══════════════════════════════════════════════════════════════════
    // WPF navigation
    // ═══════════════════════════════════════════════════════════════════

    void NavigateWpfToSubPage(string section, string subPage)
    {
        var proc = _fixture.GetProcess();
        AutomationHelper.NavigateToPageFresh(proc, section);
        Thread.Sleep(1000);
        proc = _fixture.GetProcess();
        var root = AutomationHelper.GetRoot(proc);
        AutomationHelper.ClickButtonWithScroll(root, subPage);
        Thread.Sleep(2000);
    }

    // ═══════════════════════════════════════════════════════════════════
    // WinUI navigation — flyout uses SelectionItemPattern by index,
    // sub-pages use mouse click (no InvokePattern available)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Navigate WinUI to a flyout page by selecting the Nth list item.
    /// Goes to Home first to reset the nav stack (same as WPF NavigateToPageFresh).
    /// </summary>
    static void NavigateWinUIFlyout(Process winui, string page)
    {
        winui.Refresh();
        if (winui.MainWindowHandle == IntPtr.Zero) return;
        var root = AutomationElement.FromHandle(winui.MainWindowHandle);

        var flyoutItems = GetWinUIFlyoutItems(root);
        if (flyoutItems.Count == 0) return;

        // Reset to Home first if not already targeting Home
        if (page != "Home" && FlyoutIndex.ContainsKey("Home"))
        {
            SelectFlyoutItem(flyoutItems, 0);
            Thread.Sleep(1000);
        }

        if (FlyoutIndex.TryGetValue(page, out int idx) && idx < flyoutItems.Count)
        {
            SelectFlyoutItem(flyoutItems, idx);
            Thread.Sleep(1500);
        }
    }

    /// <summary>
    /// Navigate WinUI to a sub-page: flyout first, then mouse-click the target text.
    /// </summary>
    static void NavigateWinUIToSubPage(Process winui, string section, string subPage)
    {
        // Navigate to section flyout page first
        NavigateWinUIFlyout(winui, section);
        Thread.Sleep(1000);

        // Now find and click the sub-page by text — must use mouse since WinUI
        // CollectionView items have no InvokePattern/SelectionItemPattern
        winui.Refresh();
        if (winui.MainWindowHandle == IntPtr.Zero) return;

        SetForegroundWindow(winui.MainWindowHandle);
        Thread.Sleep(300);

        var root = AutomationElement.FromHandle(winui.MainWindowHandle);

        // Try to find the target text, scrolling if needed
        if (!ClickWinUITextElement(root, subPage))
        {
            // Try scrolling to find it
            ScrollAndClick(root, subPage);
        }
        Thread.Sleep(2000);
    }

    /// <summary>
    /// Find a text element by name and click at its center via mouse.
    /// </summary>
    static bool ClickWinUITextElement(AutomationElement root, string text)
    {
        var elements = root.FindAll(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.NameProperty, text));

        foreach (AutomationElement el in elements)
        {
            var rect = el.Current.BoundingRectangle;
            if (rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0) continue;

            // Click at center of the element
            AutomationHelper.ClickAt(rect);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Scroll through lists looking for the target, then click it.
    /// </summary>
    static void ScrollAndClick(AutomationElement root, string text)
    {
        var scrollables = root.FindAll(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.List));

        foreach (AutomationElement scrollable in scrollables)
        {
            try
            {
                var scroll = (ScrollPattern)scrollable.GetCurrentPattern(ScrollPattern.Pattern);
                // Reset scroll
                scroll.SetScrollPercent(ScrollPattern.NoScroll, 0);
                Thread.Sleep(300);

                for (int i = 0; i < 15; i++)
                {
                    if (ClickWinUITextElement(root, text))
                        return;
                    if (scroll.Current.VerticalScrollPercent >= 100) break;
                    scroll.SetScrollPercent(ScrollPattern.NoScroll,
                        Math.Min(100, scroll.Current.VerticalScrollPercent + 15));
                    Thread.Sleep(300);
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Get all flyout ListItem elements from WinUI (NavigationViewItems).
    /// </summary>
    static List<AutomationElement> GetWinUIFlyoutItems(AutomationElement root)
    {
        var items = root.FindAll(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem));
        var result = new List<AutomationElement>();
        foreach (AutomationElement item in items)
        {
            // WinUI flyout items are NavigationViewItem with ShellFlyoutItemView name
            if (item.Current.Name?.Contains("ShellFlyoutItemView") == true ||
                item.Current.ClassName?.Contains("NavigationViewItem") == true)
            {
                result.Add(item);
            }
        }
        return result;
    }

    static void SelectFlyoutItem(List<AutomationElement> items, int index)
    {
        if (index >= items.Count) return;
        try
        {
            var sip = (SelectionItemPattern)items[index].GetCurrentPattern(SelectionItemPattern.Pattern);
            sip.Select();
        }
        catch { }
    }

    // ═══════════════════════════════════════════════════════════════════
    // Comparison image creation
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Creates a side-by-side comparison image: WPF on left, WinUI on right.
    /// Both windows should be at the same size before calling this.
    /// </summary>
    static void SaveComparison(Process wpfProc, Process winuiProc, string name)
    {
        using var wpfBmp = ScreenshotHelper.CaptureWindow(wpfProc);
        using var winuiBmp = ScreenshotHelper.CaptureWindow(winuiProc);

        ScreenshotHelper.SaveScreenshot(wpfBmp, CompareDir, $"{name}_wpf");
        ScreenshotHelper.SaveScreenshot(winuiBmp, CompareDir, $"{name}_winui");

        int targetH = Math.Max(wpfBmp.Height, winuiBmp.Height);
        int totalW = wpfBmp.Width + winuiBmp.Width + 20;
        using var combined = new Bitmap(totalW, targetH + 40);
        using var g = Graphics.FromImage(combined);
        g.Clear(Color.FromArgb(30, 30, 30));

        using var font = new Font("Segoe UI", 14, FontStyle.Bold);
        g.DrawString("WPF", font, Brushes.Cyan, 10, 5);
        g.DrawString("WinUI", font, Brushes.Orange, wpfBmp.Width + 20, 5);

        g.DrawImage(wpfBmp, 0, 35, wpfBmp.Width, wpfBmp.Height);
        g.DrawImage(winuiBmp, wpfBmp.Width + 20, 35, winuiBmp.Width, winuiBmp.Height);

        using var pen = new Pen(Color.Yellow, 2);
        g.DrawLine(pen, wpfBmp.Width + 9, 0, wpfBmp.Width + 9, combined.Height);

        combined.Save(Path.Combine(CompareDir, $"{name}_compare.png"), ImageFormat.Png);
    }

    // ═══════════════════════════════════════════════════════════════════
    // Process management
    // ═══════════════════════════════════════════════════════════════════

    static Process? _cachedWinUI;

    static Process? EnsureWinUI()
    {
        if (_cachedWinUI != null)
        {
            try { _cachedWinUI.Refresh(); if (!_cachedWinUI.HasExited) return _cachedWinUI; } catch { }
        }

        _cachedWinUI = FindWinUIProcess();
        if (_cachedWinUI != null) return _cachedWinUI;

        if (!File.Exists(WinUIExePath)) return null;
        Process.Start(new ProcessStartInfo { FileName = WinUIExePath, UseShellExecute = false });
        for (int i = 0; i < 30; i++)
        {
            Thread.Sleep(1000);
            _cachedWinUI = FindWinUIProcess();
            if (_cachedWinUI != null) return _cachedWinUI;
        }
        return null;
    }

    static Process? FindWinUIProcess()
    {
        foreach (var p in Process.GetProcessesByName("ControlGallery"))
        {
            try
            {
                var path = p.MainModule?.FileName ?? "";
                if (path.Contains("davidortinau", StringComparison.OrdinalIgnoreCase) ||
                    path.Contains("net10.0-windows10.0", StringComparison.OrdinalIgnoreCase))
                {
                    p.Refresh();
                    if (p.MainWindowHandle != IntPtr.Zero)
                        return p;
                }
            }
            catch { }
        }
        return null;
    }

    static Process RefreshWinUI(Process winui)
    {
        winui.Refresh();
        if (winui.MainWindowHandle != IntPtr.Zero) return winui;
        return FindWinUIProcess() ?? winui;
    }

    static string Sanitize(string name) =>
        name.Replace(" ", "_").Replace("+", "plus").ToLowerInvariant();

    static string FindRepoRoot()
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir, "Maui.WPF.sln")))
                return dir;
            dir = Path.GetDirectoryName(dir);
        }
        return @"D:\repos\rmarinho\maui.wpf";
    }
}
