using Avalonia.Controls;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class FrontOfficeStartupWizard : UserControl
{
    private static readonly string[] Steps = ["Dia", "Sessão"];

    private int _step;
    private bool _busy;
    private bool _dayOpen;
    private bool _terminalOpen;
    private bool _sessionOpened;

    public FrontOfficeStartupWizard()
    {
        InitializeComponent();
    }

    public event EventHandler<FrontOfficeStartupResult>? Finished;

    /// <summary>
    /// Loads work-session state and starts on the first incomplete step.
    /// Returns false when day and terminal are already open (nothing to guide).
    /// </summary>
    public async Task<bool> ShowAsync()
    {
        Notice.Text = string.Empty;
        _sessionOpened = false;
        AmountBox.Text = "0,00";

        SetBusy(true, "A verificar o dia e a sessão...");
        var services = AppComposition.Services;
        if (services is null)
        {
            SetBusy(false);
            Notice.Text = "A aplicação não está pronta.";
            _step = 0;
            ShowStep();
            return true;
        }

        try
        {
            var state = await services.GetRequiredService<IPosOrderService>().GetWorkSessionAsync();
            _dayOpen = state.DayOpen;
            _terminalOpen = state.TerminalOpen;
        }
        catch (Exception exception)
        {
            SetBusy(false);
            Notice.Text = exception.GetBaseException().Message;
            _dayOpen = false;
            _terminalOpen = false;
            _step = 0;
            ShowStep();
            return true;
        }

        if (_dayOpen && _terminalOpen)
        {
            SetBusy(false);
            return false;
        }

        _step = _dayOpen ? 1 : 0;
        ShowStep();
        SetBusy(false);
        return true;
    }

    private void SetBusy(bool busy, string? message = null)
    {
        _busy = busy;
        BackButton.IsEnabled = busy == false;
        PrimaryButton.IsEnabled = busy == false;
        LaterButton.IsEnabled = busy == false;
        TabBar.IsEnabled = busy == false;
        AmountBox.IsEnabled = busy == false;
        if (busy)
        {
            Busy.Show(message);
        }
        else
        {
            Busy.Hide();
        }
    }

    private void ShowStep()
    {
        DayStep.IsVisible = _step == 0;
        SessionStep.IsVisible = _step == 1;
        BackButton.IsVisible = _step > 0;

        if (_step == 0)
        {
            DayStatus.Text = _dayOpen
                ? "O dia já está aberto. Avance para abrir a sessão deste terminal."
                : "O dia ainda está fechado.";
            PrimaryText.Text = _dayOpen ? "Seguinte" : "Abrir dia";
        }
        else
        {
            PrimaryText.Text = "Abrir sessão";
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
            else if (index < _step || (index == 0 && _dayOpen))
            {
                tab.Classes.Add("bo_fiscal_wizard_tab_done");
            }

            var target = index;
            tab.Click += (_, _) =>
            {
                if (_busy)
                {
                    return;
                }

                if (target == 0 || _dayOpen)
                {
                    _step = target;
                    Notice.Text = string.Empty;
                    ShowStep();
                }
            };
            TabBar.Children.Add(tab);
        }
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        if (_step > 0)
        {
            _step--;
            Notice.Text = string.Empty;
            ShowStep();
        }
    }

    private async void OnPrimaryClick(object? sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        Notice.Text = string.Empty;
        if (_step == 0)
        {
            if (_dayOpen)
            {
                _step = 1;
                ShowStep();
                return;
            }

            await RunOpenDayAsync();
            return;
        }

        await RunOpenSessionAsync();
    }

    private async Task RunOpenDayAsync()
    {
        SetBusy(true, "A abrir o dia...");
        var result = await RunActionAsync("open-day", null);
        if (result.Succeeded)
        {
            _dayOpen = true;
            _step = 1;
            Notice.Text = result.Message ?? "Dia aberto.";
            ShowStep();
        }
        else
        {
            Notice.Text = result.Error ?? result.Message ?? "Não foi possível abrir o dia.";
        }

        SetBusy(false);
    }

    private async Task RunOpenSessionAsync()
    {
        if (MoneyFormat.TryParse(AmountBox.Text, out var amount) == false || amount < 0)
        {
            Notice.Text = "Indique um valor inicial válido (zero ou superior).";
            return;
        }

        SetBusy(true, "A abrir a sessão...");
        var result = await RunActionAsync("open-session", AmountBox.Text);
        if (result.Succeeded)
        {
            _terminalOpen = true;
            _sessionOpened = true;
            SetBusy(false);
            Finished?.Invoke(this, new FrontOfficeStartupResult(SessionOpened: true, AmountText: AmountBox.Text));
            return;
        }

        Notice.Text = result.Error ?? result.Message ?? "Não foi possível abrir a sessão.";
        SetBusy(false);
    }

    private async Task<ListingSaveResult> RunActionAsync(string action, string? amount)
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return ListingSaveResult.Fail("A aplicação não está pronta.");
        }

        try
        {
            return await services.GetRequiredService<IBackOfficeListingService>()
                .RunActionAsync(action, null, amount);
        }
        catch (Exception exception)
        {
            return ListingSaveResult.Fail(exception.GetBaseException().Message);
        }
    }

    private void OnLaterClick(object? sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        Finished?.Invoke(this, new FrontOfficeStartupResult(SessionOpened: _sessionOpened, AmountText: null));
    }
}

public sealed record FrontOfficeStartupResult(bool SessionOpened, string? AmountText);
