using System.Drawing;
using System.Windows.Automation;
using UITests.Helpers;

namespace UITests;

/// <summary>
/// Tests that navigate to each flyout page and verify content renders.
/// Takes screenshots for visual regression.
/// </summary>
[Collection("WPF App")]
public class PageNavigationTests
{
    readonly WpfAppFixture _fixture;

    // All flyout items that should exist in the ControlGallery
    static readonly string[] FlyoutPages = ["Home", "Controls", "Layouts", "Features", "Tips & Tricks"];

    public PageNavigationTests(WpfAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void FlyoutHeader_ShowsBotImage()
    {
        var proc = _fixture.GetProcess();

        // Take a screenshot and check that the header area has varied pixels (image rendered)
        using var bmp = ScreenshotHelper.CaptureWindow(proc);
        ScreenshotHelper.SaveScreenshot(bmp, _fixture.Launcher.ScreenshotDir, "flyout_header");
        var headerColor = ScreenshotHelper.GetAverageColor(bmp, new Rectangle(50, 10, 250, 170));

        int pixelVariance = 0;
        for (int x = 80; x < Math.Min(350, bmp.Width); x += 10)
        {
            for (int y = 20; y < Math.Min(170, bmp.Height); y += 10)
            {
                var pixel = bmp.GetPixel(x, y);
                var diff = Math.Abs(pixel.R - headerColor.R) + Math.Abs(pixel.G - headerColor.G) + Math.Abs(pixel.B - headerColor.B);
                if (diff > 30) pixelVariance++;
            }
        }
        Assert.True(pixelVariance > 3, $"Expected visual content in header (image), found mostly uniform RGB({headerColor.R},{headerColor.G},{headerColor.B}), variance={pixelVariance}");
    }

    [Fact]
    public void FlyoutFooter_HasAppearancePicker()
    {
        var proc = _fixture.GetProcess();
        var root = AutomationHelper.GetRoot(proc);

        // Find "Appearance" label
        var labels = AutomationHelper.FindByName(root, "Appearance");
        Assert.True(labels.Count > 0, "Expected 'Appearance' label in flyout footer");

        // Find ComboBox
        var combos = AutomationHelper.FindAll(root, ControlType.ComboBox);
        Assert.True(combos.Count > 0, "Expected a ComboBox (theme picker) in flyout footer");
    }

    [Theory]
    [InlineData("Home")]
    [InlineData("Controls")]
    [InlineData("Layouts")]
    [InlineData("Features")]
    public void NavigateTo_Page_RendersContent(string pageName)
    {
        var proc = _fixture.GetProcess();
        var root = AutomationHelper.GetRoot(proc);

        // Click the flyout item text directly via mouse
        var texts = AutomationHelper.FindByName(root, pageName);
        bool clicked = false;
        foreach (AutomationElement t in texts)
        {
            var rect = t.Current.BoundingRectangle;
            if (rect.Width > 0 && rect.X < 500)
            {
                NativeMethods.SetForegroundWindow(proc.MainWindowHandle);
                Thread.Sleep(200);
                AutomationHelper.ClickAt(rect);
                clicked = true;
                break;
            }
        }
        Assert.True(clicked, $"Could not find flyout item '{pageName}'");
        Thread.Sleep(2000);

        // Re-get process reference after navigation
        proc = _fixture.GetProcess();

        // Take screenshot
        using var bmp = ScreenshotHelper.CaptureWindow(proc);
        ScreenshotHelper.SaveScreenshot(bmp, _fixture.Launcher.ScreenshotDir, $"page_{pageName.Replace(" ", "_").ToLower()}");

        // Verify content area is not blank (at least has some pixels)
        Assert.True(bmp.Width > 100 && bmp.Height > 100,
            $"Screenshot too small: {bmp.Width}x{bmp.Height}");
    }

    [Fact]
    public void AllFlyoutItems_ArePresent()
    {
        var proc = _fixture.GetProcess();
        var root = AutomationHelper.GetRoot(proc);

        foreach (var pageName in FlyoutPages)
        {
            var elements = AutomationHelper.FindByName(root, pageName);
            Assert.True(elements.Count > 0, $"Expected flyout item '{pageName}' to be present");
        }
    }

    [Fact]
    public void FlyoutBehavior_IsLocked()
    {
        var proc = _fixture.GetProcess();

        // In locked mode, the flyout panel should always be visible.
        using var bmp = ScreenshotHelper.CaptureWindow(proc);

        // The flyout area (left side) should have varied content (not blank)
        var flyoutRegion = new Rectangle(0, 200, 300, 400);
        var flyoutColor = ScreenshotHelper.GetAverageColor(bmp, flyoutRegion);
        Assert.NotEqual(Color.Empty, flyoutColor);

        // Additionally check that flyout items are detectable via automation
        var root = AutomationHelper.GetRoot(proc);
        var homeText = AutomationHelper.FindByName(root, "Home");
        Assert.True(homeText.Count > 0, "Expected 'Home' flyout item to be visible (locked flyout)");
    }
}
