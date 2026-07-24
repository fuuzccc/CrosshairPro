using CrosshairPro.Core.Models;

namespace CrosshairPro.Core.Services;

public interface ISettingsService
{
    AppSettings Settings { get; }

    event EventHandler? SettingsChanged;

    void Load();

    void Save();

    void Reset();

    void UpdateCrosshairSettings(CrosshairSettings settings);

    void UpdateAppSettings(Action<AppSettings> updateAction);
}
