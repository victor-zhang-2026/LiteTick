using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using LiteTick.Windows.Platform;

namespace LiteTick.Windows.UI;

internal sealed class FloatingBallWindow : Window
{
    private readonly FloatingCoordinator _coordinator;
    private readonly Ellipse _innerCircle;
    private NativeMethods.Point _dragStartCursor;
    private NativeMethods.Rect _dragStartWindow;
    private bool _dragging;
    private bool _pointerDown;

    internal FloatingBallWindow(FloatingCoordinator coordinator)
    {
        _coordinator = coordinator;
        Width = 48;
        Height = 48;
        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.NoResize;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        Topmost = true;
        ShowInTaskbar = false;
        ShowActivated = false;
        Focusable = false;
        SizeToContent = SizeToContent.Manual;

        var root = new Grid { Background = Brushes.Transparent };
        var outer = new Ellipse
        {
            Margin = new Thickness(3),
            Fill = Brushes.White,
            Effect = new DropShadowEffect
            {
                BlurRadius = 6,
                ShadowDepth = 1,
                Opacity = 0.2,
                Color = Colors.Black
            }
        };
        _innerCircle = new Ellipse
        {
            Margin = new Thickness(7),
            Fill = new SolidColorBrush(Color.FromRgb(7, 193, 96))
        };
        var check = new System.Windows.Shapes.Path
        {
            Data = Geometry.Parse("M 14.4,25 L 21.1,31.8 L 33.6,18.3"),
            Stroke = Brushes.White,
            StrokeThickness = 4.2,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            StrokeLineJoin = PenLineJoin.Round
        };
        root.Children.Add(outer);
        root.Children.Add(_innerCircle);
        root.Children.Add(check);
        Content = root;

        MouseEnter += (_, _) =>
        {
            _innerCircle.Margin = new Thickness(6);
            _innerCircle.Fill = new SolidColorBrush(Color.FromRgb(5, 154, 75));
            _coordinator.TriggerHoverChanged(true);
        };
        MouseLeave += (_, _) =>
        {
            _innerCircle.Margin = new Thickness(7);
            _innerCircle.Fill = new SolidColorBrush(Color.FromRgb(7, 193, 96));
            _coordinator.TriggerHoverChanged(false);
        };
        MouseLeftButtonDown += BeginPointerInteraction;
        MouseMove += ContinuePointerInteraction;
        MouseLeftButtonUp += EndPointerInteraction;
        MouseRightButtonUp += ShowContextMenu;
    }

    internal void RefreshLocalization()
    {
        if (ContextMenu?.IsOpen == true)
        {
            ContextMenu.IsOpen = false;
        }
    }

    private void BeginPointerInteraction(object sender, MouseButtonEventArgs eventArgs)
    {
        if (!NativeMethods.GetCursorPos(out _dragStartCursor)
            || !NativeMethods.GetWindowRect(new System.Windows.Interop.WindowInteropHelper(this).Handle, out _dragStartWindow))
        {
            return;
        }
        _pointerDown = true;
        _dragging = false;
        CaptureMouse();
        eventArgs.Handled = true;
    }

    private void ContinuePointerInteraction(object sender, MouseEventArgs eventArgs)
    {
        if (!_pointerDown || eventArgs.LeftButton != MouseButtonState.Pressed
            || !NativeMethods.GetCursorPos(out var cursor))
        {
            return;
        }

        var dx = cursor.X - _dragStartCursor.X;
        var dy = cursor.Y - _dragStartCursor.Y;
        if (!_dragging && Math.Abs(dx) + Math.Abs(dy) >= 3)
        {
            _dragging = true;
        }
        if (_dragging)
        {
            _coordinator.MoveTrigger(
                _dragStartWindow.Left + dx,
                _dragStartWindow.Top + dy,
                _dragStartWindow.Width,
                _dragStartWindow.Height);
        }
    }

    private void EndPointerInteraction(object sender, MouseButtonEventArgs eventArgs)
    {
        if (!_pointerDown)
        {
            return;
        }
        _pointerDown = false;
        ReleaseMouseCapture();
        if (_dragging)
        {
            _coordinator.FinishTriggerDrag();
        }
        else
        {
            _coordinator.TriggerClicked();
        }
        _dragging = false;
        eventArgs.Handled = true;
    }

    private void ShowContextMenu(object sender, MouseButtonEventArgs eventArgs)
    {
        var menu = new ContextMenu();
        var quit = new MenuItem { Header = _coordinator.Text.Quit };
        quit.Click += (_, _) => _coordinator.RequestQuit();
        menu.Items.Add(quit);
        ContextMenu = menu;
        menu.IsOpen = true;
        eventArgs.Handled = true;
    }
}
