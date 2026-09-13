using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using LiteTick.Windows.Platform;
using LiteTick.Windows.Services;

namespace LiteTick.Windows.UI;

internal sealed class FloatingCoordinator
{
    private const int TriggerMargin = 8;
    private const int PanelMargin = 12;

    private readonly Application _application;
    private readonly TaskStore _taskStore;
    private readonly SettingsStore _settingsStore;
    private readonly DispatcherTimer _closeTimer;
    private FloatingBallWindow? _ballWindow;
    private ChecklistWindow? _checklistWindow;
    private bool _triggerHovered;
    private bool _panelHovered;
    private bool _panelPinned;
    private bool _panelLocked;
    private bool _modalOpen;
    private bool _isShuttingDown;

    internal FloatingCoordinator(Application application, TaskStore taskStore, SettingsStore settingsStore)
    {
        _application = application;
        _taskStore = taskStore;
        _settingsStore = settingsStore;
        _closeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
        _closeTimer.Tick += (_, _) => CloseIfTransient();
    }

    internal TextCatalog Text => new(IsChinese);
    internal ThemePalette Palette => new(IsDark);
    internal bool IsChinese => _settingsStore.Current.Language == "zh-CN";
    internal bool IsDark => _settingsStore.Current.Appearance == "dark";
    internal bool IsPanelLocked => _panelLocked;
    internal bool IsShuttingDown => _isShuttingDown;

    internal void Start()
    {
        _ballWindow = new FloatingBallWindow(this);
        _checklistWindow = new ChecklistWindow(this, _taskStore);
        _checklistWindow.Deactivated += (_, _) =>
        {
            if (_panelPinned && !_panelLocked && !_modalOpen)
            {
                DismissPanel();
            }
        };

        _ballWindow.Show();
        PositionInitialTrigger();
    }

    internal void TriggerHoverChanged(bool hovering)
    {
        _triggerHovered = hovering;
        if (hovering)
        {
            CancelClose();
            OpenPanel(pin: false);
        }
        else
        {
            ScheduleClose();
        }
    }

    internal void PanelHoverChanged(bool hovering)
    {
        _panelHovered = hovering;
        if (hovering)
        {
            CancelClose();
        }
        else
        {
            ScheduleClose();
        }
    }

    internal void PanelEngaged()
    {
        _panelPinned = true;
        CancelClose();
    }

    internal void TriggerClicked()
    {
        _panelPinned = true;
        OpenPanel(pin: true);
    }

    internal void MoveTrigger(int x, int y, int width, int height)
    {
        if (_ballWindow is null)
        {
            return;
        }
        NativeMethods.MoveTopmost(HandleOf(_ballWindow), x, y, width, height, activate: false);
    }

    internal void FinishTriggerDrag()
    {
        if (_ballWindow is null || !NativeMethods.GetWindowRect(HandleOf(_ballWindow), out var triggerRect))
        {
            return;
        }

        var targetPoint = NativeMethods.GetCursorPos(out var cursor) ? cursor : triggerRect.Center;
        var work = NativeMethods.WorkAreaForPoint(targetPoint);
        var settledX = Clamp(triggerRect.Left, work.Left + TriggerMargin, work.Right - triggerRect.Width - TriggerMargin);
        var settledY = Clamp(triggerRect.Top, work.Top + TriggerMargin, work.Bottom - triggerRect.Height - TriggerMargin);
        NativeMethods.MoveTopmost(HandleOf(_ballWindow), settledX, settledY, triggerRect.Width, triggerRect.Height, activate: false);

        _settingsStore.Current.TriggerX = settledX;
        _settingsStore.Current.TriggerY = settledY;
        _settingsStore.Save();
        if (_checklistWindow?.IsVisible == true)
        {
            PositionChecklist();
        }
    }

    internal void TogglePanelLock()
    {
        _panelLocked = !_panelLocked;
        if (_panelLocked)
        {
            _panelPinned = true;
            CancelClose();
        }
        else if (!_triggerHovered && !_panelHovered)
        {
            ScheduleClose();
        }
        _checklistWindow?.Refresh();
    }

    internal void ClosePanelFromButton()
    {
        _panelLocked = false;
        _panelPinned = false;
        DismissPanel();
    }

    internal void ToggleLanguage()
    {
        _settingsStore.Current.Language = IsChinese ? "en" : "zh-CN";
        _settingsStore.Save();
        _ballWindow?.RefreshLocalization();
        _checklistWindow?.Refresh();
    }

    internal void ToggleAppearance()
    {
        _settingsStore.Current.Appearance = IsDark ? "light" : "dark";
        _settingsStore.Save();
        _checklistWindow?.Refresh();
    }

