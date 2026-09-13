namespace LiteTick.Windows.Services;

internal static class LocalPaths
{
    internal static string DataDirectory { get; } = ResolveDataDirectory();

    internal static string ItemsFile => Path.Combine(DataDirectory, "items.json");
    internal static string BackupFile => Path.Combine(DataDirectory, "items.backup.json");
    internal static string SettingsFile => Path.Combine(DataDirectory, "settings.json");

    private static string ResolveDataDirectory()
    {
        var testOverride = Environment.GetEnvironmentVariable("LITETICK_DATA_DIR");
        return string.IsNullOrWhiteSpace(testOverride)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LiteTick")
            : Path.GetFullPath(testOverride);
    }
}
