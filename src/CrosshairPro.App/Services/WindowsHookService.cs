using System.Diagnostics;
using System.Runtime.InteropServices;
using CrosshairPro.App.Helpers;
using CrosshairPro.Core.Services;

namespace CrosshairPro.App.Services;

public class WindowsHookService : IMouseHookService
{
    private readonly ISettingsService _settingsService;
    private IntPtr _mouseHookId = IntPtr.Zero;
    private IntPtr _keyboardHookId = IntPtr.Zero;
    private Win32Api.LowLevelProc? _mouseProc;
    private Win32Api.LowLevelProc? _keyboardProc;

    private bool _isButtonDown;
    private Stopwatch? _buttonStopwatch;
    private bool _hasTriggeredLongPress;
    private DateTime _lastClickTime = DateTime.MinValue;
    private int _clickCount;

    private bool _isDragging;
    private bool _hasDragged;
    private int _dragStartX;
    private int _dragStartY;
    private float _dragStartOffsetX;
    private float _dragStartOffsetY;
    private const int DragThreshold = 5;

    public bool IsHookInstalled => _mouseHookId != IntPtr.Zero || _keyboardHookId != IntPtr.Zero;

    public event EventHandler? RightButtonLongPressed;
    public event EventHandler? RightButtonPressed;
    public event EventHandler? RightButtonReleased;

    public WindowsHookService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void InstallHook()
    {
        if (IsHookInstalled)
            return;

        _buttonStopwatch = new Stopwatch();

        var vkCode = GetTargetVirtualKey();

        if (vkCode is 0x01 or 0x02 or 0x04)
        {
            _mouseProc = MouseHookProc;
            _mouseHookId = SetHook(Win32Api.WH_MOUSE_LL, _mouseProc);
        }
        else
        {
            _keyboardProc = KeyboardHookProc;
            _keyboardHookId = SetHook(Win32Api.WH_KEYBOARD_LL, _keyboardProc);
        }
    }

    public void UninstallHook()
    {
        if (_mouseHookId != IntPtr.Zero)
        {
            Win32Api.UnhookWindowsHookEx(_mouseHookId);
            _mouseHookId = IntPtr.Zero;
        }

        if (_keyboardHookId != IntPtr.Zero)
        {
            Win32Api.UnhookWindowsHookEx(_keyboardHookId);
            _keyboardHookId = IntPtr.Zero;
        }

        _buttonStopwatch?.Stop();
        _buttonStopwatch = null;
    }

    public void SetHoldThresholdMs(int milliseconds)
    {
    }

    private IntPtr SetHook(int idHook, Win32Api.LowLevelProc proc)
    {
        return Win32Api.SetWindowsHookEx(idHook, proc, IntPtr.Zero, 0);
    }

    private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var vkCode = GetTargetVirtualKey();
            uint downMsg = 0, upMsg = 0;

            switch (vkCode)
            {
                case 0x01:
                    downMsg = Win32Api.WM_LBUTTONDOWN;
                    upMsg = Win32Api.WM_LBUTTONUP;
                    break;
                case 0x02:
                    downMsg = Win32Api.WM_RBUTTONDOWN;
                    upMsg = Win32Api.WM_RBUTTONUP;
                    break;
                case 0x04:
                    downMsg = Win32Api.WM_MBUTTONDOWN;
                    upMsg = Win32Api.WM_MBUTTONUP;
                    break;
            }

