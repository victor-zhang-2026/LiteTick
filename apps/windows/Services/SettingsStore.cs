using System.Globalization;
using System.Text.Json;
using LiteTick.Windows.Models;

namespace LiteTick.Windows.Services;

public sealed class SettingsStore
{
    public AppSettings Current { get; private set; }

    public SettingsStore()
    {
        Current = Load();
        var needsSave = false;
        if (string.IsNullOrWhiteSpace(Current.Language))
        {
            Current.Language = CultureInfo.CurrentUICulture.Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase)
                ? "zh-CN"
                : "en";
            needsSave = true;
        }
        if (Current.Appearance is not ("light" or "dark"))
        {
            Current.Appearance = "light";
            needsSave = true;
        }
        if (needsSave)
        {
            Save();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(LocalPaths.DataDirectory);
        var temporaryPath = LocalPaths.SettingsFile + ".tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(Current, JsonFile.Options));
        File.Move(temporaryPath, LocalPaths.SettingsFile, overwrite: true);
    }

    private static AppSettings Load()
    {
        try
        {
            if (!File.Exists(LocalPaths.SettingsFile))
            {
                return new AppSettings();
            }
            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(LocalPaths.SettingsFile), JsonFile.Options)
                ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }
}
