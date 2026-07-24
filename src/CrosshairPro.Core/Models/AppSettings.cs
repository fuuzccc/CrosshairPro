using CrosshairPro.Core.Models;

namespace CrosshairPro.Core.Models;

public class AppSettings
{
    public CrosshairSettings Crosshair { get; set; } = new();

    public bool IsCrosshairEnabled { get; set; } = true;

    public int RightClickHoldThresholdMs { get; set; } = 500;

    public bool StartMinimized { get; set; } = false;

    public bool AutoStart { get; set; } = false;

    public string SelectedMonitor { get; set; } = "Primary";

    public double CrosshairScale { get; set; } = 1.0;

    public bool MinimizeToTray { get; set; } = true;

    public bool ShowInTaskbar { get; set; } = true;

    public bool AlwaysOnTop { get; set; } = true;

    public double WindowOpacity { get; set; } = 1.0;

    public int WindowWidth { get; set; } = 900;

    public int WindowHeight { get; set; } = 650;

    public double WindowPositionX { get; set; } = -1;

    public double WindowPositionY { get; set; } = -1;

    public string Theme { get; set; } = "Dark";

    public bool EnableMouseHook { get; set; } = true;

    public string Language { get; set; } = "zh-CN";
}
