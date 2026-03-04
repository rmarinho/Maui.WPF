using System.IO;
using System.Drawing;
using UITests.Helpers;

namespace UITests;

/// <summary>
/// Side-by-side comparison tests between WPF and WinUI reference app.
/// These tests are skipped if the WinUI app is not available.
/// </summary>
[Collection("WPF App")]
public class WinUIComparisonTests
{
    readonly WpfAppFixture _fixture;

    public WinUIComparisonTests(WpfAppFixture fixture)
    {
        _fixture = fixture;
    }

    static bool WinUIAvailable =>
        File.Exists(@"D:\repos\davidortinau\ControlGallery\src\ControlGallery\bin\Debug\net10.0-windows10.0.19041.0\win-x64\ControlGallery.exe");

    [Fact]
    public void HomePage_VisuallyMatchesWinUI()
    {
        if (!WinUIAvailable)
            return;

        using var winuiLauncher = new AppLauncher();
        var winuiProc = winuiLauncher.LaunchWinUI();
        if (winuiProc == null)
            return;

        Thread.Sleep(3000);

        try
        {
            var wpfProc = _fixture.GetProcess();

            using var wpfBmp = ScreenshotHelper.CaptureWindow(wpfProc);
            using var winuiBmp = ScreenshotHelper.CaptureWindow(winuiProc);

            ScreenshotHelper.SaveScreenshot(wpfBmp, _fixture.Launcher.ScreenshotDir, "compare_wpf_home");
            ScreenshotHelper.SaveScreenshot(winuiBmp, _fixture.Launcher.ScreenshotDir, "compare_winui_home");

            var wpfBg = ScreenshotHelper.SampleContentBackground(wpfBmp);
            var winuiBg = ScreenshotHelper.SampleContentBackground(winuiBmp);

            Assert.Equal(ScreenshotHelper.IsLight(wpfBg), ScreenshotHelper.IsLight(winuiBg));
        }
        finally
        {
            try { winuiProc.Kill(entireProcessTree: true); } catch { }
        }
    }
}
