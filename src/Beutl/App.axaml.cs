﻿using System.Reflection;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using Avalonia.Threading;

using Beutl.Configuration;
using Beutl.NodeTree.Nodes;
using Beutl.Operators;
using Beutl.Services;
using Beutl.ViewModels;
using Beutl.Views;

using FluentAvalonia.Core;
using FluentAvalonia.Styling;

using Reactive.Bindings;

namespace Beutl;

public sealed class App : Application
{
    private FluentAvaloniaTheme? _theme;
    private MainViewModel? _mainViewModel;

    public override void Initialize()
    {
        FAUISettings.SetAnimationsEnabledAtAppLevel(true);

        //PaletteColors
        Type colorsType = typeof(Colors);
        PropertyInfo[] colorProperties = colorsType.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static);
        Color[] colors = colorProperties.Select(p => p.GetValue(null)).OfType<Color>().ToArray();

        GlobalConfiguration config = GlobalConfiguration.Instance;
        ViewConfig view = config.ViewConfig;
        CultureInfo.CurrentUICulture = view.UICulture;

        AvaloniaXamlLoader.Load(this);
        Resources["PaletteColors"] = colors;

        _theme = (FluentAvaloniaTheme)Styles[0];

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
            _theme.CustomAccentColor = customColor;
        }
    }

    public override void RegisterServices()
    {
        base.RegisterServices();
        PropertyEditorExtension.DefaultHandler = new PropertyEditorService.PropertyEditorExtensionImpl();

        ProjectItemContainer.Current.Generator = new ProjectItemGenerator();
        NotificationService.Handler = new NotificationServiceHandler();

        GetMainViewModel().RegisterServices();

        LibraryRegistrar.RegisterAll();
        NodesRegistrar.RegisterAll();
        ReactivePropertyScheduler.SetDefault(AvaloniaScheduler.Instance);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = GetMainViewModel(),
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = GetMainViewModel(),
            };
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

    public static FluentAvaloniaTheme? GetFATheme()
    {
        return (Current as App)?._theme;
    }

    private MainViewModel GetMainViewModel()
    {
        return _mainViewModel ??= new MainViewModel();
    }
}
