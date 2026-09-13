namespace LiteTick.Windows.Models;

public sealed class AppSettings
{
    public string Language { get; set; } = string.Empty;
    public string Appearance { get; set; } = "light";
    public int? TriggerX { get; set; }
    public int? TriggerY { get; set; }
}
