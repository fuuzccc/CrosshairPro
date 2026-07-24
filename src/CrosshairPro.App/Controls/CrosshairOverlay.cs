using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CrosshairPro.Core.Models;

namespace CrosshairPro.App.Controls;

public class CrosshairOverlay : Control
{
    public static readonly StyledProperty<CrosshairSettings?> SettingsProperty =
        AvaloniaProperty.Register<CrosshairOverlay, CrosshairSettings?>(nameof(Settings));

    public CrosshairSettings? Settings
    {
        get => GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    public static readonly StyledProperty<double> ScaleProperty =
        AvaloniaProperty.Register<CrosshairOverlay, double>(nameof(Scale), 1.0);

    public double Scale
    {
        get => GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    static CrosshairOverlay()
    {
        AffectsRender<CrosshairOverlay>(
            SettingsProperty,
            ScaleProperty,
            BoundsProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Settings == null)
            return;

        Helpers.CrosshairRenderer.Render(context, Settings, Bounds.Size, Scale);
    }
}
