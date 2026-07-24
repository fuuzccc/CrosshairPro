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
}
