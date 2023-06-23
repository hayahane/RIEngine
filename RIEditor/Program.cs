using Avalonia;
using Avalonia.ReactiveUI;
using System;
using RIEngine.Utility.AssetImporter;

namespace RIEditor;

class Program
{
    /*// Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();*/

    public static void Main(string[] args)
    {
        ObjFileImporter.ImportObjFile(@"E:\Monologist\CsProjects\RIEngine\Resources\RIEngine\cube.obj",
            @"E:\Monologist\CsProjects\RIEngine\RIEngine\Assets\cube.modelAsset");
    }
}
