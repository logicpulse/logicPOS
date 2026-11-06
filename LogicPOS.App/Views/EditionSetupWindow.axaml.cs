using Avalonia.Controls;
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
        RbCommunity.IsCheckedChanged += (_, _) => RefreshHostingPanel();
        RbBusiness.IsCheckedChanged += (_, _) => RefreshHostingPanel();
        RbCloud.IsCheckedChanged += (_, _) => RefreshHostingPanel();
        RbRemoteApi.IsCheckedChanged += (_, _) => RefreshApiFields();
        RbLocalServer.IsCheckedChanged += (_, _) => RefreshApiFields();
        RefreshHostingPanel();
    }

    private void RefreshHostingPanel()
    {
        PanelBusinessHosting.IsVisible = RbBusiness.IsChecked == true;
        RefreshApiFields();
    }

    private void RefreshApiFields()
    {
        var remote = RbBusiness.IsChecked == true && RbRemoteApi.IsChecked == true;
        TxtApiAddress.IsVisible = remote;
        TxtClientId.IsVisible = remote;
    }

    private void OnContinueClick(object? sender, RoutedEventArgs e)
    {
        if (RbCloud.IsChecked == true)
        {
            SelectedEdition = ProductEdition.Cloud;
            SelectedHosting = ProductHosting.Cloud;
            ApiAddress = "https://api.logic-pos.com/";
            ClientId = null;
        }
        else if (RbBusiness.IsChecked == true)
        {
            SelectedEdition = ProductEdition.Business;
            if (RbRemoteApi.IsChecked == true)
            {
                SelectedHosting = ProductHosting.RemoteApi;
                ApiAddress = string.IsNullOrWhiteSpace(TxtApiAddress.Text)
                    ? null
                    : TxtApiAddress.Text.Trim();
                if (string.IsNullOrWhiteSpace(ApiAddress))
                {
                    LblStatus.Text = "Indique o endereço da API.";
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