namespace LiteTick.Windows;

internal static class TextValue
{
    internal static bool IsBlank(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }
        return value!.Trim().Length == 0;
    }
}
