using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LiteTick.Windows.UI;

internal sealed class AboutWindow : Window
{
    internal AboutWindow(TextCatalog text, ThemePalette palette)
    {
        Title = text.About;
        Width = 390;
        Height = 390;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Topmost = true;
        Background = palette.WindowBackground;
        Foreground = palette.TextPrimary;

        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), inherit: false)
            .OfType<AssemblyInformationalVersionAttribute>()
            .FirstOrDefault()?.InformationalVersion
            ?? "0.1.0+build.1.development";

        var root = new StackPanel
        {
            Margin = new Thickness(28),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        root.Children.Add(CreateLogo(palette));
        root.Children.Add(new TextBlock
        {
            Text = text.ProductName,
            FontSize = 22,
            FontWeight = FontWeights.SemiBold,
            Foreground = palette.TextPrimary,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 12, 0, 4)
        });
        if (text.IsChinese)
        {
            root.Children.Add(new TextBlock
            {
                Text = "LiteTick",
                FontSize = 14,
                FontWeight = FontWeights.Medium,
                Foreground = palette.TextSecondary,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, -1, 0, 4)
            });
        }
        root.Children.Add(new TextBlock
        {
            Text = text.Description,
            FontSize = 13,
            Foreground = palette.TextSecondary,
            HorizontalAlignment = HorizontalAlignment.Center
        });
        root.Children.Add(new TextBlock
        {
            Text = $"Windows · {version}",
            FontSize = 12,
            Foreground = palette.TextTertiary,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 8, 0, 4)
        });
        root.Children.Add(new TextBlock
        {
            Text = text.LocalOnly,
            FontSize = 12,
            Foreground = palette.TextSecondary,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 18)
        });

        var links = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center };
        links.Inlines.Add(CreateLink(text.Repository, "https://github.com/victor-zhang-2026/LiteTick"));
        links.Inlines.Add(new Run("   ·   "));
        links.Inlines.Add(CreateLink(text.Privacy, "https://github.com/victor-zhang-2026/LiteTick/blob/main/docs/privacy.md"));
        links.Inlines.Add(new Run("   ·   "));
        links.Inlines.Add(CreateLink(text.License, "https://github.com/victor-zhang-2026/LiteTick/blob/main/LICENSE"));
        root.Children.Add(links);
        Content = root;
    }

    private static Grid CreateLogo(ThemePalette palette)
    {
        var logo = new Grid { Width = 88, Height = 88 };
        logo.Children.Add(new Border
        {
            Background = palette.Green,
            CornerRadius = new CornerRadius(19)
        });
        logo.Children.Add(new System.Windows.Shapes.Path
        {
            Data = Geometry.Parse("M 25,45 L 38,58 L 64,31"),
            Stroke = Brushes.White,
            StrokeThickness = 7,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            StrokeLineJoin = PenLineJoin.Round
        });
        return logo;
    }

    private static Hyperlink CreateLink(string label, string uri)
    {
        var link = new Hyperlink(new Run(label)) { NavigateUri = new Uri(uri) };
        link.RequestNavigate += (_, eventArgs) =>
        {
            Process.Start(new ProcessStartInfo(eventArgs.Uri.AbsoluteUri) { UseShellExecute = true });
            eventArgs.Handled = true;
        };
        return link;
    }
}
