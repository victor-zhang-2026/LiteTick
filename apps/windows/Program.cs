using System.Threading;
using System.Windows;
using System.Windows.Threading;
using LiteTick.Windows.Services;
using LiteTick.Windows.UI;

namespace LiteTick.Windows;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var singleInstance = new Mutex(initiallyOwned: true, "LiteTick.Windows.SingleInstance", out var ownsMutex);
        if (!ownsMutex)
        {
            return;
        }

        var application = new Application
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown
        };
        application.DispatcherUnhandledException += HandleUnhandledException;

        var settingsStore = new SettingsStore();
        var taskStore = new TaskStore();
        var coordinator = new FloatingCoordinator(application, taskStore, settingsStore);
        coordinator.Start();
        application.Run();
    }

    private static void HandleUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs eventArgs)
    {
        MessageBox.Show(
            $"LiteTick encountered an unexpected error.\n\n{eventArgs.Exception.Message}",
            "LiteTick",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        eventArgs.Handled = true;
    }
}