            if ((uint)wParam == Win32Api.WM_MOUSEMOVE)
            {
                if (_isDragging)
                {
                    var mouseStruct = Marshal.PtrToStructure<Win32Api.MSLLHOOKSTRUCT>(lParam);
                    HandleDragMove(mouseStruct.pt.x, mouseStruct.pt.y);
                }
            }
            else if ((uint)wParam == downMsg)
            {
                var mouseStruct = Marshal.PtrToStructure<Win32Api.MSLLHOOKSTRUCT>(lParam);
                HandleButtonDown(mouseStruct.pt.x, mouseStruct.pt.y);
            }
            else if ((uint)wParam == upMsg)
            {
                HandleButtonUp();
            }
        }

        return Win32Api.CallNextHookEx(_mouseHookId, nCode, wParam, lParam);
    }

    private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var kbdStruct = Marshal.PtrToStructure<Win32Api.KBDLLHOOKSTRUCT>(lParam);
            var targetVk = GetTargetVirtualKey();

            if ((uint)wParam == Win32Api.WM_KEYDOWN || (uint)wParam == Win32Api.WM_SYSKEYDOWN)
            {
                if (kbdStruct.vkCode == targetVk)
                {
                    HandleButtonDown();
                }
            }
            else if ((uint)wParam == Win32Api.WM_KEYUP || (uint)wParam == Win32Api.WM_SYSKEYUP)
            {
                if (kbdStruct.vkCode == targetVk)
                {
                    HandleButtonUp();
                }
            }
        }

        return Win32Api.CallNextHookEx(_keyboardHookId, nCode, wParam, lParam);
    }

    private uint GetTargetVirtualKey()
    {
        var hotkey = _settingsService.Settings.HotkeyMouseButton;

        if (uint.TryParse(hotkey, out var vk))
        {
            return vk;
        }

        return hotkey switch
        {
            "Left" => 0x01,
            "Right" => 0x02,
            "Middle" => 0x04,
            _ => 0x04
        };
    }

    private void HandleButtonDown(int x = 0, int y = 0)
    {
        _isButtonDown = true;
        _hasTriggeredLongPress = false;
        _buttonStopwatch?.Restart();
        RightButtonPressed?.Invoke(this, EventArgs.Empty);

        if (_settingsService.Settings.EnableDragCrosshair &&
            _settingsService.Settings.IsCrosshairEnabled)
        {
            _isDragging = true;
            _hasDragged = false;
            _dragStartX = x;
            _dragStartY = y;
            _dragStartOffsetX = _settingsService.Settings.Crosshair.OffsetX;
            _dragStartOffsetY = _settingsService.Settings.Crosshair.OffsetY;
        }

        var triggerMode = _settingsService.Settings.HotkeyTriggerMode;
        if (triggerMode == "LongPress")
        {
            _ = CheckLongPressAsync();
        }
    }

    private void HandleButtonUp()
    {
        _isButtonDown = false;
        _buttonStopwatch?.Stop();
        RightButtonReleased?.Invoke(this, EventArgs.Empty);

        if (_isDragging)
        {
            _isDragging = false;
            _settingsService.Save();
        }

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

    private void HandleDragMove(int x, int y)
    {
        if (!_isDragging) return;

        var dx = x - _dragStartX;
        var dy = y - _dragStartY;

        if (!_hasDragged && (Math.Abs(dx) > DragThreshold || Math.Abs(dy) > DragThreshold))
        {
            _hasDragged = true;
        }

        _settingsService.UpdateAppSettings(s =>
        {
            s.Crosshair.OffsetX = _dragStartOffsetX + dx;
            s.Crosshair.OffsetY = _dragStartOffsetY + dy;
        });
    }

    private void CheckShortPress()
    {
        if (_hasDragged) return;

        var threshold = _settingsService.Settings.RightClickHoldThresholdMs;
        if (_buttonStopwatch?.ElapsedMilliseconds < threshold)
        {
            RightButtonLongPressed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void CheckDoubleClick()
    {
        if (_hasDragged) return;

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

            if (_isButtonDown && IsHookInstalled && !_hasTriggeredLongPress && !_hasDragged)
            {
                _hasTriggeredLongPress = true;
                RightButtonLongPressed?.Invoke(this, EventArgs.Empty);
            }
        }
        catch
        {
        }
    }

    public void SimulateRightButtonDown() => HandleButtonDown();
    public void SimulateRightButtonUp() => HandleButtonUp();
}
