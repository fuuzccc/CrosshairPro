using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrosshairPro.Core.Services;
using CrosshairPro.Core.Models;
using System.Collections.ObjectModel;
using CrosshairPro.App.Helpers;

namespace CrosshairPro.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly ICrosshairService _crosshairService;
    private readonly IMouseHookService _mouseHookService;
    private readonly IPresetService _presetService;

    [ObservableProperty]
    private CrosshairSettingsViewModel _crosshairSettings;

    [ObservableProperty]
    private bool _isCrosshairEnabled;

    [ObservableProperty]
    private string _statusMessage = "准备就绪";

    [ObservableProperty]
    private int _selectedTabIndex;

    public IReadOnlyList<CrosshairPreset> BuiltInPresets { get; }
    public IReadOnlyList<CrosshairPreset> CustomPresets { get; }

    public ObservableCollection<string> MonitorOptions { get; } = new() { "主显示器" };
    public ObservableCollection<string> ThemeOptions { get; } = new() { "深色", "浅色", "明日方舟" };
    public ObservableCollection<string> LanguageOptions { get; } = new() { "简体中文", "English" };
    public ObservableCollection<string> TriggerModeOptions { get; } = new() { "长按", "短按", "双击" };

    public List<string> AllHotkeyDisplayOptions { get; }
    private readonly List<string> _hotkeyValues;

    public string DeveloperName => "fuuzccc";
    public string DeveloperGithub => "https://github.com/fuuzccc";
    public string AppVersion => "v1.5.0";

    public bool MinimizeToTray
    {
        get => _settingsService.Settings.MinimizeToTray;
        set
        {
            _settingsService.UpdateAppSettings(s => s.MinimizeToTray = value);
            OnPropertyChanged();
        }
    }

    public bool ShowInTaskbar
    {
        get => _settingsService.Settings.ShowInTaskbar;
        set
        {
            _settingsService.UpdateAppSettings(s => s.ShowInTaskbar = value);
            OnPropertyChanged();
        }
    }

    public bool AlwaysOnTop
    {
        get => _settingsService.Settings.AlwaysOnTop;
        set
        {
            _settingsService.UpdateAppSettings(s => s.AlwaysOnTop = value);
            OnPropertyChanged();
        }
    }

    public bool StartMinimized
    {
        get => _settingsService.Settings.StartMinimized;
        set
        {
            _settingsService.UpdateAppSettings(s => s.StartMinimized = value);
            OnPropertyChanged();
        }
    }

    public bool AutoStart
    {
        get => _settingsService.Settings.AutoStart;
        set
        {
            _settingsService.UpdateAppSettings(s => s.AutoStart = value);
            OnPropertyChanged();
            if (value)
                StatusMessage = "开机自启已启用（需管理员权限）";
            else
                StatusMessage = "开机自启已关闭";
        }
    }

    public bool EnableMouseHook
    {
        get => _settingsService.Settings.EnableMouseHook;
        set
        {
            _settingsService.UpdateAppSettings(s => s.EnableMouseHook = value);
            OnPropertyChanged();
            if (value)
            {
                _mouseHookService.InstallHook();
                StatusMessage = "鼠标钩子已启用";
            }
            else
            {
                _mouseHookService.UninstallHook();
                StatusMessage = "鼠标钩子已关闭";
            }
        }
    }

    public bool EnableDragCrosshair
    {
        get => _settingsService.Settings.EnableDragCrosshair;
        set
        {
            _settingsService.UpdateAppSettings(s => s.EnableDragCrosshair = value);
            OnPropertyChanged();
            StatusMessage = value ? "拖动调整准星已启用" : "拖动调整准星已关闭";
        }
    }

    public float CrosshairOffsetX
    {
        get => _settingsService.Settings.Crosshair.OffsetX;
        set
        {
            _settingsService.UpdateAppSettings(s => s.Crosshair.OffsetX = value);
            OnPropertyChanged();
            _crosshairService.UpdateSettings(_settingsService.Settings.Crosshair);
        }
    }

    public float CrosshairOffsetY
    {
        get => _settingsService.Settings.Crosshair.OffsetY;
        set
        {
            _settingsService.UpdateAppSettings(s => s.Crosshair.OffsetY = value);
            OnPropertyChanged();
            _crosshairService.UpdateSettings(_settingsService.Settings.Crosshair);
        }
    }

    [RelayCommand]
    private void ResetCrosshairPosition()
    {
        _settingsService.UpdateAppSettings(s =>
        {
            s.Crosshair.OffsetX = 0;
            s.Crosshair.OffsetY = 0;
        });
        OnPropertyChanged(nameof(CrosshairOffsetX));
        OnPropertyChanged(nameof(CrosshairOffsetY));
        _crosshairService.UpdateSettings(_settingsService.Settings.Crosshair);
        StatusMessage = "准星位置已重置到屏幕中心";
    }


    public double WindowOpacity
    {
        get => _settingsService.Settings.WindowOpacity;
        set
        {
            _settingsService.UpdateAppSettings(s => s.WindowOpacity = value);
            OnPropertyChanged();
        }
    }

    public double CrosshairScale
    {
        get => _settingsService.Settings.CrosshairScale;
        set
        {
            _settingsService.UpdateAppSettings(s => s.CrosshairScale = value);
            OnPropertyChanged();
            _crosshairService.UpdateSettings(CrosshairSettings.ToModel());
        }
    }

    public int RightClickHoldThresholdMs
    {
        get => _settingsService.Settings.RightClickHoldThresholdMs;
        set
        {
            _settingsService.UpdateAppSettings(s => s.RightClickHoldThresholdMs = value);
            OnPropertyChanged();
            _mouseHookService.SetHoldThresholdMs(value);
        }
    }

    public int SelectedThemeIndex
    {
        get => _settingsService.Settings.Theme switch
        {
            "Dark" => 0,
            "Light" => 1,
            "Arknights" => 2,
            _ => 0
        };
        set
        {
            var theme = value switch
            {
                0 => "Dark",
                1 => "Light",
                2 => "Arknights",
                _ => "Dark"
            };
            _settingsService.UpdateAppSettings(s => s.Theme = theme);
            OnPropertyChanged();
            ApplyTheme(theme);
            StatusMessage = "主题已更改";
        }
    }

    private void ApplyTheme(string theme)
    {
        if (App.Current is App app)
        {
            app.ApplyTheme(theme);
        }
    }

    public int SelectedHotkeyIndex
    {
        get
        {
            var current = _settingsService.Settings.HotkeyMouseButton;
            for (int i = 0; i < _hotkeyValues.Count; i++)
            {
                if (_hotkeyValues[i] == current)
                    return i;
            }
            return 0;
        }
        set
        {
            if (value >= 0 && value < _hotkeyValues.Count)
            {
                var hotkey = _hotkeyValues[value];
                _settingsService.UpdateAppSettings(s => s.HotkeyMouseButton = hotkey);
                OnPropertyChanged();
                StatusMessage = "热键已更新";

                if (_settingsService.Settings.EnableMouseHook)
                {
                    _mouseHookService.UninstallHook();
                    _mouseHookService.InstallHook();
                }
            }
        }
    }

    public int SelectedTriggerModeIndex
    {
        get
        {
            return _settingsService.Settings.HotkeyTriggerMode switch
            {
                "LongPress" => 0,
                "ShortPress" => 1,
                "DoubleClick" => 2,
                _ => 0
            };
        }
        set
        {
            var mode = value switch
            {
                0 => "LongPress",
                1 => "ShortPress",
                2 => "DoubleClick",
                _ => "LongPress"
            };
            _settingsService.UpdateAppSettings(s => s.HotkeyTriggerMode = mode);
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsLongPressMode));
            StatusMessage = "触发方式已更新";
        }
    }

    public bool IsLongPressMode => _settingsService.Settings.HotkeyTriggerMode == "LongPress" || _settingsService.Settings.HotkeyTriggerMode == "ShortPress";

    public int HotkeyClickCount
    {
        get => _settingsService.Settings.HotkeyClickCount;
        set
        {
            _settingsService.UpdateAppSettings(s => s.HotkeyClickCount = value);
            OnPropertyChanged();
        }
    }

    public MainViewModel(
        ISettingsService settingsService,
        ICrosshairService crosshairService,
        IMouseHookService mouseHookService,
        IPresetService presetService)
    {
        _settingsService = settingsService;
        _crosshairService = crosshairService;
        _mouseHookService = mouseHookService;
        _presetService = presetService;

        var displayOptions = new List<string>();
        var values = new List<string>();
        foreach (var key in Win32Api.AllKeys)
        {
            displayOptions.Add(key.Name);
            values.Add(key.Code.ToString());
        }
        AllHotkeyDisplayOptions = displayOptions;
        _hotkeyValues = values;

        _settingsService.Load();

        _crosshairSettings = new CrosshairSettingsViewModel(_settingsService.Settings.Crosshair);
        _isCrosshairEnabled = _settingsService.Settings.IsCrosshairEnabled;

        BuiltInPresets = _presetService.GetBuiltInPresets();
        CustomPresets = _presetService.GetCustomPresets();

        _crosshairSettings.PropertyChanged += (s, e) =>
        {
            _settingsService.UpdateCrosshairSettings(_crosshairSettings.ToModel());
            _crosshairService.UpdateSettings(_crosshairSettings.ToModel());
        };

        _mouseHookService.RightButtonLongPressed += OnRightButtonLongPressed;
        _mouseHookService.SetHoldThresholdMs(_settingsService.Settings.RightClickHoldThresholdMs);

        if (_isCrosshairEnabled)
        {
            _crosshairService.Show();
        }

        if (_settingsService.Settings.EnableMouseHook)
        {
            _mouseHookService.InstallHook();
        }
    }

    [RelayCommand]
    private void ToggleCrosshair()
    {
        IsCrosshairEnabled = !IsCrosshairEnabled;

        if (IsCrosshairEnabled)
        {
            _crosshairService.Show();
            StatusMessage = "准星已开启";
        }
        else
        {
            _crosshairService.Hide();
            StatusMessage = "准星已关闭";
        }

        _settingsService.UpdateAppSettings(s => s.IsCrosshairEnabled = IsCrosshairEnabled);
    }

    [RelayCommand]
    private void ApplyPreset(CrosshairPreset? preset)
    {
        if (preset == null)
            return;

        CrosshairSettings = new CrosshairSettingsViewModel(preset.Settings);
        _settingsService.UpdateCrosshairSettings(preset.Settings);
        _crosshairService.UpdateSettings(preset.Settings);
        StatusMessage = $"已应用预设: {preset.Name}";
    }

    [RelayCommand]
    private void SavePreset()
    {
        var preset = new CrosshairPreset
        {
            Name = $"自定义预设 {DateTime.Now:yyyyMMdd_HHmmss}",
            Description = "用户自定义保存的预设",
            Author = "User",
            Settings = CrosshairSettings.ToModel()
        };

        _presetService.SaveCustomPreset(preset);
        StatusMessage = $"预设已保存: {preset.Name}";
    }

    [ObservableProperty]
    private string _importCode = string.Empty;

    [RelayCommand]
    private void ImportFromCode()
    {
        if (string.IsNullOrWhiteSpace(ImportCode))
        {
            StatusMessage = "请输入 CS2 准星代码";
            return;
        }

        var settings = _presetService.ImportFromCs2Code(ImportCode);
        if (settings != null)
        {
            CrosshairSettings = new CrosshairSettingsViewModel(settings);
            _settingsService.UpdateCrosshairSettings(settings);
            _crosshairService.UpdateSettings(settings);
            StatusMessage = "已成功导入准星设置";
        }
        else
        {
            StatusMessage = "导入失败，请检查代码格式";
        }
    }

    [ObservableProperty]
    private string _exportedCode = string.Empty;

    [RelayCommand]
    private void ExportCurrentCode()
    {
        ExportedCode = _presetService.ExportToCs2Code(CrosshairSettings.ToModel());
        StatusMessage = "准星代码已生成，可复制使用";
    }

    [RelayCommand]
    private void ResetSettings()
    {
        var defaultSettings = new AppSettings();
        _settingsService.Reset();
        _settingsService.Load();
        CrosshairSettings = new CrosshairSettingsViewModel(_settingsService.Settings.Crosshair);
        IsCrosshairEnabled = _settingsService.Settings.IsCrosshairEnabled;
        OnPropertyChanged(nameof(MinimizeToTray));
        OnPropertyChanged(nameof(ShowInTaskbar));
        OnPropertyChanged(nameof(AlwaysOnTop));
        OnPropertyChanged(nameof(StartMinimized));
        OnPropertyChanged(nameof(AutoStart));
        OnPropertyChanged(nameof(EnableMouseHook));
        OnPropertyChanged(nameof(EnableDragCrosshair));
        OnPropertyChanged(nameof(CrosshairOffsetX));
        OnPropertyChanged(nameof(CrosshairOffsetY));
        OnPropertyChanged(nameof(WindowOpacity));
        OnPropertyChanged(nameof(CrosshairScale));
        OnPropertyChanged(nameof(RightClickHoldThresholdMs));
        OnPropertyChanged(nameof(SelectedThemeIndex));
        OnPropertyChanged(nameof(SelectedHotkeyIndex));
        OnPropertyChanged(nameof(SelectedTriggerModeIndex));
        OnPropertyChanged(nameof(IsLongPressMode));
        OnPropertyChanged(nameof(HotkeyClickCount));
        StatusMessage = "设置已重置为默认值";
    }

    private void OnRightButtonLongPressed(object? sender, EventArgs e)
    {
        ToggleCrosshair();
    }
}
