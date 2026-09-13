using System.Windows.Media;

namespace LiteTick.Windows.UI;

internal sealed class ThemePalette
{
    internal Brush WindowBackground { get; }
    internal Brush TextPrimary { get; }
    internal Brush TextSecondary { get; }
    internal Brush TextTertiary { get; }
    internal Brush Divider { get; }
    internal Brush FieldBackground { get; }
    internal Brush HoverBackground { get; }
    internal Brush PinnedBackground { get; }
    internal Brush Green { get; }
    internal Brush Destructive { get; }
    internal Brush Transparent { get; } = Brushes.Transparent;

    internal ThemePalette(bool dark)
    {
        WindowBackground = Brush(dark ? "#FF1E1E1E" : "#FFF6F6F6");
        TextPrimary = Brush(dark ? "#FFF2F2F2" : "#FF1F1F1F");
        TextSecondary = Brush(dark ? "#FFA7A7A7" : "#FF666666");
        TextTertiary = Brush(dark ? "#FF777777" : "#FF949494");
        Divider = Brush(dark ? "#FF3A3A3A" : "#FFD8D8D8");
        FieldBackground = Brush(dark ? "#FF292929" : "#FFFFFFFF");
        HoverBackground = Brush(dark ? "#FF2B2B2B" : "#FFECECEC");
        PinnedBackground = Brush(dark ? "#FF174D30" : "#FFD6F5E3");
        Green = Brush(dark ? "#FF18C96E" : "#FF07C160");
        Destructive = Brush(dark ? "#FFFF6B68" : "#FFD64545");
    }

    private static SolidColorBrush Brush(string value)
    {
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
        brush.Freeze();
        return brush;
    }
}
