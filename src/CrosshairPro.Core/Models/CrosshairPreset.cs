namespace CrosshairPro.Core.Models;

public class CrosshairPreset
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Author { get; set; }

    public CrosshairSettings Settings { get; set; } = new();
}
