using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LogicPOS.Core;

namespace LogicPOS.App.Views;

public partial class EditionSetupWindow : Window
{
    public bool Completed { get; private set; }

    public ProductEdition SelectedEdition { get; private set; } = ProductEdition.Community;

    public ProductHosting SelectedHosting { get; private set; } = ProductHosting.Local;

    public string? ApiAddress { get; private set; }

    public string? ClientId { get; private set; }

    public EditionSetupWindow()
    {
        InitializeComponent();
        RbRemoteApi.IsCheckedChanged += (_, _) => RefreshApiFields();
        RbLocalServer.IsCheckedChanged += (_, _) => RefreshApiFields();
        SelectEdition(ProductEdition.Community, closeImmediately: false);
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Completed = false;
        Close();
    }

    private void OnCommunityCardPressed(object? sender, PointerPressedEventArgs e)
        => SelectEdition(ProductEdition.Community, closeImmediately: false);

    private void OnBusinessCardPressed(object? sender, PointerPressedEventArgs e)
        => SelectEdition(ProductEdition.Business, closeImmediately: false);

    private void OnCloudCardPressed(object? sender, PointerPressedEventArgs e)
        => SelectEdition(ProductEdition.Cloud, closeImmediately: false);

    private void OnCommunityClick(object? sender, RoutedEventArgs e)
        => SelectEdition(ProductEdition.Community, closeImmediately: true);

    private void OnBusinessClick(object? sender, RoutedEventArgs e)
        => SelectEdition(ProductEdition.Business, closeImmediately: false);

    private void OnCloudClick(object? sender, RoutedEventArgs e)
        => SelectEdition(ProductEdition.Cloud, closeImmediately: true);

    private void SelectEdition(ProductEdition edition, bool closeImmediately)
    {
        SelectedEdition = edition;
        LblStatus.Text = string.Empty;

        CardCommunity.Classes.Set("selected-community", edition == ProductEdition.Community);
        CardBusiness.Classes.Set("selected-business", edition == ProductEdition.Business);
        CardCloud.Classes.Set("selected-cloud", edition == ProductEdition.Cloud);

        PanelBusinessHosting.IsVisible = edition == ProductEdition.Business;
        RefreshApiFields();
        RefreshContinueButton(edition);

        if (closeImmediately)
            TryCompleteAndClose();
    }

    private void RefreshContinueButton(ProductEdition edition)
    {
        BtnContinue.Content = edition switch
        {
            ProductEdition.Community => "Começar a usar",
            ProductEdition.Business => "Continuar com Business",
            ProductEdition.Cloud => "Continuar com Cloud",
            _ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(edition), (int)edition, typeof(ProductEdition)),
        };

        BtnContinue.Background = edition switch
        {
            ProductEdition.Community => Avalonia.Media.Brush.Parse("#CBDF96"),
            ProductEdition.Business => Avalonia.Media.Brush.Parse("#3D7EA6"),
            ProductEdition.Cloud => Avalonia.Media.Brush.Parse("#5B7C99"),
            _ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(edition), (int)edition, typeof(ProductEdition)),
        };

        BtnContinue.Foreground = edition == ProductEdition.Community
            ? Avalonia.Media.Brush.Parse("#263238")
            : Avalonia.Media.Brushes.White;
    }

    private void RefreshApiFields()
    {
        var remote = SelectedEdition == ProductEdition.Business && RbRemoteApi.IsChecked == true;
        TxtApiAddress.IsVisible = remote;
        TxtClientId.IsVisible = remote;
    }

    private void OnContinueClick(object? sender, RoutedEventArgs e)
        => TryCompleteAndClose();

    private void TryCompleteAndClose()
    {
        if (SelectedEdition == ProductEdition.Cloud)
        {
            SelectedHosting = ProductHosting.Cloud;
            ApiAddress = "https://api.logic-pos.com/";
            ClientId = null;
        }
        else if (SelectedEdition == ProductEdition.Business)
        {
            if (RbRemoteApi.IsChecked == true)
            {
                SelectedHosting = ProductHosting.RemoteApi;
                ApiAddress = string.IsNullOrWhiteSpace(TxtApiAddress.Text)
                    ? null
                    : TxtApiAddress.Text.Trim();
                if (string.IsNullOrWhiteSpace(ApiAddress))
                {
                    LblStatus.Text = "Indique o endereço da API.";
                    PanelBusinessHosting.IsVisible = true;
                    return;
                }

                ClientId = TxtClientId.Text?.Trim();
            }
            else
            {
                SelectedHosting = ProductHosting.LocalServer;
                ApiAddress = null;
                ClientId = null;
            }
        }
        else
        {
            SelectedEdition = ProductEdition.Community;
            SelectedHosting = ProductHosting.Local;
            ApiAddress = null;
            ClientId = null;
        }

        Completed = true;
        Close();
    }
}
