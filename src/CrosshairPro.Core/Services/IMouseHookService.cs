namespace CrosshairPro.Core.Services;

public interface IMouseHookService
{
    event EventHandler? RightButtonLongPressed;

    event EventHandler? RightButtonPressed;

    event EventHandler? RightButtonReleased;

    bool IsHookInstalled { get; }

    void InstallHook();

    void UninstallHook();

    void SetHoldThresholdMs(int milliseconds);
}
