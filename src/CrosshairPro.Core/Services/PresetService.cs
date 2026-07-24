using Newtonsoft.Json;
using CrosshairPro.Core.Models;
using CrosshairPro.Core.Helpers;

namespace CrosshairPro.Core.Services;

public class PresetService : IPresetService
{
    private readonly string _customPresetsPath;
    private List<CrosshairPreset> _customPresets = new();

    public PresetService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var settingsDir = Path.Combine(appDataPath, "CrosshairPro");
        Directory.CreateDirectory(settingsDir);
        _customPresetsPath = Path.Combine(settingsDir, "presets.json");
        LoadCustomPresets();
    }

    public IReadOnlyList<CrosshairPreset> GetBuiltInPresets()
    {
        return new List<CrosshairPreset>
        {
            new()
            {
                Name = "默认绿色静态",
                Description = "经典绿色静态准星",
                Author = "System",
                Settings = new CrosshairSettings
                {
                    Style = CrosshairStyle.ClassicStatic,
                    Size = 2,
                    Thickness = 1,
                    Gap = -2,
                    HasCenterDot = false,
                    IsTShaped = false,
                    ColorPreset = CrosshairColorPreset.Green,
                    Alpha = 255,
                    HasOutline = true,
                    OutlineThickness = 1
                }
            },
            new()
            {
                Name = "职业选手 - ZywOo",
                Description = "Vitality.ZywOo 准星设置",
                Author = "ZywOo",
                Settings = Cs2ConfigParser.ParseCrosshairCode(
                    "cl_crosshairstyle 4;cl_crosshairsize 1.5;cl_crosshairthickness 0.5;cl_crosshairgap -2;cl_crosshairdot 0;cl_crosshair_t 0;cl_crosshaircolor 5;cl_crosshaircolor_r 0;cl_crosshaircolor_g 255;cl_crosshaircolor_b 0;cl_crosshairalpha 255;cl_crosshair_drawoutline 1;cl_crosshair_outlinethickness 0.5;")
            },
            new()
            {
                Name = "职业选手 - s1mple",
                Description = "NAVI.s1mple 准星设置",
                Author = "s1mple",
                Settings = Cs2ConfigParser.ParseCrosshairCode(
                    "cl_crosshairstyle 4;cl_crosshairsize 2;cl_crosshairthickness 1;cl_crosshairgap -3;cl_crosshairdot 0;cl_crosshair_t 0;cl_crosshaircolor 5;cl_crosshaircolor_r 0;cl_crosshaircolor_g 255;cl_crosshaircolor_b 0;cl_crosshairalpha 255;cl_crosshair_drawoutline 1;cl_crosshair_outlinethickness 1;")
            },
            new()
            {
                Name = "T形准星",
                Description = "仅显示十字下半部分",
                Author = "System",
                Settings = new CrosshairSettings
                {
                    Style = CrosshairStyle.ClassicStatic,
                    Size = 3,
                    Thickness = 1.5f,
                    Gap = 0,
                    HasCenterDot = true,
                    IsTShaped = true,
                    ColorPreset = CrosshairColorPreset.Cyan,
                    Alpha = 255,
                    HasOutline = true,
                    OutlineThickness = 1
                }
            },
            new()
            {
                Name = "小点准星",
                Description = "仅中心点，适合精准射击",
                Author = "System",
                Settings = new CrosshairSettings
                {
                    Style = CrosshairStyle.ClassicStatic,
                    Size = 0,
                    Thickness = 1,
                    Gap = 0,
                    HasCenterDot = true,
                    CenterDotSize = 1.5f,
                    IsTShaped = false,
                    ColorPreset = CrosshairColorPreset.Yellow,
                    Alpha = 255,
                    HasOutline = true,
                    OutlineThickness = 1
                }
            },
            new()
            {
                Name = "粗十字准星",
                Description = "大尺寸粗线准星，高可见度",
                Author = "System",
                Settings = new CrosshairSettings
                {
                    Style = CrosshairStyle.ClassicStatic,
                    Size = 4,
                    Thickness = 2,
                    Gap = -1,
                    HasCenterDot = false,
                    IsTShaped = false,
                    ColorPreset = CrosshairColorPreset.Red,
                    Alpha = 255,
                    HasOutline = true,
                    OutlineThickness = 1.5f
                }
            }
        };
    }

    public IReadOnlyList<CrosshairPreset> GetCustomPresets()
    {
        return _customPresets.AsReadOnly();
    }

    public void SaveCustomPreset(CrosshairPreset preset)
    {
        var existing = _customPresets.FirstOrDefault(p => p.Name == preset.Name);
        if (existing != null)
        {
            _customPresets.Remove(existing);
        }
        _customPresets.Add(preset);
        SaveCustomPresets();
    }

    public void DeleteCustomPreset(string name)
    {
        var preset = _customPresets.FirstOrDefault(p => p.Name == name);
        if (preset != null)
        {
            _customPresets.Remove(preset);
            SaveCustomPresets();
        }
    }

    public CrosshairSettings? ImportFromCs2Code(string code)
    {
        try
        {
            return Cs2ConfigParser.ParseCrosshairCode(code);
        }
        catch
        {
            return null;
        }
    }

    public string ExportToCs2Code(CrosshairSettings settings)
    {
        return Cs2ConfigParser.GenerateCrosshairCode(settings);
    }

    private void LoadCustomPresets()
    {
        try
        {
            if (File.Exists(_customPresetsPath))
            {
                var json = File.ReadAllText(_customPresetsPath);
                var presets = JsonConvert.DeserializeObject<List<CrosshairPreset>>(json);
                if (presets != null)
                {
                    _customPresets = presets;
                }
            }
        }
        catch
        {
            _customPresets = new List<CrosshairPreset>();
        }
    }

    private void SaveCustomPresets()
    {
        try
        {
            var json = JsonConvert.SerializeObject(_customPresets, Formatting.Indented);
            File.WriteAllText(_customPresetsPath, json);
        }
        catch
        {
            // 忽略保存错误
        }
    }
}
