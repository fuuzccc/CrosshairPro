using System.Text;
using System.Text.RegularExpressions;
using CrosshairPro.Core.Models;

namespace CrosshairPro.Core.Helpers;

public static class Cs2ConfigParser
{
    public static CrosshairSettings ParseCrosshairCode(string code)
    {
        var settings = new CrosshairSettings();

        if (string.IsNullOrWhiteSpace(code))
            return settings;

        var commands = code.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var cmd in commands)
        {
            var trimmed = cmd.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            var parts = trimmed.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                continue;

            var command = parts[0].ToLowerInvariant();
            var valueStr = parts[1];

            try
            {
                switch (command)
                {
                    case "cl_crosshairstyle":
                        if (int.TryParse(valueStr, out var style) && Enum.IsDefined(typeof(CrosshairStyle), style))
                            settings.Style = (CrosshairStyle)style;
                        break;

                    case "cl_crosshairsize":
                        if (float.TryParse(valueStr, out var size))
                            settings.Size = size;
                        break;

                    case "cl_crosshairthickness":
                        if (float.TryParse(valueStr, out var thickness))
                            settings.Thickness = thickness;
                        break;

                    case "cl_crosshairgap":
                        if (float.TryParse(valueStr, out var gap))
                            settings.Gap = gap;
                        break;

                    case "cl_crosshairdot":
                        settings.HasCenterDot = valueStr == "1";
                        break;

                    case "cl_crosshair_t":
                        settings.IsTShaped = valueStr == "1";
                        break;

                    case "cl_crosshaircolor":
                        if (int.TryParse(valueStr, out var color) && Enum.IsDefined(typeof(CrosshairColorPreset), color))
                            settings.ColorPreset = (CrosshairColorPreset)color;
                        break;

                    case "cl_crosshaircolor_r":
                        if (byte.TryParse(valueStr, out var r))
                            settings.ColorR = r;
                        break;

                    case "cl_crosshaircolor_g":
                        if (byte.TryParse(valueStr, out var g))
                            settings.ColorG = g;
                        break;

                    case "cl_crosshaircolor_b":
                        if (byte.TryParse(valueStr, out var b))
                            settings.ColorB = b;
                        break;

                    case "cl_crosshairusealpha":
                        break;

                    case "cl_crosshairalpha":
                        if (byte.TryParse(valueStr, out var alpha))
                            settings.Alpha = alpha;
                        break;

                    case "cl_crosshair_drawoutline":
                        settings.HasOutline = valueStr == "1";
                        break;

                    case "cl_crosshair_outlinethickness":
                        if (float.TryParse(valueStr, out var outlineThickness))
                            settings.OutlineThickness = outlineThickness;
                        break;

                    case "cl_crosshair_recoil":
                        settings.FollowRecoil = valueStr == "1";
                        break;

                    case "cl_crosshair_sniper_width":
                        if (float.TryParse(valueStr, out var sniperWidth))
                            settings.SniperWidth = sniperWidth;
                        break;
                }
            }
            catch
            {
                // 忽略解析错误的命令
            }
        }

        return settings;
    }

    public static string GenerateCrosshairCode(CrosshairSettings settings)
    {
        var sb = new StringBuilder();

        sb.Append($"cl_crosshairstyle {(int)settings.Style};");
        sb.Append($"cl_crosshairsize {settings.Size};");
        sb.Append($"cl_crosshairthickness {settings.Thickness};");
        sb.Append($"cl_crosshairgap {settings.Gap};");
        sb.Append($"cl_crosshairdot {(settings.HasCenterDot ? 1 : 0)};");
        sb.Append($"cl_crosshair_t {(settings.IsTShaped ? 1 : 0)};");
        sb.Append($"cl_crosshaircolor {(int)settings.ColorPreset};");

        if (settings.ColorPreset == CrosshairColorPreset.Custom)
        {
            sb.Append($"cl_crosshaircolor_r {settings.ColorR};");
            sb.Append($"cl_crosshaircolor_g {settings.ColorG};");
            sb.Append($"cl_crosshaircolor_b {settings.ColorB};");
        }

        sb.Append($"cl_crosshairusealpha 1;");
        sb.Append($"cl_crosshairalpha {settings.Alpha};");
        sb.Append($"cl_crosshair_drawoutline {(settings.HasOutline ? 1 : 0)};");
        sb.Append($"cl_crosshair_outlinethickness {settings.OutlineThickness};");
        sb.Append($"cl_crosshair_recoil {(settings.FollowRecoil ? 1 : 0)};");
        sb.Append($"cl_crosshair_sniper_width {settings.SniperWidth};");

        return sb.ToString();
    }
}
