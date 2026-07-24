using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using CrosshairPro.App.Services;
using CrosshairPro.App.ViewModels;

namespace CrosshairPro.App.Views;

public partial class MainWindow : Window
{
    private readonly TrayIconService _trayIconService = new();

    public MainWindow()
    {
        InitializeComponent();
        try
        {
            SetWindowIcon();
        }
        catch
        {
        }
    }

    private void SetWindowIcon()
    {
        try
        {
            var iconStream = AssetLoader.Open(new Uri("avares://CrosshairPro.App/Assets/app_icon.png"));
            Icon = new WindowIcon(iconStream);
        }
        catch
        {
            try
            {
                var iconStream = typeof(MainWindow).Assembly.GetManifestResourceStream("CrosshairPro.App.Assets.app_icon.png");
                if (iconStream != null)
                {
                    Icon = new WindowIcon(iconStream);
                }
            }
            catch
            {
            }
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        try
        {
            if (DataContext is MainViewModel vm)
            {
                _trayIconService.Initialize(
                    this,
                    () => vm.IsCrosshairEnabled,
                    () => vm.ToggleCrosshairCommand.Execute(null),
                    () => ShowMainWindow(),
                    () => ExitApp()
                );

                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(MainViewModel.IsCrosshairEnabled))
                    {
                        _trayIconService.UpdateToolTip($"CrosshairPro - {(vm.IsCrosshairEnabled ? "准星已开启" : "准星已关闭")}");
                    }
                };

                _trayIconService.UpdateToolTip($"CrosshairPro - {(vm.IsCrosshairEnabled ? "准星已开启" : "准星已关闭")}");

                if (vm.StartMinimized)
                {
                    Hide();
                }
            }
        }
        catch
        {
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        try
        {
            if (DataContext is MainViewModel vm && vm.MinimizeToTray)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                _trayIconService.Remove();
                base.OnClosing(e);
            }
        }
        catch
        {
            base.OnClosing(e);
        }
    }

    private void ShowMainWindow()
    {
        try
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
            Topmost = true;
            Topmost = false;
        }
        catch
        {
        }
    }

    private void ExitApp()
    {
        try
        {
            _trayIconService.Remove();
        }
        catch
        {
        }
        Close();
    }
}
