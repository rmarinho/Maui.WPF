using System;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform.WPF;

namespace Sample;

public class MauiWPFApp : MauiWPFApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    [STAThread]
    static void Main(string[] args)
    {
        var app = new MauiWPFApp();
        app.Run();
    }
}
