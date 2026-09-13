using LiteTick.Windows.Services;

var testDirectory = Path.Combine(Path.GetTempPath(), "litetick-core-smoke-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(testDirectory);
Environment.SetEnvironmentVariable("LITETICK_DATA_DIR", testDirectory);

try
{
    var settings = new SettingsStore();
    Check(settings.Current.Language is "zh-CN" or "en", "default language");
    Check(settings.Current.Appearance == "light", "default appearance");
    settings.Current.Appearance = "dark";
    settings.Current.TriggerX = -1440;
    settings.Current.TriggerY = 240;
    settings.Save();

    var reloadedSettings = new SettingsStore();
    Check(reloadedSettings.Current.Appearance == "dark", "settings appearance persistence");
    Check(reloadedSettings.Current.TriggerX == -1440 && reloadedSettings.Current.TriggerY == 240, "settings position persistence");

    var store = new TaskStore();
    store.Add("Alpha");
    store.Add("Beta");
    Check(string.Join(",", store.Pending.Select(item => item.Text)) == "Beta,Alpha", "new tasks appear first");

    var alpha = store.Pending.Single(item => item.Text == "Alpha");
    store.TogglePin(alpha.Id);
    store.Add("Gamma");
    Check(string.Join(",", store.Pending.Select(item => item.Text)) == "Alpha,Gamma,Beta", "pinned item stays first");

    var beta = store.Pending.Single(item => item.Text == "Beta");
    var gamma = store.Pending.Single(item => item.Text == "Gamma");
    store.Move(beta.Id, gamma.Id);
    Check(string.Join(",", store.Pending.Select(item => item.Text)) == "Alpha,Beta,Gamma", "reorder respects pinned boundary");

    store.Update(beta.Id, "Beta updated");
    store.Complete(beta.Id);
    Check(store.Completed.Single().Text == "Beta updated", "completion and edit persistence");
    store.Restore(beta.Id);
    Check(string.Join(",", store.Pending.Select(item => item.Text)) == "Alpha,Beta updated,Gamma", "restore follows pinned item");

    store.Complete(gamma.Id);
    store.DeleteCompleted([gamma.Id]);
    Check(store.Completed.Count == 0, "completed deletion");
    Check(File.Exists(Path.Combine(testDirectory, "items.json")), "primary task file exists");
    Check(File.Exists(Path.Combine(testDirectory, "items.backup.json")), "valid backup exists");

    File.WriteAllText(Path.Combine(testDirectory, "items.json"), "not valid json");
    var recovered = new TaskStore();
    Check(recovered.Pending.Count > 0, "backup recovery");

    Console.WriteLine("Core smoke tests passed.");
}
finally
{
    Directory.Delete(testDirectory, recursive: true);
}

static void Check(bool condition, string name)
{
    if (!condition)
    {
        throw new InvalidOperationException($"Failed: {name}");
    }
}
