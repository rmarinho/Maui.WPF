using System.Diagnostics;
using UITests.Helpers;

namespace UITests;

/// <summary>
/// Shared fixture that launches the WPF app once for all tests in the collection.
/// Build must be done before running tests (dotnet build the ControlGallery project).
/// Always use GetProcess() to get a fresh, valid Process reference.
/// </summary>
public class WpfAppFixture : IDisposable
{
    public AppLauncher Launcher { get; }

    public WpfAppFixture()
    {
        Launcher = new AppLauncher();
        Launcher.LaunchWpf(build: true);
        // Give the app a moment to fully render
        Thread.Sleep(4000);
    }

    /// <summary>
    /// Always returns a fresh Process object for the WPF ControlGallery
    /// with a valid MainWindowHandle. Distinguishes from WinUI by module path.
    /// </summary>
    public Process GetProcess()
    {
        for (int i = 0; i < 10; i++)
        {
            foreach (var p in Process.GetProcessesByName("ControlGallery"))
            {
                try
                {
                    p.Refresh();
                    if (p.MainWindowHandle == IntPtr.Zero) continue;
                    // Distinguish WPF from WinUI by checking module path
                    var path = p.MainModule?.FileName ?? "";
                    if (path.Contains("net10.0-windows10.0", StringComparison.OrdinalIgnoreCase))
                        continue; // Skip WinUI process
                    return p;
                }
                catch { }
            }
            Thread.Sleep(500);
        }
        throw new InvalidOperationException("Could not find WPF ControlGallery process with a valid window");
    }

    public void Dispose()
    {
        try
        {
            var proc = GetProcess();
            proc.Kill(entireProcessTree: true);
        }
        catch { }
        Launcher.Dispose();
    }
}

[CollectionDefinition("WPF App")]
public class WpfAppCollection : ICollectionFixture<WpfAppFixture> { }
