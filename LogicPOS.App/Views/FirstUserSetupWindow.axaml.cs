using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;

namespace LogicPOS.App.Views;

public partial class FirstUserSetupWindow : Window
{
    private static readonly string[] Steps = ["Boas-vindas", "Utilizador", "Perfil", "PIN", "Resumo"];

    private readonly IFirstUserSetupWizard _wizard;
    private readonly List<RadioButton> _profiles = new();
    private int _step;
    private bool _busy;

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
        TouchFields.Attach(this);
        ListingNotice.BindToast(LblStatus, this);
        ShowStep();
        Opened += async (_, _) => await LoadAsync();
    }

    private async Task LoadAsync()
    {
        FirstUserSetupDraft draft;
        try
        {
            draft = await _wizard.LoadAsync();
        }
        catch (Exception exception)
        {
            LblStatus.Text = exception.GetBaseException().Message;
            NextButton.IsEnabled = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(draft.BlockedReason) == false)
        {
            LblStatus.Text = draft.BlockedReason;
            NextButton.IsEnabled = false;
            return;
        }

        ProfileList.Children.Clear();
        _profiles.Clear();
        foreach (var profile in draft.Profiles)
        {
            var option = new RadioButton
            {
                Classes = { "bo_entity_input" },
                GroupName = "FirstUserProfile",
                Content = profile.Label,
                Tag = profile,
                IsChecked = profile.Id == draft.PreferredProfileId
            };
            _profiles.Add(option);
            ProfileList.Children.Add(option);
        }

        TxtName.Text = "Administrador";
        TxtLogin.Text = "admin";
    }

    private FirstUserProfileChoice? SelectedProfile()
        => _profiles.FirstOrDefault(option => option.IsChecked == true)?.Tag as FirstUserProfileChoice;

    private FirstUserSetupRequest BuildRequest() => new()
    {
        ProfileId = SelectedProfile()?.Id ?? Guid.Empty,
        Name = TxtName.Text ?? string.Empty,
        Login = TxtLogin.Text ?? string.Empty,
        Code = TxtCode.Text ?? string.Empty,
        AccessPin = TxtPin.Text ?? string.Empty,
        AccessPinConfirm = TxtPinConfirm.Text ?? string.Empty
    };

    private string? ValidateStep()
    {
        var request = BuildRequest();
        return Steps[_step] switch
        {
            "Utilizador" => request.ValidateIdentity(),
            "Perfil" => SelectedProfile() is null ? "Seleccione um perfil." : null,
            "PIN" => request.ValidatePin(),
            _ => null
        };
    }

    private void ShowStep()
    {
        var name = Steps[_step];
        WelcomeStep.IsVisible = name == "Boas-vindas";
        UserStep.IsVisible = name == "Utilizador";
        ProfileStep.IsVisible = name == "Perfil";
        PinStep.IsVisible = name == "PIN";
        SummaryStep.IsVisible = name == "Resumo";
        BackButton.IsVisible = _step > 0;
        NextButton.IsVisible = _step < Steps.Length - 1;
        BtnCreate.IsVisible = _step == Steps.Length - 1;

        if (name == "Resumo")
        {
            var request = BuildRequest();
            SummaryName.Text = request.Name.Trim();
            SummaryLogin.Text = request.Login.Trim();
            SummaryCode.Text = string.IsNullOrWhiteSpace(request.Code) ? "1" : request.Code.Trim();
            SummaryProfile.Text = SelectedProfile()?.Label ?? string.Empty;
        }

        TabBar.Children.Clear();
        for (var index = 0; index < Steps.Length; index++)
        {
            var tab = new Button
            {
                Classes = { "bo_fiscal_wizard_tab" },
                Content = $"{index + 1}. {Steps[index]}"
            };
            if (index == _step)
            {
                tab.Classes.Add("bo_fiscal_wizard_tab_on");
            }
            else if (index < _step)
            {
                tab.Classes.Add("bo_fiscal_wizard_tab_done");
            }

            var target = index;
            tab.Click += (_, _) =>
            {
                if (_busy == false && target <= _step)
                {
                    _step = target;
                    LblStatus.Text = string.Empty;
                    ShowStep();
                }
            };
            TabBar.Children.Add(tab);
        }
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        if (_busy || _step == 0)
        {
            return;
        }

        _step--;
        LblStatus.Text = string.Empty;
        ShowStep();
    }

    private void OnNextClick(object? sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        var error = ValidateStep();
        if (error is not null)
        {
            LblStatus.Text = error;
            return;
        }

        LblStatus.Text = string.Empty;
        _step++;
        ShowStep();
    }

    private async void OnCreateClick(object? sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        _busy = true;
        BtnCreate.IsEnabled = false;
        BackButton.IsEnabled = false;
        LblStatus.Text = string.Empty;

        try
        {
            var result = await _wizard.CreateAsync(BuildRequest());
            if (result.Ok == false)
            {
                LblStatus.Text = result.Message;
                return;
            }

            Completed = true;
            Close();
        }
        catch (Exception exception)
        {
            LblStatus.Text = exception.GetBaseException().Message;
        }
        finally
        {
            _busy = false;
            BtnCreate.IsEnabled = true;
            BackButton.IsEnabled = true;
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        if (_busy == false)
        {
            Close();
        }
    }

    private void OnTitlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }
}
