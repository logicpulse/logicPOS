using System.Reflection;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using LogicPOS.Core;
using LogicPOS.Core.Fiscal;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App;

/// <summary>
/// Top-of-window notice for the public open-source builds (Desktop / Android).
/// Hidden on LogicPulse Host (certified) entry points.
/// </summary>
internal static class DevEditionBanner
{
    private const string BannerTag = "dev_edition_banner";

    private const string Message =
        "Versão de desenvolvimento — não utilize para emitir documentos fiscais verdadeiros.";

    public static void Attach(Window window)
    {
        if (ShouldShow() == false)
        {
            return;
        }

        if (window.Content is not null)
        {
            EnsureOnWindow(window);
            return;
        }

        void OnOpened(object? sender, EventArgs e)
        {
            window.Opened -= OnOpened;
            EnsureOnWindow(window);
        }

        window.Opened += OnOpened;
    }

    public static void Attach(Panel host)
    {
        if (ShouldShow() == false)
        {
            return;
        }

        EnsureOnPanel(host);
    }

    /// <summary>Overlay on a UserControl root (e.g. LoginWindow hosted without a Window).</summary>
    public static void Attach(ContentControl control)
    {
        if (ShouldShow() == false)
        {
            return;
        }

        if (control is Window window)
        {
            EnsureOnWindow(window);
            return;
        }

        if (control.Content is Panel panel)
        {
            EnsureOnPanel(panel);
            return;
        }

        if (control.Content is not Control original)
        {
            return;
        }

        var root = new Panel();
        control.Content = null;
        root.Children.Add(original);
        root.Children.Add(CreateBanner());
        control.Content = root;
    }

    private static bool ShouldShow()
    {
        var entry = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
        if (entry.Contains("Host", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (entry.Contains("Desktop", StringComparison.OrdinalIgnoreCase)
            || entry.Contains("Android", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var fiscal = AppComposition.Services?.GetService<IFiscalModule>();
        return fiscal is null || fiscal.IsAvailable == false;
    }

    private static void EnsureOnWindow(Window window)
    {
        if (window.Content is Panel panel)
        {
            EnsureOnPanel(panel);
            return;
        }

        if (window.Content is not Control original)
        {
            return;
        }

        var root = new Panel();
        window.Content = null;
        root.Children.Add(original);
        root.Children.Add(CreateBanner());
        window.Content = root;
    }

    private static void EnsureOnPanel(Panel panel)
    {
        foreach (var child in panel.Children)
        {
            if (child is Border border && Equals(border.Tag, BannerTag))
            {
                return;
            }
        }

        panel.Children.Add(CreateBanner());
    }

    private static Border CreateBanner()
        => new()
        {
            Tag = BannerTag,
            Classes = { "dev_edition_banner" },
            IsHitTestVisible = false,
            Child = new TextBlock
            {
                Text = Message,
                Classes = { "dev_edition_banner_text" },
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            }
        };
}
