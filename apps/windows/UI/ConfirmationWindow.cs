using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LiteTick.Windows.UI;

internal sealed class ConfirmationWindow : Window
{
    private ConfirmationWindow(
        string title,
        string message,
        string confirmLabel,
        string cancelLabel,
        ThemePalette palette,
        bool destructive)
    {
        Title = title;
        Width = 430;
        Height = 222;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        AllowsTransparency = true;
        Background = Brushes.Transparent;

        var shell = new Border
        {
            Background = palette.WindowBackground,
            BorderBrush = palette.Divider,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(24, 22, 24, 18)
        };

        var layout = new Grid();
        layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        layout.Children.Add(new TextBlock
        {
            Text = title,
            Foreground = palette.TextPrimary,
            FontSize = 16,
            FontWeight = FontWeights.SemiBold,
            TextWrapping = TextWrapping.Wrap
        });

        var body = new TextBlock
        {
            Text = message,
            Foreground = palette.TextSecondary,
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 12, 0, 18)
        };
        Grid.SetRow(body, 1);
        layout.Children.Add(body);

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        var cancel = CreateButton(cancelLabel, palette.TextPrimary, palette.FieldBackground, palette);
        cancel.IsDefault = false;
        cancel.IsCancel = true;
        cancel.Click += (_, _) => DialogResult = false;
        actions.Children.Add(cancel);

        var confirmForeground = destructive ? Brushes.White : Brushes.White;
        var confirmBackground = destructive ? palette.Destructive : palette.Green;
        var confirm = CreateButton(confirmLabel, confirmForeground, confirmBackground, palette);
        confirm.IsDefault = true;
        confirm.Margin = new Thickness(8, 0, 0, 0);
        confirm.Click += (_, _) => DialogResult = true;
        actions.Children.Add(confirm);
        Grid.SetRow(actions, 2);
        layout.Children.Add(actions);

        shell.Child = layout;
        Content = shell;
    }

    internal static bool Show(
        Window? owner,
        string title,
        string message,
        string confirmLabel,
        string cancelLabel,
        ThemePalette palette,
        bool destructive = false)
    {
        var dialog = new ConfirmationWindow(title, message, confirmLabel, cancelLabel, palette, destructive);
        if (owner is not null && owner.IsVisible)
        {
            dialog.Owner = owner;
        }
        else
        {
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        return dialog.ShowDialog() == true;
    }

    private static Button CreateButton(string label, Brush foreground, Brush background, ThemePalette palette)
    {
        var button = new Button
        {
            Content = label,
            MinWidth = 76,
            Height = 32,
            Padding = new Thickness(14, 0, 14, 0),
            Foreground = foreground,
            Background = background,
            BorderBrush = palette.Divider,
            BorderThickness = new Thickness(1),
            FontSize = 13,
            FontWeight = FontWeights.Medium,
            Template = ButtonTemplate()
        };
        AutomationProperties.SetName(button, label);
        return button;
    }

    private static ControlTemplate ButtonTemplate()
    {
        var border = new FrameworkElementFactory(typeof(Border));
        border.Name = "Root";
        border.SetValue(Border.BackgroundProperty, new System.Windows.Data.Binding("Background")
        {
            RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent)
        });
        border.SetValue(Border.BorderBrushProperty, new System.Windows.Data.Binding("BorderBrush")
        {
            RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent)
        });
        border.SetValue(Border.BorderThicknessProperty, new System.Windows.Data.Binding("BorderThickness")
        {
            RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent)
        });
        border.SetValue(Border.CornerRadiusProperty, new CornerRadius(6));

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
        hover.Setters.Add(new Setter(UIElement.OpacityProperty, 0.88, "Root"));
        template.Triggers.Add(hover);
        var pressed = new Trigger { Property = ButtonBase.IsPressedProperty, Value = true };
        pressed.Setters.Add(new Setter(UIElement.OpacityProperty, 0.7, "Root"));
        template.Triggers.Add(pressed);
        return template;
    }
}
