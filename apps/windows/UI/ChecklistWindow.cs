using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using LiteTick.Windows.Models;
using LiteTick.Windows.Services;

namespace LiteTick.Windows.UI;

internal sealed class ChecklistWindow : Window
{
    private const string ReorderFormat = "LiteTick.TaskReorder";

    private readonly FloatingCoordinator _coordinator;
    private readonly TaskStore _store;
    private readonly Border _shell;
    private StackPanel? _completedStack;
    private Button? _deleteCompletedButton;
    private TextBox? _captureInput;
    private bool _showingCompleted;
    private bool _dragInProgress;
    private string _searchQuery = string.Empty;

    internal ChecklistWindow(FloatingCoordinator coordinator, TaskStore store)
    {
        _coordinator = coordinator;
        _store = store;
        Width = 372;
        Height = 492;
        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.NoResize;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        Topmost = true;
        ShowInTaskbar = false;
        SizeToContent = SizeToContent.Manual;

        _shell = new Border
        {
            Margin = new Thickness(6),
            CornerRadius = new CornerRadius(14),
            Effect = new DropShadowEffect
            {
                BlurRadius = 16,
                ShadowDepth = 3,
                Opacity = 0.24,
                Color = Colors.Black
            }
        };
        Content = _shell;

        MouseEnter += (_, _) => _coordinator.PanelHoverChanged(true);
        MouseLeave += (_, _) => _coordinator.PanelHoverChanged(false);
        PreviewMouseDown += (_, _) => _coordinator.PanelEngaged();
        PreviewKeyDown += (_, eventArgs) =>
        {
            if (eventArgs.Key == Key.Escape)
            {
                _coordinator.ClosePanelFromButton();
                eventArgs.Handled = true;
            }
        };
        Closing += HandleClosing;
    }

    internal void Refresh()
    {
        var palette = _coordinator.Palette;
        _shell.Background = palette.WindowBackground;
        _shell.BorderBrush = palette.Divider;
        _shell.BorderThickness = new Thickness(1);
        _shell.Child = _showingCompleted
            ? BuildCompletedScreen(palette, _coordinator.Text)
            : BuildMainScreen(palette, _coordinator.Text);
    }

