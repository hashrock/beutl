﻿using Avalonia;
using Avalonia.ReactiveUI;

using FluentAvalonia.UI.Windowing;

namespace Beutl;

internal static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // STAThread属性がついている時に 'async Task Main' にするとDrag and Dropが動作しなくなる。
        Process[] processes = Process.GetProcessesByName("bpt");
        if (processes.Length > 0)
        {
            var startInfo = new ProcessStartInfo(Path.Combine(AppContext.BaseDirectory, "Beutl.WaitingDialog"))
            {
                ArgumentList =
                {
                    "--title", "Opening Beutl.",
                    "--subtitle", "Changes to the package are in progress.",
                    "--content", "To open Beutl, close Beutl.PackageTools.",
                    "--icon", "Info",
                    "--progress"
                }
            };

            var process = Process.Start(startInfo);

            foreach (Process item in processes)
            {
                if (!item.HasExited)
                {
                    item.WaitForExit();
                }
            }

            process?.Kill();
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
#if DEBUG
        GC.KeepAlive(typeof(Avalonia.Svg.Skia.SvgImageExtension).Assembly);
        GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);
        GC.KeepAlive(typeof(FluentIcons.FluentAvalonia.SymbolIcon).Assembly);
        GC.KeepAlive(typeof(FluentIcons.Common.Symbol).Assembly);
        GC.KeepAlive(typeof(AsyncImageLoader.ImageLoader).Assembly);
#endif

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI()
            .With(new Win32PlatformOptions()
            {
                UseWindowsUIComposition = true,
                //EnableMultitouch = true,
                CompositionBackdropCornerRadius = 8f
            })
            .UseFAWindowing()
            .LogToTrace();
    }
}
