using System.Windows.Media;

namespace LiteTick.Windows.UI;

internal static class IconGlyphs
{
#if NETFRAMEWORK
    internal const string Back = "←";
    internal const string Language = "文";
    internal const string Light = "☀";
    internal const string Dark = "☾";
    internal const string Completed = "✓";
    internal const string Lock = "●";
    internal const string Unlock = "○";
    internal const string Close = "×";
    internal const string Pin = "↑";
    internal const string Delete = "×";
    internal static FontFamily FontFamily { get; } = new("Segoe UI Symbol, Segoe UI");
#else
    internal const string Back = "\uE72B";
    internal const string Language = "\uE774";
    internal const string Light = "\uE706";
    internal const string Dark = "\uE708";
    internal const string Completed = "\uE73E";
    internal const string Lock = "\uE77A";
    internal const string Unlock = "\uE77A";
    internal const string Close = "\uE711";
    internal const string Pin = "\uE718";
    internal const string Delete = "\uE74D";
    internal static FontFamily FontFamily { get; } = new("Segoe Fluent Icons, Segoe MDL2 Assets");
#endif
}