    private Grid BuildMainScreen(ThemePalette palette, TextCatalog text)
    {
        var layout = new Grid();
        layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(48) });
        layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(64) });
        layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        layout.Children.Add(BuildHeader(palette, text));

        var capture = BuildInputField(palette, text.AddPlaceholder, out var input);
        _captureInput = input;
        Grid.SetRow(capture, 1);
        layout.Children.Add(capture);
        input.KeyDown += (_, eventArgs) =>
        {
            if (eventArgs.Key != Key.Enter)
            {
                return;
            }
            _store.Add(input.Text);
            input.Clear();
            Refresh();
            Dispatcher.BeginInvoke(() =>
            {
                _captureInput?.Focus();
                if (_captureInput is not null)
                {
                    _captureInput.CaretIndex = _captureInput.Text.Length;
                }
            });
            eventArgs.Handled = true;
        };

        var pending = _store.Pending;
        UIElement content;
        if (pending.Count == 0)
        {
            content = EmptyState(
                _store.Completed.Count == 0 ? text.NewListEmpty : text.NoPending,
                palette);
        }
        else
        {
            var stack = new StackPanel();
            foreach (var item in pending)
            {
                stack.Children.Add(CreatePendingRow(item, palette, text));
            }

            var endDropZone = new Border
            {
                Height = 30,
                Background = Brushes.Transparent,
                AllowDrop = true
            };
            endDropZone.DragOver += (_, eventArgs) =>
            {
                eventArgs.Effects = eventArgs.Data.GetDataPresent(ReorderFormat)
                    ? DragDropEffects.Move
                    : DragDropEffects.None;
                eventArgs.Handled = true;
            };
            endDropZone.Drop += (_, eventArgs) =>
            {
                if (TryGetDraggedId(eventArgs.Data, out var draggedId))
                {
                    _store.MoveToEnd(draggedId);
                    Refresh();
                }
                eventArgs.Handled = true;
            };
            stack.Children.Add(endDropZone);
            content = new ScrollViewer
            {
                Content = stack,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };
        }
        Grid.SetRow(content, 2);
        layout.Children.Add(content);
        return layout;
    }

    private Grid BuildCompletedScreen(ThemePalette palette, TextCatalog text)
    {
        var layout = new Grid();
        layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(48) });
        layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(62) });
        layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        layout.Children.Add(BuildHeader(palette, text));

        var searchField = BuildInputField(palette, text.SearchCompleted, out var search);
        search.Text = _searchQuery;
        search.CaretIndex = search.Text.Length;
        search.TextChanged += (_, _) =>
        {
            _searchQuery = search.Text;
            RefreshCompletedRows(palette, text);
        };
        Grid.SetRow(searchField, 1);
        layout.Children.Add(searchField);

        _completedStack = new StackPanel();
        var scroll = new ScrollViewer
        {
            Content = _completedStack,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };
        Grid.SetRow(scroll, 2);
        layout.Children.Add(scroll);

        _deleteCompletedButton = new Button
        {
            Margin = new Thickness(12, 8, 12, 12),
            Padding = new Thickness(10, 5, 10, 5),
            HorizontalAlignment = HorizontalAlignment.Right,
            Foreground = palette.Destructive,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            FontSize = 11,
            Template = FlatButtonTemplate(palette.HoverBackground, new CornerRadius(5))
        };
        _deleteCompletedButton.Click += (_, _) => DeleteCompletedScope(text);
        Grid.SetRow(_deleteCompletedButton, 3);
        layout.Children.Add(_deleteCompletedButton);

        RefreshCompletedRows(palette, text);
        return layout;
    }

    private Grid BuildHeader(ThemePalette palette, TextCatalog text)
    {
        var header = new Grid { Margin = new Thickness(16, 0, 16, 0) };
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var identity = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center
        };
        if (_showingCompleted)
        {
            identity.Children.Add(IconButton(IconGlyphs.Back, text.BackToList, palette, (_, _) =>
            {
                _showingCompleted = false;
                _searchQuery = string.Empty;
                Refresh();
            }));
            identity.Children.Add(new TextBlock
            {
                Text = text.Completed,
                Foreground = palette.TextPrimary,
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 0, 0)
            });
        }
        else
        {
            var logo = IconButton("✓", text.About, palette, (_, _) => _coordinator.ShowAbout());
            logo.Foreground = Brushes.White;
            logo.Background = palette.Green;
            logo.FontFamily = new FontFamily("Segoe UI");
            logo.FontWeight = FontWeights.Bold;
            logo.Width = 28;
            logo.Height = 28;
            logo.Template = FlatButtonTemplate(palette.Green, new CornerRadius(8));
            identity.Children.Add(logo);
            identity.Children.Add(new TextBlock
            {
                Text = text.ProductName,
                Foreground = palette.TextPrimary,
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(8, 0, 0, 0)
            });
        }
        header.Children.Add(identity);

        var tools = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
        };
        tools.Children.Add(IconButton(IconGlyphs.Language, text.Language, palette, (_, _) => _coordinator.ToggleLanguage()));
        tools.Children.Add(IconButton(
            _coordinator.IsDark ? IconGlyphs.Light : IconGlyphs.Dark,
            _coordinator.IsDark ? text.SwitchToLight : text.SwitchToDark,
            palette,
            (_, _) => _coordinator.ToggleAppearance()));
        if (!_showingCompleted)
        {
            tools.Children.Add(IconButton(IconGlyphs.Completed, text.ShowCompleted, palette, (_, _) =>
            {
                _showingCompleted = true;
                Refresh();
            }));
        }
        tools.Children.Add(IconButton(
            CreateVerticalPinIcon(palette, _coordinator.IsPanelLocked),
            _coordinator.IsPanelLocked ? text.UnpinPanel : text.PinPanel,
            palette,
            (_, _) => _coordinator.TogglePanelLock(),
            _coordinator.IsPanelLocked ? palette.Green : palette.TextSecondary));
        tools.Children.Add(IconButton(IconGlyphs.Close, text.ClosePanel, palette, (_, _) => _coordinator.ClosePanelFromButton(), palette.TextSecondary));
        Grid.SetColumn(tools, 1);
        header.Children.Add(tools);
        return header;
    }

    private Border CreatePendingRow(TaskItem item, ThemePalette palette, TextCatalog text)
    {
        var row = new Border
        {
            Background = item.IsPinned ? palette.PinnedBackground : Brushes.Transparent,
            BorderBrush = palette.Divider,
            BorderThickness = new Thickness(0, 0, 0, 0.5),
            MinHeight = 40,
            Padding = new Thickness(10, 11, 10, 11),
            AllowDrop = true
        };
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(28) });

        var completion = CompletionButton(completed: false, palette, text.CompleteItem);
