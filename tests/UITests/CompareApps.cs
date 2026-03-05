using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using UITests.Helpers;

namespace UITests;

/// <summary>
/// Captures side-by-side screenshots of WPF and WinUI ControlGallery apps
/// navigating through every page and sub-page, producing comparison images.
/// 
/// Usage: dotnet test --filter CompareApps --no-build
/// Output: tests/UITests/Comparisons/*.png
/// </summary>
[Collection("WPF App")]
public class CompareApps
{
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

    // ── Flyout pages ──────────────────────────────────────────────────
    static readonly string[] FlyoutPages = { "Home", "Controls", "Layouts", "Features" };

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

        // Navigate WPF
        var wpf = _fixture.GetProcess();
        AutomationHelper.NavigateToPageFresh(wpf, page);
        Thread.Sleep(1500);
        wpf = _fixture.GetProcess();

        // Navigate WinUI
        NavigateWinUI(winui, page);
        Thread.Sleep(1500);
        winui = RefreshWinUI(winui);

        SaveComparison(wpf, winui, $"page_{Sanitize(page)}");
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

        NavigateToSubPage(_fixture.GetProcess(), "Controls", control);
        var wpf = _fixture.GetProcess();

        NavigateWinUIToSubPage(winui, "Controls", control);
        winui = RefreshWinUI(winui);

        SaveComparison(wpf, winui, $"control_{Sanitize(control)}");
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

        NavigateToSubPage(_fixture.GetProcess(), "Layouts", layout);
        var wpf = _fixture.GetProcess();

        NavigateWinUIToSubPage(winui, "Layouts", layout);
        winui = RefreshWinUI(winui);

        SaveComparison(wpf, winui, $"layout_{Sanitize(layout)}");
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

        NavigateToSubPage(_fixture.GetProcess(), "Features", feature);
        var wpf = _fixture.GetProcess();

        NavigateWinUIToSubPage(winui, "Features", feature);
        winui = RefreshWinUI(winui);

        SaveComparison(wpf, winui, $"feature_{Sanitize(feature)}");
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
    // Core helpers
    // ═══════════════════════════════════════════════════════════════════

    void NavigateToSubPage(Process wpfProc, string section, string subPage)
    {
        AutomationHelper.NavigateToPageFresh(wpfProc, section);
        Thread.Sleep(1000);
        var proc = _fixture.GetProcess();
        var root = AutomationHelper.GetRoot(proc);
        AutomationHelper.ClickButtonWithScroll(root, subPage);
        Thread.Sleep(2000);
    }

    static void NavigateWinUI(Process winui, string page)
    {
        try
        {
            winui.Refresh();
            if (winui.MainWindowHandle == IntPtr.Zero) return;
            var root = AutomationElement.FromHandle(winui.MainWindowHandle);
            AutomationHelper.ClickFlyoutItem(root, page);
        }
        catch { }
    }

    static void NavigateWinUIToSubPage(Process winui, string section, string subPage)
    {
        try
        {
            NavigateWinUI(winui, section);
            Thread.Sleep(1500);
            winui.Refresh();
            if (winui.MainWindowHandle == IntPtr.Zero) return;
            var root = AutomationElement.FromHandle(winui.MainWindowHandle);
            // Try scroll-aware click
            AutomationHelper.ClickButtonWithScroll(root, subPage);
            Thread.Sleep(2000);
        }
        catch { }
    }

    /// <summary>
    /// Creates a side-by-side comparison image: WPF on left, WinUI on right.
    /// Saves both individual and combined images.
    /// </summary>
    static void SaveComparison(Process wpfProc, Process winuiProc, string name)
    {
        // WPF: use PrintWindow (reliable, headless-capable)
        using var wpfBmp = ScreenshotHelper.CaptureWindow(wpfProc);
        // WinUI: use BitBlt from screen (PrintWindow returns black for DirectComposition)
        using var winuiBmp = ScreenshotHelper.CaptureWindowBitBlt(winuiProc);

        // Save individual
        ScreenshotHelper.SaveScreenshot(wpfBmp, CompareDir, $"{name}_wpf");
        ScreenshotHelper.SaveScreenshot(winuiBmp, CompareDir, $"{name}_winui");

        // Create side-by-side (normalize heights)
        int targetH = Math.Max(wpfBmp.Height, winuiBmp.Height);
        int totalW = wpfBmp.Width + winuiBmp.Width + 20; // 20px gap
        using var combined = new Bitmap(totalW, targetH + 40); // +40 for label
        using var g = Graphics.FromImage(combined);
        g.Clear(Color.FromArgb(30, 30, 30));

        // Labels
        using var font = new Font("Segoe UI", 14, FontStyle.Bold);
        g.DrawString("WPF", font, Brushes.Cyan, 10, 5);
        g.DrawString("WinUI", font, Brushes.Orange, wpfBmp.Width + 20, 5);

        // Images
        g.DrawImage(wpfBmp, 0, 35, wpfBmp.Width, wpfBmp.Height);
        g.DrawImage(winuiBmp, wpfBmp.Width + 20, 35, winuiBmp.Width, winuiBmp.Height);

        // Separator line
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
        // Try cached
        if (_cachedWinUI != null)
        {
            try { _cachedWinUI.Refresh(); if (!_cachedWinUI.HasExited) return _cachedWinUI; } catch { }
        }

        // Find by path
        _cachedWinUI = FindWinUIProcess();
        if (_cachedWinUI != null) return _cachedWinUI;

        // Launch
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
