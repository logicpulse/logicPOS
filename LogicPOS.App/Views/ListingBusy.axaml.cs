using Avalonia.Controls;
using Avalonia.Media;

namespace LogicPOS.App.Views;

public partial class ListingBusy : UserControl
{
    public ListingBusy()
    {
        InitializeComponent();
        // Listings / popups: card only — no white/dark scrim over the grid.
        UseTransparentScrim();
    }

    /// <summary>
    /// Dark scrim (#99000000) behind the loading card. Prefer transparent for listings/popups.
    /// </summary>
    public void UseDarkScrim()
    {
        Scrim.Background = new SolidColorBrush(Color.Parse("#99000000"));
    }

    /// <summary>
    /// Loading card only; transparent host (no overlay scrim).
    /// </summary>
    public void UseTransparentScrim()
    {
        Scrim.Background = Brushes.Transparent;
    }

    /// <summary>
    /// Login sits on a dark wallpaper; the white card keeps the label readable without a scrim.
    /// </summary>
    public void UseLoginBusy()
    {
        Scrim.Background = Brushes.Transparent;
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
