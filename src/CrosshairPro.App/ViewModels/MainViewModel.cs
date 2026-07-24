using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrosshairPro.Core.Services;
using CrosshairPro.Core.Models;

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

    public IReadOnlyList<CrosshairPreset> BuiltInPresets { get; }
    public IReadOnlyList<CrosshairPreset> CustomPresets { get; }

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

        if (_isCrosshairEnabled)
        {
            _crosshairService.Show();
        }

        _mouseHookService.InstallHook();
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

    [RelayCommand]
    private void ImportFromCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            StatusMessage = "请输入 CS2 准星代码";
            return;
        }

        var settings = _presetService.ImportFromCs2Code(code);
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

    private void OnRightButtonLongPressed(object? sender, EventArgs e)
    {
        ToggleCrosshair();
    }
}