    internal void ShowAbout()
    {
        var about = new AboutWindow(Text, Palette)
        {
            Owner = _checklistWindow
        };
        about.ShowDialog();
    }

    internal void RequestQuit()
    {
        var text = Text;
        if (!ShowConfirmation(
                text.QuitTitle,
                text.QuitMessage,
                text.Yes,
                text.No))
        {
            return;
        }

        _settingsStore.Save();
        _isShuttingDown = true;
        _application.Shutdown();
    }

    internal bool ShowConfirmation(
        string title,
        string message,
        string confirmLabel,
        string cancelLabel,
        bool destructive = false)
    {
        _modalOpen = true;
        CancelClose();
        try
        {
            return ConfirmationWindow.Show(
                _checklistWindow,
                title,
                message,
                confirmLabel,
                cancelLabel,
                Palette,
                destructive);
        }
        finally
        {
            _modalOpen = false;
            if (!_panelLocked && !_panelPinned && !_triggerHovered && !_panelHovered)
            {
                ScheduleClose();
            }
        }
    }

    private void OpenPanel(bool pin)
    {
        if (_checklistWindow is null)
        {
            return;
        }
        if (pin)
        {
            _panelPinned = true;
        }

        CancelClose();
        _checklistWindow.Refresh();
        if (!_checklistWindow.IsVisible)
        {
            _checklistWindow.Show();
        }
        PositionChecklist();
        _checklistWindow.Activate();
    }

    private void PositionInitialTrigger()
    {
        if (_ballWindow is null)
        {
            return;
        }
        var handle = HandleOf(_ballWindow);
        if (!NativeMethods.GetWindowRect(handle, out var rect))
        {
            return;
        }

        int x;
        int y;
        if (_settingsStore.Current.TriggerX is int savedX && _settingsStore.Current.TriggerY is int savedY)
        {
            var savedRect = new NativeMethods.Rect
            {
                Left = savedX,
                Top = savedY,
                Right = savedX + rect.Width,
                Bottom = savedY + rect.Height
            };
            var work = NativeMethods.WorkAreaForRect(savedRect);
            x = Clamp(savedX, work.Left + TriggerMargin, work.Right - rect.Width - TriggerMargin);
            y = Clamp(savedY, work.Top + TriggerMargin, work.Bottom - rect.Height - TriggerMargin);
        }
        else
        {
            var work = NativeMethods.WorkAreaForPoint(new NativeMethods.Point(0, 0), nearest: false);
            x = work.Right - rect.Width - 10;
            y = work.Top + (work.Height - rect.Height) / 2;
        }

        NativeMethods.MoveTopmost(handle, x, y, rect.Width, rect.Height, activate: false);
    }

    private void PositionChecklist()
    {
        if (_ballWindow is null || _checklistWindow is null)
        {
            return;
        }
        var triggerHandle = HandleOf(_ballWindow);
        var checklistHandle = HandleOf(_checklistWindow);
        if (!NativeMethods.GetWindowRect(triggerHandle, out var triggerRect)
            || !NativeMethods.GetWindowRect(checklistHandle, out var panelRect))
        {
            return;
        }

        var work = NativeMethods.WorkAreaForRect(triggerRect);
        var spaceOnLeft = triggerRect.Left - work.Left - PanelMargin;
        var spaceOnRight = work.Right - triggerRect.Right - PanelMargin;
        var proposedX = spaceOnLeft >= panelRect.Width || spaceOnLeft >= spaceOnRight
            ? triggerRect.Left - panelRect.Width
            : triggerRect.Right;
        var x = Clamp(proposedX, work.Left + PanelMargin, work.Right - panelRect.Width - PanelMargin);
        var centeredY = triggerRect.Top + (triggerRect.Height - panelRect.Height) / 2;
        var y = Clamp(centeredY, work.Top + PanelMargin, work.Bottom - panelRect.Height - PanelMargin);
        NativeMethods.MoveTopmost(checklistHandle, x, y, panelRect.Width, panelRect.Height, activate: true);
    }

    private void ScheduleClose()
    {
        _closeTimer.Stop();
        _closeTimer.Start();
    }

    private void CancelClose()
    {
        _closeTimer.Stop();
    }

    private void CloseIfTransient()
    {
        _closeTimer.Stop();
        if (!_modalOpen && !_panelLocked && !_panelPinned && !_triggerHovered && !_panelHovered)
        {
            DismissPanel();
        }
    }

    private void DismissPanel()
    {
        CancelClose();
        _panelPinned = false;
        _checklistWindow?.Hide();
    }

    private static IntPtr HandleOf(Window window) => new WindowInteropHelper(window).Handle;

    private static int Clamp(int value, int minimum, int maximum)
    {
        return maximum < minimum ? minimum : Math.Min(Math.Max(value, minimum), maximum);
    }
}
