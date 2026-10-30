using Avalonia.Controls;
using Avalonia.Media;

namespace LogicPOS.App.Views;

public partial class ListingBusy : UserControl
{
    private static readonly IBrush DarkLabel = new SolidColorBrush(Color.Parse("#323537"));
    private static readonly IBrush LightLabel = new SolidColorBrush(Colors.White);

    public ListingBusy()
    {
        InitializeComponent();
        // Listings / popups: spinner only — no white/dark scrim over the grid.
        UseTransparentScrim();
    }

    /// <summary>
    /// Dark scrim (#99000000) + white label. Prefer transparent for listings/popups.
    /// </summary>
    public void UseDarkScrim()
    {
        Scrim.Background = new SolidColorBrush(Color.Parse("#99000000"));
        Label.Foreground = LightLabel;
    }

    /// <summary>
    /// Spinner + label only; transparent host (no overlay scrim).
    /// </summary>
    public void UseTransparentScrim()
    {
        Scrim.Background = Brushes.Transparent;
        Label.Foreground = DarkLabel;
    }

    /// <summary>
    /// Login sits on a dark wallpaper — keep label readable without a scrim.
    /// </summary>
    public void UseLoginBusy()
    {
        Scrim.Background = Brushes.Transparent;
        Label.Foreground = LightLabel;
    }

    public void Show(string? message = null)
    {
        Label.Text = string.IsNullOrWhiteSpace(message) ? "A carregar..." : message;
        IsVisible = true;
    }

    public void Hide()
    {
        IsVisible = false;
        Label.Text = "A carregar...";
    }
}
