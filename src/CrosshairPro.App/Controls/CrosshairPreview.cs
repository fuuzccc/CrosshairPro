using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CrosshairPro.App.Helpers;
using CrosshairPro.App.ViewModels;
using CrosshairPro.Core.Models;

namespace CrosshairPro.App.Controls;

public class CrosshairPreview : Control
{
    public static readonly StyledProperty<CrosshairSettingsViewModel?> SettingsProperty =
        AvaloniaProperty.Register<CrosshairPreview, CrosshairSettingsViewModel?>(nameof(Settings));

    public CrosshairSettingsViewModel? Settings
    {
        get => GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    static CrosshairPreview()
    {
        AffectsRender<CrosshairPreview>(SettingsProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Settings == null)
            return;

        var settings = Settings.ToModel();
        var renderSize = Bounds.Size;
        var scale = 1.5;

        CrosshairRenderer.Render(context, settings, renderSize, scale);
    }
}
