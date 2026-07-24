using System.Diagnostics;
using CrosshairPro.Core.Services;

namespace CrosshairPro.App.Services;

public class MouseHookService : IMouseHookService
{
    private readonly ISettingsService _settingsService;
    private bool _isHookInstalled;
    private bool _isButtonDown;
    private Stopwatch? _buttonStopwatch;
    private bool _hasTriggeredLongPress;
    private DateTime _lastClickTime = DateTime.MinValue;
    private int _clickCount;

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
        _buttonStopwatch = new Stopwatch();
    }

    public void UninstallHook()
    {
        if (!_isHookInstalled)
            return;

        _isHookInstalled = false;
        _buttonStopwatch?.Stop();
        _buttonStopwatch = null;
    }

    public void SetHoldThresholdMs(int milliseconds)
    {
    }

    public void SimulateButtonDown(string button)
    {
        if (!_isHookInstalled)
            return;

        var targetButton = _settingsService.Settings.HotkeyMouseButton;
        if (button != targetButton)
            return;

        _isButtonDown = true;
        _hasTriggeredLongPress = false;
        _buttonStopwatch?.Restart();
        RightButtonPressed?.Invoke(this, EventArgs.Empty);

        var triggerMode = _settingsService.Settings.HotkeyTriggerMode;
        if (triggerMode == "LongPress")
        {
            _ = CheckLongPressAsync();
        }
    }

    public void SimulateButtonUp(string button)
    {
        if (!_isHookInstalled)
            return;

        var targetButton = _settingsService.Settings.HotkeyMouseButton;
        if (button != targetButton)
            return;

        _isButtonDown = false;
        _buttonStopwatch?.Stop();
        RightButtonReleased?.Invoke(this, EventArgs.Empty);

        var triggerMode = _settingsService.Settings.HotkeyTriggerMode;
        if (triggerMode == "ShortPress")
        {
            CheckShortPress();
        }
        else if (triggerMode == "DoubleClick")
        {
            CheckDoubleClick();
        }
    }

    public void SimulateRightButtonDown()
    {
        SimulateButtonDown("Right");
    }

    public void SimulateRightButtonUp()
    {
        SimulateButtonUp("Right");
    }

    private void CheckShortPress()
    {
        var threshold = _settingsService.Settings.RightClickHoldThresholdMs;
        if (_buttonStopwatch?.ElapsedMilliseconds < threshold)
        {
            RightButtonLongPressed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void CheckDoubleClick()
    {
        var now = DateTime.Now;
        var doubleClickThreshold = 300;

        if ((now - _lastClickTime).TotalMilliseconds <= doubleClickThreshold)
        {
            _clickCount++;
            if (_clickCount >= _settingsService.Settings.HotkeyClickCount)
            {
                RightButtonLongPressed?.Invoke(this, EventArgs.Empty);
                _clickCount = 0;
            }
        }
        else
        {
            _clickCount = 1;
        }

        _lastClickTime = now;
    }

    private async Task CheckLongPressAsync()
    {
        var threshold = _settingsService.Settings.RightClickHoldThresholdMs;

        try
        {
            await Task.Delay(threshold);

            if (_isButtonDown && _isHookInstalled && !_hasTriggeredLongPress)
            {
                _hasTriggeredLongPress = true;
                RightButtonLongPressed?.Invoke(this, EventArgs.Empty);
            }
        }
        catch
        {
        }
    }
}
