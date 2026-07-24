using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CrosshairPro.Core.Models;
using CrosshairPro.App.Controls;
using CrosshairPro.App.Helpers;

namespace CrosshairPro.App.Windows;

public class CrosshairOverlayWindow : Window
{
    private readonly CrosshairOverlay _crosshairOverlay;

    public static readonly StyledProperty<CrosshairSettings?> CrosshairSettingsProperty =
        AvaloniaProperty.Register<CrosshairOverlayWindow, CrosshairSettings?>(nameof(CrosshairSettings));

    public CrosshairSettings? CrosshairSettings
    {
        get => GetValue(CrosshairSettingsProperty);
        set => SetValue(CrosshairSettingsProperty, value);
    }

    public CrosshairOverlayWindow()
    {
        Title = "CrosshairPro Overlay";
        CanResize = false;
        SystemDecorations = SystemDecorations.None;
        Topmost = true;
        ShowInTaskbar = false;
        WindowState = WindowState.Normal;
        Focusable = false;
        IsHitTestVisible = false;

        Background = Brushes.Transparent;
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };

        _crosshairOverlay = new CrosshairOverlay
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
            IsHitTestVisible = false
        };

        Content = _crosshairOverlay;

        _crosshairOverlay.Bind(CrosshairOverlay.SettingsProperty,
            this.GetObservable(CrosshairSettingsProperty));

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        try
        {
            WindowHelper.MakeClickThrough(this);
            PositionOverlayFullscreen();
            _crosshairOverlay.InvalidateVisual();
        }
        catch
        {
        }
    }

    private void PositionOverlayFullscreen()
    {
        if (!OperatingSystem.IsWindows())
            return;

        var handle = this.TryGetPlatformHandle();
        if (handle == null || handle.Handle == IntPtr.Zero)
            return;

        int screenWidth = Win32Api.GetSystemMetrics(Win32Api.SM_CXSCREEN);
        int screenHeight = Win32Api.GetSystemMetrics(Win32Api.SM_CYSCREEN);

        int exStyle = Win32Api.GetWindowLong(handle.Handle, Win32Api.GWL_EXSTYLE);
        Win32Api.SetWindowLong(handle.Handle, Win32Api.GWL_EXSTYLE,
            exStyle | Win32Api.WS_EX_TRANSPARENT | Win32Api.WS_EX_LAYERED | Win32Api.WS_EX_TOOLWINDOW);

        int style = Win32Api.GetWindowLong(handle.Handle, Win32Api.GWL_STYLE);
        Win32Api.SetWindowLong(handle.Handle, Win32Api.GWL_STYLE,
            unchecked((int)(Win32Api.WS_POPUP | Win32Api.WS_VISIBLE)));

        Win32Api.SetWindowPos(
            handle.Handle,
            new IntPtr(-1),
            0, 0,
            screenWidth, screenHeight,
            Win32Api.SWP_NOACTIVATE);
    }

    public void SetCrosshairSettings(CrosshairSettings settings)
    {
        CrosshairSettings = settings;
        _crosshairOverlay.InvalidateVisual();
    }

    public void UpdatePosition(PixelPoint position, Size size)
    {
        Position = position;
        Width = size.Width;
        Height = size.Height;
        _crosshairOverlay.InvalidateVisual();
    }
}
