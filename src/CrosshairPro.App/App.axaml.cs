using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using CrosshairPro.Core.Services;
using CrosshairPro.App.Services;
using CrosshairPro.App.ViewModels;

namespace CrosshairPro.App;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

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
        }

        base.OnFrameworkInitializationCompleted();
    }
}
