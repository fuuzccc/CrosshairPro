using Avalonia.Controls;
using Avalonia.Platform;
using CrosshairPro.App.Helpers;

namespace CrosshairPro.App.Helpers;

public static class WindowHelper
{
    public static void MakeClickThrough(Window window)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                var handle = window.TryGetPlatformHandle();
                if (handle != null && handle.Handle != IntPtr.Zero)
                {
                    var exStyle = Win32Api.GetWindowLong(handle.Handle, Win32Api.GWL_EXSTYLE);
                    Win32Api.SetWindowLong(handle.Handle, Win32Api.GWL_EXSTYLE,
                        exStyle | Win32Api.WS_EX_TRANSPARENT | Win32Api.WS_EX_LAYERED | Win32Api.WS_EX_TOOLWINDOW);
                }
            }
        }
        catch
        {
        }
    }

    public static void MakeNormalWindow(Window window)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                var handle = window.TryGetPlatformHandle();
                if (handle != null && handle.Handle != IntPtr.Zero)
                {
                    var exStyle = Win32Api.GetWindowLong(handle.Handle, Win32Api.GWL_EXSTYLE);
                    Win32Api.SetWindowLong(handle.Handle, Win32Api.GWL_EXSTYLE,
                        exStyle & ~Win32Api.WS_EX_TRANSPARENT);
                }
            }
        }
        catch
        {
        }
    }
}
