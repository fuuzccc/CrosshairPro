using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using CrosshairPro.Core.Models;
using CrosshairPro.Core.Services;
using CrosshairPro.App.Windows;

namespace CrosshairPro.App.Services;

public class CrosshairService : ICrosshairService
{
    private CrosshairOverlayWindow? _overlayWindow;
    private readonly ISettingsService _settingsService;
    private bool _isVisible;

    public bool IsVisible => _isVisible;

    public event EventHandler<bool>? VisibilityChanged;

    public CrosshairService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _settingsService.SettingsChanged += OnSettingsChanged;
    }

    public void Show()
    {
        if (_isVisible)
            return;

        EnsureOverlayWindow();
        _overlayWindow?.Show();
        _isVisible = true;
        VisibilityChanged?.Invoke(this, true);
    }

    public void Hide()
    {
        if (!_isVisible)
            return;

        _overlayWindow?.Hide();
        _isVisible = false;
        VisibilityChanged?.Invoke(this, false);
    }

    public void Toggle()
    {
        if (_isVisible)
            Hide();
        else
            Show();
    }

    public void UpdateSettings(CrosshairSettings settings)
    {
        EnsureOverlayWindow();
        _overlayWindow?.SetCrosshairSettings(settings);
    }

    private void EnsureOverlayWindow()
    {
        if (_overlayWindow != null)
            return;

        _overlayWindow = new CrosshairOverlayWindow();

        _overlayWindow.SetCrosshairSettings(_settingsService.Settings.Crosshair);

        _overlayWindow.Opened += (s, e) =>
        {
            var primaryScreen = GetPrimaryScreen();
            if (primaryScreen != null)
            {
                var bounds = primaryScreen.Bounds;
                _overlayWindow.UpdatePosition(
                    new PixelPoint(bounds.X, bounds.Y),
                    new Size(bounds.Width, bounds.Height));
            }
        };
    }

    private Screen? GetPrimaryScreen()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = desktop.MainWindow;
            if (mainWindow != null)
            {
                return mainWindow.Screens?.Primary;
            }
        }

        return null;
    }

    private void OnSettingsChanged(object? sender, EventArgs e)
    {
        UpdateSettings(_settingsService.Settings.Crosshair);
    }
}
