using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using CrosshairPro.Core.Services;
using CrosshairPro.App.Services;
using CrosshairPro.App.ViewModels;
using System;

namespace CrosshairPro.App;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    private bool _arknightsThemeLoaded;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IPresetService, PresetService>();
        services.AddSingleton<ICrosshairService, CrosshairService>();

        if (OperatingSystem.IsWindows())
        {
            services.AddSingleton<IMouseHookService, WindowsHookService>();
        }
        else
        {
            services.AddSingleton<IMouseHookService, MouseHookService>();
        }

        services.AddTransient<MainViewModel>();

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainVm = Services.GetRequiredService<MainViewModel>();

            desktop.MainWindow = new Views.MainWindow
            {
                DataContext = mainVm
            };

            desktop.Exit += (s, e) =>
            {
                try
                {
                    var hookService = Services.GetService<IMouseHookService>();
                    hookService?.UninstallHook();
                }
                catch
                {
                }
            };
        }

        base.OnFrameworkInitializationCompleted();

        var settingsService = Services.GetService<ISettingsService>();
        if (settingsService != null && settingsService.Settings.Theme == "Arknights")
        {
            ApplyTheme("Arknights");
        }
    }

    public void ApplyTheme(string theme)
    {
        if (theme == "Arknights")
        {
            if (!_arknightsThemeLoaded)
            {
                var style = new StyleInclude(new Uri("avares://CrosshairPro.App/Themes/ArknightsTheme.axaml"))
                {
                    Source = new Uri("avares://CrosshairPro.App/Themes/ArknightsTheme.axaml")
                };
                Styles.Add(style);
                _arknightsThemeLoaded = true;
            }

            RequestedThemeVariant = ThemeVariant.Dark;
        }
        else
        {
            if (_arknightsThemeLoaded)
            {
                for (int i = Styles.Count - 1; i >= 0; i--)
                {
                    if (Styles[i] is StyleInclude styleInclude &&
                        styleInclude.Source != null &&
                        styleInclude.Source.ToString().Contains("ArknightsTheme"))
                    {
                        Styles.RemoveAt(i);
                    }
                }
                _arknightsThemeLoaded = false;
            }

            RequestedThemeVariant = theme == "Light" ? ThemeVariant.Light : ThemeVariant.Dark;
        }
    }
}
