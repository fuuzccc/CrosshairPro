using System.Diagnostics;
using CrosshairPro.Core.Services;

namespace CrosshairPro.App.Services;

public class MouseHookService : IMouseHookService
{
    private readonly ISettingsService _settingsService;
    private bool _isHookInstalled;
    private bool _isRightButtonDown;
    private Stopwatch? _rightButtonStopwatch;
    private bool _hasTriggeredLongPress;

    public bool IsHookInstalled => _isHookInstalled;

    public event EventHandler? RightButtonLongPressed;
    public event EventHandler? RightButtonPressed;
    public event EventHandler? RightButtonReleased;

    public MouseHookService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void InstallHook()
    {
        if (_isHookInstalled)
            return;

        _isHookInstalled = true;
        _rightButtonStopwatch = new Stopwatch();
    }

    public void UninstallHook()
    {
        if (!_isHookInstalled)
            return;

        _isHookInstalled = false;
        _rightButtonStopwatch?.Stop();
        _rightButtonStopwatch = null;
    }

    public void SetHoldThresholdMs(int milliseconds)
    {
        // 阈值会在下次检测时从 settings 中读取
    }

    public void SimulateRightButtonDown()
    {
        if (!_isHookInstalled)
            return;

        _isRightButtonDown = true;
        _hasTriggeredLongPress = false;
        _rightButtonStopwatch?.Restart();
        RightButtonPressed?.Invoke(this, EventArgs.Empty);

        _ = CheckLongPressAsync();
    }

    public void SimulateRightButtonUp()
    {
        if (!_isHookInstalled)
            return;

        _isRightButtonDown = false;
        _rightButtonStopwatch?.Stop();
        RightButtonReleased?.Invoke(this, EventArgs.Empty);
    }

    private async Task CheckLongPressAsync()
    {
        var threshold = _settingsService.Settings.RightClickHoldThresholdMs;

        try
        {
            await Task.Delay(threshold);

            if (_isRightButtonDown && _isHookInstalled && !_hasTriggeredLongPress)
            {
                _hasTriggeredLongPress = true;
                RightButtonLongPressed?.Invoke(this, EventArgs.Empty);
            }
        }
        catch
        {
            // 忽略任务取消等异常
        }
    }
}
