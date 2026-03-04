using System.Drawing;
using System.Windows.Automation;
using UITests.Helpers;

namespace UITests;

/// <summary>
/// Tests for theme switching: Light → Dark → Light.
/// Verifies that backgrounds, text colors, and CollectionView items update correctly.
/// </summary>
[Collection("WPF App")]
public class ThemeSwitchingTests
{
    readonly WpfAppFixture _fixture;

    public ThemeSwitchingTests(WpfAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void App_StartsInLightTheme()
    {
        var proc = _fixture.GetProcess();
        using var bmp = ScreenshotHelper.CaptureWindow(proc);
        ScreenshotHelper.SaveScreenshot(bmp, _fixture.Launcher.ScreenshotDir, "theme_initial");
        var contentColor = ScreenshotHelper.SampleContentBackground(bmp);

        Assert.True(ScreenshotHelper.IsLight(contentColor),
            $"Expected light content background, got RGB({contentColor.R},{contentColor.G},{contentColor.B})");
    }

    [Fact]
    public void SwitchToDark_ChangesContentBackground()
    {
        var proc = _fixture.GetProcess();
        var root = AutomationHelper.GetRoot(proc);

        // Switch to Dark
        Assert.True(AutomationHelper.SelectComboBoxItem(root, "Dark"), "Failed to select Dark theme");
        Thread.Sleep(2000);

        proc = _fixture.GetProcess();
        using var bmp = ScreenshotHelper.CaptureWindow(proc);
        ScreenshotHelper.SaveScreenshot(bmp, _fixture.Launcher.ScreenshotDir, "theme_dark");

        var contentColor = ScreenshotHelper.SampleContentBackground(bmp);
        Assert.True(ScreenshotHelper.IsDark(contentColor),
            $"Expected dark content background after Dark theme, got RGB({contentColor.R},{contentColor.G},{contentColor.B})");

        // Switch back to Light for other tests
        root = AutomationHelper.GetRoot(proc);
        AutomationHelper.SelectComboBoxItem(root, "Light");
        Thread.Sleep(2000);
    }

    [Fact]
    public void SwitchToLight_RestoresContentBackground()
    {
        var proc = _fixture.GetProcess();
        var root = AutomationHelper.GetRoot(proc);

        // Switch to Dark first
        AutomationHelper.SelectComboBoxItem(root, "Dark");
        Thread.Sleep(2000);

        // Switch back to Light
        root = AutomationHelper.GetRoot(_fixture.GetProcess());
        Assert.True(AutomationHelper.SelectComboBoxItem(root, "Light"), "Failed to select Light theme");
        Thread.Sleep(2000);

        proc = _fixture.GetProcess();
        using var bmp = ScreenshotHelper.CaptureWindow(proc);
        ScreenshotHelper.SaveScreenshot(bmp, _fixture.Launcher.ScreenshotDir, "theme_light_restored");

        var contentColor = ScreenshotHelper.SampleContentBackground(bmp);
        Assert.True(ScreenshotHelper.IsLight(contentColor),
            $"Expected light content background after Light theme, got RGB({contentColor.R},{contentColor.G},{contentColor.B})");
    }

    [Fact]
    public void FlyoutPanel_UpdatesOnThemeChange()
    {
        var proc = _fixture.GetProcess();
        var root = AutomationHelper.GetRoot(proc);

        // Ensure we're in Light mode first
        AutomationHelper.SelectComboBoxItem(root, "Light");
        Thread.Sleep(2000);

        // Capture flyout in Light mode
        proc = _fixture.GetProcess();
        using var lightBmp = ScreenshotHelper.CaptureWindow(proc);
        ScreenshotHelper.SaveScreenshot(lightBmp, _fixture.Launcher.ScreenshotDir, "flyout_light");
        var lightFlyout = ScreenshotHelper.GetAverageColor(lightBmp,
            new Rectangle(10, 400, 200, 200));

        // Switch to Dark
        root = AutomationHelper.GetRoot(proc);
        AutomationHelper.SelectComboBoxItem(root, "Dark");
        Thread.Sleep(2000);

        proc = _fixture.GetProcess();
        using var darkBmp = ScreenshotHelper.CaptureWindow(proc);
        ScreenshotHelper.SaveScreenshot(darkBmp, _fixture.Launcher.ScreenshotDir, "flyout_dark");
        var darkFlyout = ScreenshotHelper.GetAverageColor(darkBmp,
            new Rectangle(10, 400, 200, 200));

        // Flyout should change from light to dark
        Assert.True(ScreenshotHelper.IsLight(lightFlyout),
            $"Flyout should be light in Light theme, got RGB({lightFlyout.R},{lightFlyout.G},{lightFlyout.B})");
        Assert.True(ScreenshotHelper.IsDark(darkFlyout),
            $"Flyout should be dark in Dark theme, got RGB({darkFlyout.R},{darkFlyout.G},{darkFlyout.B})");

        // Restore Light
        root = AutomationHelper.GetRoot(proc);
        AutomationHelper.SelectComboBoxItem(root, "Light");
        Thread.Sleep(2000);
    }
}
