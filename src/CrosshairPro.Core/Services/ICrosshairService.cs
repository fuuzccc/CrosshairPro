using CrosshairPro.Core.Models;

namespace CrosshairPro.Core.Services;

public interface ICrosshairService
{
    bool IsVisible { get; }

    event EventHandler<bool>? VisibilityChanged;

    void Show();

    void Hide();

    void Toggle();

    void UpdateSettings(CrosshairSettings settings);
}
