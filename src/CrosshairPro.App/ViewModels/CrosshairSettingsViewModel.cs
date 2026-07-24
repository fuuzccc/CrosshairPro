using CommunityToolkit.Mvvm.ComponentModel;
using CrosshairPro.Core.Models;

namespace CrosshairPro.App.ViewModels;

public partial class CrosshairSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private CrosshairStyle _style;

    [ObservableProperty]
    private float _size;

    [ObservableProperty]
    private float _thickness;

    [ObservableProperty]
    private float _gap;

    [ObservableProperty]
    private bool _hasCenterDot;

    [ObservableProperty]
    private float _centerDotSize;

    [ObservableProperty]
    private bool _isTShaped;

    [ObservableProperty]
    private CrosshairColorPreset _colorPreset;

    [ObservableProperty]
    private byte _colorR;

    [ObservableProperty]
    private byte _colorG;

    [ObservableProperty]
    private byte _colorB;

    [ObservableProperty]
    private byte _alpha;

    [ObservableProperty]
    private bool _hasOutline;

    [ObservableProperty]
    private float _outlineThickness;

    [ObservableProperty]
    private bool _followRecoil;

    [ObservableProperty]
    private float _sniperWidth;

    [ObservableProperty]
    private float _offsetX;

    [ObservableProperty]
    private float _offsetY;

    public CrosshairSettingsViewModel()
    {
    }

    public CrosshairSettingsViewModel(CrosshairSettings settings)
    {
        Style = settings.Style;
        Size = settings.Size;
        Thickness = settings.Thickness;
        Gap = settings.Gap;
        HasCenterDot = settings.HasCenterDot;
        CenterDotSize = settings.CenterDotSize;
        IsTShaped = settings.IsTShaped;
        ColorPreset = settings.ColorPreset;
        ColorR = settings.ColorR;
        ColorG = settings.ColorG;
        ColorB = settings.ColorB;
        Alpha = settings.Alpha;
        HasOutline = settings.HasOutline;
        OutlineThickness = settings.OutlineThickness;
        FollowRecoil = settings.FollowRecoil;
        SniperWidth = settings.SniperWidth;
        OffsetX = settings.OffsetX;
        OffsetY = settings.OffsetY;
    }

    public CrosshairSettings ToModel()
    {
        return new CrosshairSettings
        {
            Style = Style,
            Size = Size,
            Thickness = Thickness,
            Gap = Gap,
            HasCenterDot = HasCenterDot,
            CenterDotSize = CenterDotSize,
            IsTShaped = IsTShaped,
            ColorPreset = ColorPreset,
            ColorR = ColorR,
            ColorG = ColorG,
            ColorB = ColorB,
            Alpha = Alpha,
            HasOutline = HasOutline,
            OutlineThickness = OutlineThickness,
            FollowRecoil = FollowRecoil,
            SniperWidth = SniperWidth,
            OffsetX = OffsetX,
            OffsetY = OffsetY
        };
    }
}
