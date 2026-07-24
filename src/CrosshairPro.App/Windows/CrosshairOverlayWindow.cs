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

        var panel = new Panel
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
            IsHitTestVisible = false,
            Margin = new Thickness(0)
        };

        panel.Children.Add(_crosshairOverlay);
        Content = panel;

        _crosshairOverlay.Bind(CrosshairOverlay.SettingsProperty,
            this.GetObservable(CrosshairSettingsProperty));

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        try
        {
            WindowHelper.MakeClickThrough(this);
            _crosshairOverlay.InvalidateVisual();
        }
        catch
        {
        }
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
