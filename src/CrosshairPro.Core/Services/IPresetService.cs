using CrosshairPro.Core.Models;

namespace CrosshairPro.Core.Services;

public interface IPresetService
{
    IReadOnlyList<CrosshairPreset> GetBuiltInPresets();

    IReadOnlyList<CrosshairPreset> GetCustomPresets();

    void SaveCustomPreset(CrosshairPreset preset);

    void DeleteCustomPreset(string name);

    CrosshairSettings? ImportFromCs2Code(string code);

    string ExportToCs2Code(CrosshairSettings settings);
}
