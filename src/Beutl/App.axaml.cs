﻿using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Presenters;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using Avalonia.Threading;
using Beutl.Api.Services;
using Beutl.Configuration;
using Beutl.NodeTree.Nodes;
using Beutl.Operators;
using Beutl.Pages;
using Beutl.Services;
using Beutl.Services.StartupTasks;
using Beutl.ViewModels;
using Beutl.Views;
using FluentAvalonia.Core;
using FluentAvalonia.Styling;
using NuGet.Configuration;
using Reactive.Bindings;

namespace Beutl;

public sealed class App : Application
{
    private readonly TaskCompletionSource _windowOpenTcs = new();
    private FluentAvaloniaTheme? _theme;
    private MainViewModel? _mainViewModel;
    private Startup? _startUp;

    public override void Initialize()
    {
        _startUp = GetMainViewModel().RunStartupTask();
        _startUp.WaitAll().ContinueWith(_ => _startUp = null);

        using Activity? activity = Telemetry.StartActivity("App.Initialize");

        FAUISettings.SetAnimationsEnabledAtAppLevel(true);

        GlobalConfiguration config = GlobalConfiguration.Instance;
        ViewConfig view = config.ViewConfig;

        AvaloniaXamlLoader.Load(this);
        Resources["PaletteColors"] = AppHelpers.GetPaletteColors();

        _theme = (FluentAvaloniaTheme)Styles[0];

        activity?.AddEvent(new ActivityEvent("Xaml_Loaded"));

        view.GetObservable(ViewConfig.ThemeProperty).Subscribe(v =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                switch (v)
                {
                    case ViewConfig.ViewTheme.Light:
                        RequestedThemeVariant = ThemeVariant.Light;
                        break;
                    case ViewConfig.ViewTheme.Dark:
                        RequestedThemeVariant = ThemeVariant.Dark;
                        break;
                    case ViewConfig.ViewTheme.HighContrast:
                        RequestedThemeVariant = FluentAvaloniaTheme.HighContrastTheme;
                        break;
                    case ViewConfig.ViewTheme.System:
                        _theme.PreferSystemTheme = true;
                        break;
                }
            }, DispatcherPriority.Send);
        });

        if (view.UseCustomAccentColor && Color.TryParse(view.CustomAccentColor, out Color customColor))
        {
            activity?.SetTag("CustomAccentColor", customColor.ToString());

            _theme.CustomAccentColor = customColor;
        }

        if (OperatingSystem.IsMacOS())
        {
            _theme.RemoveRange(1, 3);
        }
    }

    public override void RegisterServices()
    {
        using Activity? activity = Telemetry.StartActivity("App.RegisterServices");

        base.RegisterServices();

        PropertyEditorExtension.DefaultHandler = new PropertyEditorService.PropertyEditorExtensionImpl();
        ProjectItemContainer.Current.Generator = new ProjectItemGenerator();
        NotificationService.Handler = new NotificationServiceHandler();

        // 以下三つの処理は意外と重い
        Parallel.Invoke(
            () => GetMainViewModel().RegisterServices(),
            LibraryRegistrar.RegisterAll,
            NodesRegistrar.RegisterAll);

        ReactivePropertyScheduler.SetDefault(AvaloniaScheduler.Instance);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (OperatingSystem.IsMacOS())
            {
                desktop.MainWindow = new MacWindow { DataContext = GetMainViewModel(), };
            }
            else
            {
                desktop.MainWindow = new MainWindow { DataContext = GetMainViewModel(), };
            }

            desktop.MainWindow.Opened += (_, _) => _windowOpenTcs.SetResult();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView { DataContext = GetMainViewModel(), };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static IClipboard? GetClipboard()
    {
        return GetTopLevel()?.Clipboard;
    }

    public static TopLevel? GetTopLevel()
    {
        return (Current?.ApplicationLifetime) switch
        {
            IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow,
            ISingleViewApplicationLifetime { MainView: { } mainview } => TopLevel.GetTopLevel(mainview),
            _ => null,
        };
    }

    public static ContextCommandManager? GetContextCommandManager()
    {
        return ((App)Current!).GetMainViewModel().ContextCommandManager;
    }

    public static FluentAvaloniaTheme? GetFATheme()
    {
        return (Current as App)?._theme;
    }

    public static async ValueTask WaitStartupTask()
    {
        if (Current is App { _startUp: { } obj })
        {
            await obj.WaitAll();
        }
    }

    public static async ValueTask WaitLoadingExtensions()
    {
        if (Current is App { _startUp: { } obj })
        {
            await obj.WaitLoadingExtensions();
        }
    }

    public static async ValueTask WaitWindowOpened()
    {
        if (Current is App app)
        {
            await app._windowOpenTcs.Task.ConfigureAwait(false);
        }
    }

    private MainViewModel GetMainViewModel()
    {
        return _mainViewModel ??= new MainViewModel();
    }

    private async void AboutBeutlClicked(object? sender, EventArgs e)
    {
        if (_mainViewModel != null
            && ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
        {
            using var dialogViewModel = _mainViewModel.CreateSettingsDialog();
            var dialog = new SettingsDialog { DataContext = dialogViewModel };
            dialogViewModel.GoToSettingsPage();
            await dialog.ShowDialog(window);
        }
    }

    private async void OpenSettingsClicked(object? sender, EventArgs e)
    {
        if (_mainViewModel != null
            && ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
        {
            using var dialogViewModel = _mainViewModel.CreateSettingsDialog();
            var dialog = new SettingsDialog { DataContext = dialogViewModel };
            dialogViewModel.GoToAccountSettingsPage();
            await dialog.ShowDialog(window);
        }
    }
}
