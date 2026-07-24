using Newtonsoft.Json;
using CrosshairPro.Core.Models;
using CrosshairPro.Core.Services;

namespace CrosshairPro.Core.Services;

public class SettingsService : ISettingsService
{
    private readonly string _settingsPath;
    private AppSettings _settings = new();

    public AppSettings Settings => _settings;

    public event EventHandler? SettingsChanged;

    public SettingsService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var settingsDir = Path.Combine(appDataPath, "CrosshairPro");
        Directory.CreateDirectory(settingsDir);
        _settingsPath = Path.Combine(settingsDir, "settings.json");
    }

    public void Load()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                if (settings != null)
                {
                    _settings = settings;
                }
            }
        }
        catch
        {
            _settings = new AppSettings();
        }

        OnSettingsChanged();
    }

    public void Save()
    {
        try
        {
            var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            File.WriteAllText(_settingsPath, json);
        }
        catch
        {
            // 忽略保存错误
        }
    }

    public void UpdateCrosshairSettings(CrosshairSettings settings)
    {
        _settings.Crosshair = settings;
        Save();
        OnSettingsChanged();
    }

    public void UpdateAppSettings(Action<AppSettings> updateAction)
    {
        updateAction(_settings);
        Save();
        OnSettingsChanged();
    }

    protected virtual void OnSettingsChanged()
    {
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
}