#if NET35
        completion.Click += (_, _) =>
        {
            completion.Content = CompletionMark(completed: true, palette);
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(220) };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                _store.Complete(item.Id);
                Refresh();
            };
            timer.Start();
        };
#else
        completion.Click += async (_, _) =>
        {
            completion.Content = CompletionMark(completed: true, palette);
            await Task.Delay(220);
            _store.Complete(item.Id);
            Refresh();
        };
#endif
        grid.Children.Add(completion);

        var editorHost = new Grid { Margin = new Thickness(10, 0, 4, 0), MinHeight = 24 };
        Grid.SetColumn(editorHost, 1);
        grid.Children.Add(editorHost);

        void ShowText()
        {
            editorHost.Children.Clear();
            var label = new TextBlock
            {
                Text = item.Text,
                Foreground = palette.TextPrimary,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                Cursor = Cursors.IBeam
            };
            label.MouseLeftButtonDown += (_, eventArgs) =>
            {
                ShowEditor();
                eventArgs.Handled = true;
            };
            editorHost.Children.Add(label);
        }

        void ShowEditor()
        {
            editorHost.Children.Clear();
            var editor = new TextBox
            {
                Text = item.Text,
                Foreground = palette.TextPrimary,
                Background = palette.FieldBackground,
                BorderBrush = palette.Green,
                BorderThickness = new Thickness(1),
                FontSize = 13,
                Padding = new Thickness(3, 2, 3, 2),
                VerticalContentAlignment = VerticalAlignment.Center
            };
            var finished = false;
            void Commit()
            {
                if (finished)
                {
                    return;
                }
                finished = true;
                if (!TextValue.IsBlank(editor.Text))
                {
                    _store.Update(item.Id, editor.Text);
                }
                Refresh();
            }
            editor.KeyDown += (_, eventArgs) =>
            {
                if (eventArgs.Key == Key.Enter)
                {
                    Commit();
                    eventArgs.Handled = true;
                }
                else if (eventArgs.Key == Key.Escape)
                {
                    finished = true;
                    Refresh();
                    eventArgs.Handled = true;
                }
            };
            editor.LostKeyboardFocus += (_, _) => Commit();
            editorHost.Children.Add(editor);
            Dispatcher.BeginInvoke(() =>
            {
                editor.Focus();
                editor.CaretIndex = editor.Text.Length;
            });
        }

        ShowText();

        var pin = IconButton(
            item.IsPinned ? IconGlyphs.Lock : IconGlyphs.Pin,
            item.IsPinned ? text.UnpinItem : text.PinItem,
            palette,
            (_, _) =>
            {
                _store.TogglePin(item.Id);
                Refresh();
            },
            palette.Green);
        pin.Opacity = item.IsPinned ? 1 : 0;
        pin.IsHitTestVisible = item.IsPinned;
        Grid.SetColumn(pin, 2);
        grid.Children.Add(pin);

        Button? grip = null;
        if (!item.IsPinned)
        {
            grip = GripButton(text.DragToReorder, palette, item.Id);
            grip.Opacity = 0;
            grip.IsHitTestVisible = false;
            Grid.SetColumn(grip, 3);
            grid.Children.Add(grip);
        }

        row.MouseEnter += (_, _) =>
        {
            if (!item.IsPinned)
            {
                row.Background = palette.HoverBackground;
                pin.Opacity = 1;
                pin.IsHitTestVisible = true;
                if (grip is not null)
                {
                    grip.Opacity = 1;
                    grip.IsHitTestVisible = true;
                }
            }
        };
        row.MouseLeave += (_, _) =>
        {
            row.Background = item.IsPinned ? palette.PinnedBackground : Brushes.Transparent;
            if (!item.IsPinned)
            {
                pin.Opacity = 0;
                pin.IsHitTestVisible = false;
                if (grip is not null)
                {
                    grip.Opacity = 0;
                    grip.IsHitTestVisible = false;
                }
            }
        };

        var menu = new ContextMenu();
        var edit = new MenuItem { Header = text.Edit };
        edit.Click += (_, _) => ShowEditor();
        var delete = new MenuItem { Header = text.Delete };
        delete.Click += (_, _) => DeleteItem(item, text);
        menu.Items.Add(edit);
        menu.Items.Add(delete);
        row.ContextMenu = menu;

        row.DragOver += (_, eventArgs) =>
        {
            if (TryGetDraggedId(eventArgs.Data, out var draggedId) && draggedId != item.Id && !item.IsPinned)
            {
                row.BorderBrush = palette.Green;
                row.BorderThickness = new Thickness(0, 2, 0, 0.5);
                eventArgs.Effects = DragDropEffects.Move;
            }
            else
            {
                eventArgs.Effects = DragDropEffects.None;
            }
            eventArgs.Handled = true;
        };
        row.DragLeave += (_, _) => row.BorderThickness = new Thickness(0, 0, 0, 0.5);
        row.Drop += (_, eventArgs) =>
        {
            if (TryGetDraggedId(eventArgs.Data, out var draggedId))
            {
                _store.Move(draggedId, item.Id);
                Refresh();
            }
            eventArgs.Handled = true;
        };

        row.Child = grid;
        return row;
    }

    private void RefreshCompletedRows(ThemePalette palette, TextCatalog text)
    {
        if (_completedStack is null)
        {
            return;
        }
        _completedStack.Children.Clear();
        var filtered = FilteredCompleted();
        if (filtered.Count == 0)
        {
            _completedStack.Children.Add(EmptyState(
                TextValue.IsBlank(_searchQuery) ? text.NoCompleted : text.NoResults,
                palette));
        }
        else
        {
            foreach (var item in filtered)
            {
                _completedStack.Children.Add(CreateCompletedRow(item, palette, text));
            }
        }

        if (_deleteCompletedButton is not null)
        {
            _deleteCompletedButton.Content = TextValue.IsBlank(_searchQuery)
                ? text.DeleteAll
                : text.DeleteResults;
            AutomationProperties.SetName(_deleteCompletedButton, _deleteCompletedButton.Content.ToString() ?? text.DeleteAll);
            _deleteCompletedButton.Visibility = _store.Completed.Count == 0
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }

    private Border CreateCompletedRow(TaskItem item, ThemePalette palette, TextCatalog text)
    {
        var row = new Border
        {
            BorderBrush = palette.Divider,
            BorderThickness = new Thickness(0, 0, 0, 0.5),
            Background = Brushes.Transparent,
            MinHeight = 46,
            Padding = new Thickness(10, 11, 10, 11)
        };
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });

        var completion = CompletionButton(completed: true, palette, text.RestoreItem);
        completion.Click += (_, _) =>
        {
            _store.Restore(item.Id);
            Refresh();
        };
        grid.Children.Add(completion);

        var labels = new StackPanel { Margin = new Thickness(10, 0, 6, 0) };
        labels.Children.Add(new TextBlock
        {
            Text = item.Text,
            Foreground = palette.TextSecondary,
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap,
            TextDecorations = null
        });
        labels.Children.Add(new TextBlock
        {
            Text = (item.CompletedAt ?? item.CreatedAt).LocalDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Foreground = palette.TextTertiary,
            FontSize = 10,
            Margin = new Thickness(0, 2, 0, 0)
        });
        Grid.SetColumn(labels, 1);
        grid.Children.Add(labels);

        var delete = IconButton(IconGlyphs.Delete, text.Delete, palette, (_, _) => DeleteItem(item, text), palette.Destructive);
        delete.Opacity = 0;
        delete.IsHitTestVisible = false;
        Grid.SetColumn(delete, 2);
        grid.Children.Add(delete);

        row.MouseEnter += (_, _) =>
        {
            row.Background = palette.HoverBackground;
            delete.Opacity = 1;
            delete.IsHitTestVisible = true;
        };
        row.MouseLeave += (_, _) =>
        {
            row.Background = Brushes.Transparent;
            delete.Opacity = 0;
            delete.IsHitTestVisible = false;
        };
        row.Child = grid;
        return row;
    }

    private static Border BuildInputField(ThemePalette palette, string placeholder, out TextBox input)
    {
        var border = new Border
        {
            Background = palette.FieldBackground,
            BorderBrush = palette.Divider,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(3),
            Margin = new Thickness(12, 11, 12, 11),
            Height = 42
        };
        var field = new Grid();
        var hint = new TextBlock
        {
            Text = placeholder,
            Foreground = palette.TextTertiary,
            FontSize = 14,
            Margin = new Thickness(13, 0, 42, 0),
            VerticalAlignment = VerticalAlignment.Center,
            IsHitTestVisible = false
        };
        input = new TextBox
        {
            Foreground = palette.TextPrimary,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            FontSize = 14,
            Margin = new Thickness(10, 0, 38, 0),
            VerticalContentAlignment = VerticalAlignment.Center
        };
        var capturedInput = input;
        AutomationProperties.SetName(capturedInput, placeholder);
        capturedInput.TextChanged += (_, _) => hint.Visibility = capturedInput.Text.Length == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
        field.Children.Add(hint);
        field.Children.Add(capturedInput);
        border.Child = field;
        return border;
    }

    private Button GripButton(string tooltip, ThemePalette palette, Guid itemId)
    {
        var dots = new UniformGrid { Rows = 3, Columns = 2, Width = 10, Height = 15 };
        for (var index = 0; index < 6; index++)
        {
            dots.Children.Add(new System.Windows.Shapes.Ellipse
            {
                Width = 2.5,
                Height = 2.5,
                Fill = palette.TextTertiary,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
        }
        var button = new Button
        {
            Content = dots,
            ToolTip = tooltip,
            Width = 24,
            Height = 24,
            Padding = new Thickness(0),
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            Cursor = Cursors.Hand,
            Template = FlatButtonTemplate(palette.HoverBackground, new CornerRadius(5))
        };
        AutomationProperties.SetName(button, tooltip);
        button.PreviewMouseMove += (_, eventArgs) =>
        {
            if (_dragInProgress || eventArgs.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            _dragInProgress = true;
            try
            {
                var data = new DataObject(ReorderFormat, itemId.ToString());
                _ = DragDrop.DoDragDrop(button, data, DragDropEffects.Move);
            }
            finally
            {
                _dragInProgress = false;
            }
        };
        return button;
    }

    private static Button CompletionButton(bool completed, ThemePalette palette, string accessibleName)
    {
        var button = new Button
        {
            Content = CompletionMark(completed, palette),
            Width = 24,
            Height = 24,
            Padding = new Thickness(0),
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            Cursor = Cursors.Arrow,
            Template = FlatButtonTemplate(palette.HoverBackground, new CornerRadius(5))
        };
        AutomationProperties.SetName(button, accessibleName);
        return button;
    }

    private static Border CompletionMark(bool completed, ThemePalette palette)
    {
        return new Border
        {
            Width = 16,
            Height = 16,
            CornerRadius = new CornerRadius(2),
            BorderBrush = completed ? palette.Green : palette.TextTertiary,
            BorderThickness = new Thickness(1.5),
            Background = completed ? palette.Green : Brushes.Transparent,
            Child = completed
                ? new TextBlock
                {
                    Text = "✓",
                    Foreground = Brushes.White,
                    FontSize = 11,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
                : null
        };
    }

    private static Button IconButton(
        object glyph,
        string tooltip,
        ThemePalette palette,
        RoutedEventHandler click,
        Brush? foreground = null)
    {
        var button = new Button
        {
            Content = glyph,
            ToolTip = tooltip,
            Width = 28,
            Height = 28,
            Margin = new Thickness(1, 0, 1, 0),
            Padding = new Thickness(0),
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            Foreground = foreground ?? palette.TextSecondary,
            FontFamily = IconGlyphs.FontFamily,
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            Cursor = Cursors.Arrow,
            Template = FlatButtonTemplate(palette.HoverBackground, new CornerRadius(5))
        };
        AutomationProperties.SetName(button, tooltip);
        button.Click += click;
        return button;
    }

    private static Canvas CreateVerticalPinIcon(ThemePalette palette, bool filled)
    {
        var canvas = new Canvas { Width = 14, Height = 14 };
        var brush = filled ? palette.Green : palette.TextSecondary;
        canvas.Children.Add(new System.Windows.Shapes.Path
        {
            Data = Geometry.Parse("M 4,2 L 10,2 L 9,6 L 11,8 L 8,8 L 7,13 L 6,8 L 3,8 L 5,6 Z"),
            Fill = filled ? brush : Brushes.Transparent,
            Stroke = brush,
            StrokeThickness = 1.15,
            StrokeLineJoin = PenLineJoin.Round
        });
        return canvas;
    }

    private static ControlTemplate FlatButtonTemplate(Brush hoverBackground, CornerRadius cornerRadius)
    {
        var border = new FrameworkElementFactory(typeof(Border));
        border.Name = "Root";
        border.SetValue(Border.BackgroundProperty, new System.Windows.Data.Binding("Background")
        {
            RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent)
        });
        border.SetValue(Border.CornerRadiusProperty, cornerRadius);

        var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
        presenter.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
        presenter.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
        presenter.SetValue(ContentPresenter.ContentProperty, new System.Windows.Data.Binding("Content")
        {
            RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent)
        });
        border.AppendChild(presenter);

        var template = new ControlTemplate(typeof(Button)) { VisualTree = border };
        var hover = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
        hover.Setters.Add(new Setter(Border.BackgroundProperty, hoverBackground, "Root"));
        template.Triggers.Add(hover);
        var pressed = new Trigger { Property = ButtonBase.IsPressedProperty, Value = true };
        pressed.Setters.Add(new Setter(UIElement.OpacityProperty, 0.72, "Root"));
        template.Triggers.Add(pressed);
        return template;
    }

    private static TextBlock EmptyState(string message, ThemePalette palette)
    {
        return new TextBlock
        {
            Text = message,
            Foreground = palette.TextTertiary,
            FontSize = 13,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(12, 60, 12, 12)
        };
    }

    private List<TaskItem> FilteredCompleted()
    {
        if (TextValue.IsBlank(_searchQuery))
        {
            return _store.Completed.ToList();
        }
        return _store.Completed
            .Where(item => ContainsSearch(item.Text, _searchQuery.Trim()))
            .ToList();
    }

    private static bool ContainsSearch(string text, string query)
    {
#if NETFRAMEWORK
        return text.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0;
#else
        return text.Contains(query, StringComparison.CurrentCultureIgnoreCase);
#endif
    }

    private void DeleteItem(TaskItem item, TextCatalog text)
    {
        var confirmed = _coordinator.ShowConfirmation(
            text.DeleteItemTitle,
            text.DeleteItemMessage,
            text.ConfirmDelete,
            text.Cancel,
            destructive: true);
        if (confirmed)
        {
            _store.Delete(item.Id);
            Refresh();
        }
    }

    private void DeleteCompletedScope(TextCatalog text)
    {
        var selected = FilteredCompleted();
        if (selected.Count == 0)
        {
            return;
        }
        var searching = !TextValue.IsBlank(_searchQuery);
        var confirmed = _coordinator.ShowConfirmation(
            searching ? text.DeleteResultsTitle : text.DeleteAllTitle,
            text.DeleteManyMessage(selected.Count),
            searching ? text.DeleteResults : text.DeleteAll,
            text.Cancel,
            destructive: true);
        if (!confirmed)
        {
            return;
        }
        _store.DeleteCompleted(selected.Select(item => item.Id));
        _searchQuery = string.Empty;
        Refresh();
    }

    private static bool TryGetDraggedId(IDataObject data, out Guid id)
    {
        id = Guid.Empty;
        if (!data.GetDataPresent(ReorderFormat) || data.GetData(ReorderFormat) is not string value)
        {
            return false;
        }
        try
        {
            id = new Guid(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private void HandleClosing(object? sender, CancelEventArgs eventArgs)
    {
        if (_coordinator.IsShuttingDown)
        {
            return;
        }
        eventArgs.Cancel = true;
        _coordinator.ClosePanelFromButton();
    }
}
