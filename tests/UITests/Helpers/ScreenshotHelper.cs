using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace UITests.Helpers;

/// <summary>
/// Screenshot capture and comparison utilities.
/// Uses PrintWindow Win32 API for reliable window capture regardless of z-order.
/// </summary>
public static class ScreenshotHelper
{
    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);

    [StructLayout(LayoutKind.Sequential)]
    struct RECT { public int Left, Top, Right, Bottom; }

    const uint PW_RENDERFULLCONTENT = 0x2;

    /// <summary>
    /// Capture a screenshot of a process's main window using PrintWindow.
    /// Works even when the window is partially obscured by other windows.
    /// </summary>
    public static Bitmap CaptureWindow(Process proc)
    {
        proc.Refresh();
        var hwnd = proc.MainWindowHandle;
        if (hwnd == IntPtr.Zero)
            throw new InvalidOperationException("No main window handle");

        NativeMethods.SetForegroundWindow(hwnd);
        Thread.Sleep(300);

        if (!GetWindowRect(hwnd, out var rect))
            throw new InvalidOperationException("GetWindowRect failed");

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;
        if (width <= 0 || height <= 0)
            throw new InvalidOperationException($"Window has invalid size: {width}x{height}");

        var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        var hdc = g.GetHdc();
        try
        {
            if (!PrintWindow(hwnd, hdc, PW_RENDERFULLCONTENT))
                PrintWindow(hwnd, hdc, 0); // fallback without full content flag
        }
        finally
        {
            g.ReleaseHdc(hdc);
        }
        return bmp;
    }

    /// <summary>
    /// Save a screenshot to disk.
    /// </summary>
    public static string SaveScreenshot(Bitmap bmp, string directory, string name)
    {
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, $"{name}.png");
        bmp.Save(path, ImageFormat.Png);
        return path;
    }

    /// <summary>
    /// Capture and save a screenshot of a process's main window.
    /// </summary>
    public static string CaptureAndSave(Process proc, string directory, string name)
    {
        using var bmp = CaptureWindow(proc);
        return SaveScreenshot(bmp, directory, name);
    }

    /// <summary>
    /// Get the average color of a region (useful for verifying backgrounds).
    /// </summary>
    public static Color GetAverageColor(Bitmap bmp, Rectangle region)
    {
        long r = 0, g = 0, b = 0;
        int count = 0;

        var clampedRegion = Rectangle.Intersect(region, new Rectangle(0, 0, bmp.Width, bmp.Height));
        if (clampedRegion.Width <= 0 || clampedRegion.Height <= 0)
            return Color.Empty;

        for (int x = clampedRegion.Left; x < clampedRegion.Right; x += 2)
        {
            for (int y = clampedRegion.Top; y < clampedRegion.Bottom; y += 2)
            {
                var pixel = bmp.GetPixel(x, y);
                r += pixel.R;
                g += pixel.G;
                b += pixel.B;
                count++;
            }
        }

        if (count == 0) return Color.Empty;
        return Color.FromArgb((int)(r / count), (int)(g / count), (int)(b / count));
    }

    /// <summary>
    /// Check if a color is "dark" (brightness &lt; 128).
    /// </summary>
    public static bool IsDark(Color color)
    {
        return (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) < 128;
    }

    /// <summary>
    /// Check if a color is "light" (brightness >= 128).
    /// </summary>
    public static bool IsLight(Color color)
    {
        return !IsDark(color);
    }

    /// <summary>
    /// Compare two screenshots and return the percentage of pixels that differ
    /// beyond a tolerance threshold.
    /// </summary>
    public static double CompareScreenshots(Bitmap a, Bitmap b, int tolerance = 30)
    {
        var width = Math.Min(a.Width, b.Width);
        var height = Math.Min(a.Height, b.Height);
        if (width <= 0 || height <= 0) return 1.0;

        int diffPixels = 0;
        int totalPixels = 0;

        for (int x = 0; x < width; x += 3)
        {
            for (int y = 0; y < height; y += 3)
            {
                var pa = a.GetPixel(x, y);
                var pb = b.GetPixel(x, y);
                var diff = Math.Abs(pa.R - pb.R) + Math.Abs(pa.G - pb.G) + Math.Abs(pa.B - pb.B);
                if (diff > tolerance)
                    diffPixels++;
                totalPixels++;
            }
        }

        return (double)diffPixels / totalPixels;
    }

    /// <summary>
    /// Sample the content area background color.
    /// With PrintWindow, coordinates are in window-local space.
    /// The flyout is ~320 DIP wide. Content starts to the right of it.
    /// Sample in the center of the content area to avoid headers/footers.
    /// </summary>
    public static Color SampleContentBackground(Bitmap bmp, int flyoutWidth = 480)
    {
        // Content area starts after the flyout. Sample its center.
        var contentX = flyoutWidth + (bmp.Width - flyoutWidth) / 2;
        var contentY = bmp.Height / 2;
        contentX = Math.Max(flyoutWidth + 50, Math.Min(contentX, bmp.Width - 100));
        contentY = Math.Max(100, Math.Min(contentY, bmp.Height - 100));
        var region = new Rectangle(contentX, contentY, Math.Min(100, bmp.Width - contentX), Math.Min(100, bmp.Height - contentY));
        return GetAverageColor(bmp, region);
    }
}
