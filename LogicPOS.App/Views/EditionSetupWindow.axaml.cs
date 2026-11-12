using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LogicPOS.Core;

namespace LogicPOS.App.Views;

public partial class EditionSetupWindow : Window
{
    public const string PaymentUrl = "https://logic-pos.com/";

    public bool Completed { get; private set; }

    public ProductEdition SelectedEdition { get; private set; } = ProductEdition.Community;

    public ProductHosting SelectedHosting { get; private set; } = ProductHosting.Local;

    public string? ApiAddress { get; private set; }

    public string? ClientId { get; private set; }

    public string? SoftwareKey { get; private set; }

    private bool _busy;

    public EditionSetupWindow()
    {
        InitializeComponent();
        TouchFields.Attach(this);
        ListingNotice.BindToast(LblStatus, this, skipProgress: true);
        RbRemoteApi.IsCheckedChanged += (_, _) => RefreshApiFields();
        RbLocalServer.IsCheckedChanged += (_, _) => RefreshApiFields();
        SelectEdition(ProductEdition.Community, closeImmediately: false);
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
        => SelectEdition(ProductEdition.Cloud, closeImmediately: false);

    private void OnPayBusinessClick(object? sender, RoutedEventArgs e)
        => OpenPaymentPage();

    private void OnPayCloudClick(object? sender, RoutedEventArgs e)
        => OpenPaymentPage();

    private static void OpenPaymentPage()
    {
        try
        {
            Process.Start(new ProcessStartInfo(PaymentUrl) { UseShellExecute = true });
        }
        catch
        {
            // Browser may be unavailable; status text covers the URL.
        }
    }

    private void SelectEdition(ProductEdition edition, bool closeImmediately)
    {
        SelectedEdition = edition;
        LblStatus.Text = string.Empty;

        CardCommunity.Classes.Set("selected", edition == ProductEdition.Community);
        CardBusiness.Classes.Set("selected", edition == ProductEdition.Business);
        CardCloud.Classes.Set("selected", edition == ProductEdition.Cloud);

        PanelBusinessHosting.IsVisible = edition == ProductEdition.Business;
        PanelCloudClient.IsVisible = edition == ProductEdition.Cloud;
        RefreshApiFields();
        RefreshContinueButton(edition);

        if (edition == ProductEdition.Business)
            TxtSoftwareKey.Focus();
        else if (edition == ProductEdition.Cloud)
            TxtCloudClientId.Focus();

        if (closeImmediately)
            _ = TryCompleteAndCloseAsync();
    }

    private void RefreshContinueButton(ProductEdition edition)
    {
        ContinueText.Text = edition switch
        {
            ProductEdition.Community => "Começar a usar",
            ProductEdition.Business => "Activar Business",
            ProductEdition.Cloud => "Continuar com Cloud",
            _ => throw new System.ComponentModel.InvalidEnumArgumentException(
                nameof(edition), (int)edition, typeof(ProductEdition)),
        };
    }

    private void RefreshApiFields()
    {
        var remote = SelectedEdition == ProductEdition.Business && RbRemoteApi.IsChecked == true;
        TxtApiAddress.IsVisible = remote;
        TxtClientId.IsVisible = remote;
    }

    private void OnContinueClick(object? sender, RoutedEventArgs e)
        => _ = TryCompleteAndCloseAsync();

    private async Task TryCompleteAndCloseAsync()
    {
        if (_busy)
            return;

        _busy = true;
        BtnContinue.IsEnabled = false;
        LblStatus.Text = string.Empty;

        try
        {
            if (SelectedEdition == ProductEdition.Cloud)
            {
                var cloudClientId = TxtCloudClientId.Text?.Trim();
                if (string.IsNullOrWhiteSpace(cloudClientId))
                {
                    LblStatus.Text = "Indique o Client ID da conta Cloud.";
                    PanelCloudClient.IsVisible = true;
                    TxtCloudClientId.Focus();
                    return;
                }

                LblStatus.Text = "A validar a conta Cloud…";
                var (ok, error) = await CloudClientIdProbe.ValidateAsync(cloudClientId).ConfigureAwait(true);
                if (ok == false)
                {
                    LblStatus.Text = error ?? "Conta Cloud inválida.";
                    PanelCloudClient.IsVisible = true;
                    TxtCloudClientId.Focus();
                    return;
                }

                SelectedHosting = ProductHosting.Cloud;
                ApiAddress = CloudClientIdProbe.ProductionApiBase;
                ClientId = cloudClientId;
                SoftwareKey = null;
            }
            else if (SelectedEdition == ProductEdition.Business)
            {
                var key = TxtSoftwareKey.Text?.Trim();
                if (string.IsNullOrWhiteSpace(key))
                {
                    LblStatus.Text = "Introduza a chave de software para activar o Business.";
                    PanelBusinessHosting.IsVisible = true;
                    TxtSoftwareKey.Focus();
                    return;
                }

                SoftwareKey = key;

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
                SoftwareKey = null;
            }

            Completed = true;
            Close();
        }
        finally
        {
            _busy = false;
            BtnContinue.IsEnabled = true;
        }
    }
}
