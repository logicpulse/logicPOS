using Avalonia.Controls;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;

namespace LogicPOS.App.Views;

public partial class FirstUserSetupWindow : Window
{
    private readonly IFirstUserSetupWizard _wizard;

    public bool Completed { get; private set; }

    public FirstUserSetupWindow()
        : this(AppComposition.Services?.GetService(typeof(IFirstUserSetupWizard)) as IFirstUserSetupWizard
               ?? throw new InvalidOperationException("IFirstUserSetupWizard is not registered."))
    {
    }

    public FirstUserSetupWindow(IFirstUserSetupWizard wizard)
    {
        _wizard = wizard;
        InitializeComponent();
        ListingNotice.BindToast(LblStatus, this);
        Opened += async (_, _) => await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var draft = await _wizard.LoadAsync();
        if (string.IsNullOrWhiteSpace(draft.BlockedReason) == false)
        {
            LblStatus.Text = draft.BlockedReason;
            BtnCreate.IsEnabled = false;
            return;
        }

        CmbProfile.Items.Clear();
        foreach (var profile in draft.Profiles)
            CmbProfile.Items.Add(profile);

        CmbProfile.SelectedItem = draft.Profiles.FirstOrDefault(profile => profile.Id == draft.PreferredProfileId)
            ?? draft.Profiles.FirstOrDefault();
        TxtName.Text = "Administrador";
        TxtLogin.Text = "admin";
    }

    private async void OnCreateClick(object? sender, RoutedEventArgs e)
    {
        BtnCreate.IsEnabled = false;
        LblStatus.Text = string.Empty;

        if (CmbProfile.SelectedItem is not FirstUserProfileChoice profile)
        {
            LblStatus.Text = "Seleccione um perfil.";
            BtnCreate.IsEnabled = true;
            return;
        }

        var result = await _wizard.CreateAsync(new FirstUserSetupRequest
        {
            ProfileId = profile.Id,
            Name = TxtName.Text ?? string.Empty,
            Login = TxtLogin.Text ?? string.Empty,
            AccessPin = TxtPin.Text ?? string.Empty,
            AccessPinConfirm = TxtPinConfirm.Text ?? string.Empty
        });

        if (result.Ok == false)
        {
            LblStatus.Text = result.Message;
            BtnCreate.IsEnabled = true;
            return;
        }

        Completed = true;
        Close();
    }
}
