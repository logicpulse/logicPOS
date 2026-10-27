using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace LogicPOS.App.Views;

/// <summary>
/// Portrait detection and responsive overlay sizing for BO/FO dialogs.
/// Portrait = Height &gt; Width; landscape keeps preferred size clamped to the viewport.
/// </summary>
internal static class PortraitChrome
{
    /// <summary>Margem mínima em todos os lados (nunca edge-to-edge).</summary>
    public const int DefaultMargin = 16;

    /// <summary>
    /// Reserva vertical para o dock FO flutuante (MinHeight ~64 + margens exteriores).
    /// </summary>
    public const int FloatingToolbarReserve = 80;

    public static bool TryUpdate(Control host, ref UiViewport viewport, out bool portrait)
    {
        var next = UiViewport.From(host);
        if (next.Width == viewport.Width && next.Height == viewport.Height && host.Bounds.Width > 0)
        {
            portrait = host.Classes.Contains("bo_portrait");
            return false;
        }

        viewport = next;
        portrait = next.IsPortrait;
        host.Classes.Set("bo_portrait", portrait);
        host.Classes.Set("bo_landscape", portrait == false);
        return true;
    }

    public static Border Card(string title, string? subtitle = null, string? meta = null, object? tag = null)
    {
        var stack = new StackPanel { Classes = { "bo_listing_card_body" } };
        stack.Children.Add(new TextBlock { Classes = { "bo_listing_card_title" }, Text = title ?? string.Empty });
        if (string.IsNullOrWhiteSpace(subtitle) == false)
        {
            stack.Children.Add(new TextBlock { Classes = { "bo_listing_card_subtitle" }, Text = subtitle });
        }

        if (string.IsNullOrWhiteSpace(meta) == false)
        {
            stack.Children.Add(new TextBlock { Classes = { "bo_listing_card_meta" }, Text = meta });
        }

        return new Border
        {
            Classes = { "bo_listing_card" },
            Child = stack,
            Tag = tag
        };
    }

    public static FuncDataTemplate<T> CardTemplate<T>(Func<T, Border> build) where T : class
        => new((item, _) => build(item), true);

    public static Button SheetButton(string label, EventHandler<Avalonia.Interactivity.RoutedEventArgs> click)
    {
        var button = new Button
        {
            Classes = { "bo_mobile_sheet_button" },
            Content = label,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        button.Click += click;
        return button;
    }

    /// <summary>
    /// Preferred size clamped to the host with margins (portrait fills available area, never edge-to-edge).
    /// </summary>
    public static void ApplyOverlayFrame(Control frame, bool portrait, double landscapeWidth, double landscapeHeight)
    {
        var host = TopLevel.GetTopLevel(frame) as Control ?? frame;
        FitOverlay(
            frame,
            host,
            landscapeWidth,
            landscapeHeight,
            fullBleedPortrait: portrait);
    }

    /// <summary>
    /// Sizes an overlay to the available host area with consistent margins.
    /// Portrait + <paramref name="fullBleedPortrait"/>: expand to available area (still with margins — never edge-to-edge).
    /// Landscape / non-bleed: clamp preferred size. Optional <paramref name="bottomReserve"/> clears FO dock.
    /// </summary>
    public static void FitOverlay(
        Control frame,
        Control host,
        double preferredWidth,
        double preferredHeight,
        int margin = DefaultMargin,
        bool fullBleedPortrait = true,
        bool lockHeight = true,
        int bottomReserve = 0)
    {
        var (width, height, portrait) = MeasureHost(host);
        margin = Math.Max(DefaultMargin, margin);
        var bottom = margin + Math.Max(0, bottomReserve);
        var availableW = Math.Max(200, width - (margin * 2));
        var availableH = Math.Max(200, height - margin - bottom);

        // "fullBleed" histórico = ocupar a área disponível; mantém margens (nunca colado às bordas).
        var fill = fullBleedPortrait && portrait;
        frame.Classes.Set("bo_overlay_fullbleed", fill);
        frame.Classes.Set("bo_overlay_fitted", true);

        frame.HorizontalAlignment = HorizontalAlignment.Center;
        frame.VerticalAlignment = VerticalAlignment.Center;
        frame.Margin = new Avalonia.Thickness(margin, margin, margin, bottom);

        double w;
        double h;
        if (fill)
        {
            w = availableW;
            h = availableH;
        }
        else
        {
            w = Math.Min(preferredWidth, availableW);
            h = Math.Min(preferredHeight, availableH);
        }

        frame.Width = w;
        frame.ClearValue(Layoutable.MaxWidthProperty);

        if (lockHeight || fill)
        {
            frame.Height = h;
            frame.ClearValue(Layoutable.MaxHeightProperty);
        }
        else
        {
            frame.ClearValue(Layoutable.HeightProperty);
            frame.MaxHeight = h;
        }
    }

    private static (int Width, int Height, bool Portrait) MeasureHost(Control host)
    {
        var bounds = host.Bounds;
        if (bounds.Width >= 280 && bounds.Height >= 280)
        {
            var w = Math.Max(1, (int)Math.Round(bounds.Width));
            var h = Math.Max(1, (int)Math.Round(bounds.Height));
            return (w, h, h > w);
        }

        var viewport = UiViewport.From(host);
        return (viewport.Width, viewport.Height, viewport.IsPortrait);
    }
}
