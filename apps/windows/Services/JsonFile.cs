using System.Text.Json;

namespace LiteTick.Windows.Services;

internal static class JsonFile
{
    internal static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    internal static void WriteAtomically<T>(string path, T value, Func<string, bool>? existingFileValidator = null)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temporaryPath = path + ".tmp";
        var json = JsonSerializer.Serialize(value, Options);
        File.WriteAllText(temporaryPath, json);

        _ = JsonSerializer.Deserialize<T>(File.ReadAllText(temporaryPath), Options)
            ?? throw new InvalidDataException("Serialized data could not be validated.");

        if (File.Exists(path) && existingFileValidator?.Invoke(path) == true)
        {
            File.Copy(path, LocalPaths.BackupFile, overwrite: true);
        }

        File.Move(temporaryPath, path, overwrite: true);
    }
}
