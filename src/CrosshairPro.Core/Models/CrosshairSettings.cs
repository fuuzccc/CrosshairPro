using Newtonsoft.Json;

namespace CrosshairPro.Core.Models;

public enum CrosshairStyle
{
    Default = 0,
    DefaultStatic = 1,
    Classic = 2,
    ClassicDynamic = 3,
    ClassicStatic = 4,
    Classic16 = 5
}

public enum CrosshairColorPreset
{
    Red = 0,
    Green = 1,
    Yellow = 2,
    Blue = 3,
    Cyan = 4,
    Custom = 5
}

public class CrosshairSettings
{
    public CrosshairStyle Style { get; set; } = CrosshairStyle.ClassicStatic;

    public float Size { get; set; } = 2f;

    public float Thickness { get; set; } = 1f;

    public float Gap { get; set; } = -2f;

    public bool HasCenterDot { get; set; } = false;

    public float CenterDotSize { get; set; } = 1f;

    public bool IsTShaped { get; set; } = false;

    public CrosshairColorPreset ColorPreset { get; set; } = CrosshairColorPreset.Green;

    public byte ColorR { get; set; } = 50;

    public byte ColorG { get; set; } = 250;

    public byte ColorB { get; set; } = 50;

    public byte Alpha { get; set; } = 255;

    public bool HasOutline { get; set; } = true;

    public float OutlineThickness { get; set; } = 1f;

    public bool FollowRecoil { get; set; } = false;

    public float SniperWidth { get; set; } = 1f;

    [JsonIgnore]
    public (byte R, byte G, byte B) ActualColor
    {
        get
        {
            return ColorPreset switch
            {
                CrosshairColorPreset.Red => (255, 0, 0),
                CrosshairColorPreset.Green => (50, 250, 50),
                CrosshairColorPreset.Yellow => (255, 255, 0),
                CrosshairColorPreset.Blue => (0, 0, 255),
                CrosshairColorPreset.Cyan => (0, 255, 255),
                CrosshairColorPreset.Custom => (ColorR, ColorG, ColorB),
                _ => (50, 250, 50)
            };
        }
    }
}
