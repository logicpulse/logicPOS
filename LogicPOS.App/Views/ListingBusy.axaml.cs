using Avalonia.Controls;

namespace LogicPOS.App.Views;

public partial class ListingBusy : UserControl
{
    public ListingBusy()
    {
        InitializeComponent();
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
