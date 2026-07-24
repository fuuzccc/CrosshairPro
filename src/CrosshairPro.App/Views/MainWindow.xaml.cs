using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CrosshairPro.App.Services;
using CrosshairPro.App.ViewModels;

namespace CrosshairPro.App.Views;

public partial class MainWindow : Window
{
    private readonly TrayIconService _trayIconService = new();
    private bool _isExiting;

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

                _ = LoadDeveloperAvatarAsync();
            }
        }
        catch
        {
        }
    }

    private async Task LoadDeveloperAvatarAsync()
    {
        try
        {
            var avatarUrl = "https://github.com/fuuzccc.png";
            var httpClient = new System.Net.Http.HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(avatarUrl);
            var stream = new System.IO.MemoryStream(imageBytes);
            var bitmap = new Bitmap(stream);
            DeveloperAvatar.Source = bitmap;
        }
        catch
        {
            try
            {
                var fallbackStream = AssetLoader.Open(new Uri("avares://CrosshairPro.App/Assets/app_icon.png"));
                DeveloperAvatar.Source = new Bitmap(fallbackStream);
            }
            catch
            {
            }
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        try
        {
            if (_isExiting)
            {
                _trayIconService.Remove();
                base.OnClosing(e);
                return;
            }

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
            _isExiting = true;
            _trayIconService.Remove();
        }
        catch
        {
        }

        try
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
            else
            {
                Close();
            }
        }
        catch
        {
            Close();
        }
    }
}
