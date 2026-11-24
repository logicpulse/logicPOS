using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

/// <summary>
/// Primeira entrada: modo de operação, base de dados de demonstração e dados da empresa.
/// Adapter only; the rules live in <see cref="IFirstEntryService"/>. Closing leaves it pending.
/// </summary>
public partial class FirstEntryWindow : Window
{
    private static readonly string[] Steps = ["Modo", "Demonstração", "Empresa"];

    private readonly IFirstEntryService _service;
    private int _step;
    private bool _busy;
    private bool _loaded;

    public bool Completed { get; private set; }

    public FirstEntryWindow()
        : this(AppComposition.Services?.GetService<IFirstEntryService>()
               ?? throw new InvalidOperationException("IFirstEntryService is not registered."))
    {
    }

    public FirstEntryWindow(IFirstEntryService service)
    {
        _service = service;
        InitializeComponent();
        TouchFields.Attach(this);
        ListingNotice.BindToast(LblStatus, this);
        CmbCountry.SelectionChanged += async (_, _) => await OnCountryChangedAsync();
        ShowStep();
        Opened += async (_, _) => await LoadAsync();
    }

    private async Task LoadAsync()
    {
        FirstEntryPending pending;
        try
        {
            pending = await _service.GetPendingAsync();
        }
        catch (Exception exception)
        {
            LblStatus.Text = exception.GetBaseException().Message;
            NextButton.IsEnabled = false;
            ConfirmButton.IsEnabled = false;
            return;
        }

        if (pending.IsPending == false)
        {
            Close();
            return;
        }

        // Fill the boxes before enabling the country handler's currency suggestion.
        _loaded = false;
        CmbMode.ItemsSource = pending.Modes;
        CmbMode.SelectedItem = pending.Modes.FirstOrDefault(mode => mode == pending.OperationMode)
                               ?? pending.Modes.FirstOrDefault();

        RbDemoAccept.IsChecked = pending.DemoAccepted;
        RbDemoRefuse.IsChecked = pending.DemoAccepted == false;

        CmbCountry.ItemsSource = pending.Countries;
        CmbCurrency.ItemsSource = pending.Currencies;
        CmbCountry.DisplayMemberBinding = new Avalonia.Data.Binding(nameof(CompanySetupChoice.Label));
        CmbCurrency.DisplayMemberBinding = new Avalonia.Data.Binding(nameof(CompanySetupChoice.Label));
        CmbCountry.SelectedItem = pending.Countries.FirstOrDefault(item => item.Id == pending.CountryId);
        CmbCurrency.SelectedItem = pending.Currencies.FirstOrDefault(item => item.Id == pending.CurrencyId);
        TxtFiscalNumber.Text = pending.FiscalNumber;
        _loaded = true;

        // Country prepared without a currency: suggest one the same way as a manual change.
        if (CmbCountry.SelectedItem is not null && CmbCurrency.SelectedItem is null)
        {
            await OnCountryChangedAsync();
        }
    }

    private async Task OnCountryChangedAsync()
    {
        if (_loaded == false)
        {
            return;
        }

        var country = CmbCountry.SelectedItem as CompanySetupChoice;
        try
        {
            var suggested = await _service.SuggestCurrencyIdAsync(country?.Id);
            if (suggested is not null && CmbCurrency.ItemsSource is IEnumerable<CompanySetupChoice> currencies)
            {
                CmbCurrency.SelectedItem = currencies.FirstOrDefault(item => item.Id == suggested);
            }
        }
        catch (Exception exception)
        {
            LblStatus.Text = exception.GetBaseException().Message;
        }
    }

    private FirstEntryRequest BuildRequest() => new()
    {
        OperationMode = CmbMode.SelectedItem as string,
        DemoAccepted = RbDemoAccept.IsChecked == true,
        CountryId = (CmbCountry.SelectedItem as CompanySetupChoice)?.Id,
        CurrencyId = (CmbCurrency.SelectedItem as CompanySetupChoice)?.Id,
        FiscalNumber = TxtFiscalNumber.Text
    };

    private void ShowStep()
    {
        ModeStep.IsVisible = _step == 0;
        DemoStep.IsVisible = _step == 1;
        CompanyStep.IsVisible = _step == 2;
        BackButton.IsVisible = _step > 0;
        NextButton.IsVisible = _step < Steps.Length - 1;
        ConfirmButton.IsVisible = _step == Steps.Length - 1;

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

        if (_step == 0 && CmbMode.SelectedItem is null)
        {
            LblStatus.Text = "Selecione o modo de operação.";
            return;
        }

        LblStatus.Text = string.Empty;
        _step++;
        ShowStep();
    }

    private async void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        _busy = true;
        ConfirmButton.IsEnabled = false;
        BackButton.IsEnabled = false;
        LblStatus.Text = string.Empty;

        try
        {
            var result = await _service.ConfirmAsync(BuildRequest());
            switch (result)
            {
                case FirstEntryConfirmationResult.Confirmed:
                case FirstEntryConfirmationResult.AlreadyConfirmed:
                    Completed = result == FirstEntryConfirmationResult.Confirmed;
                    Close();
                    break;
                case FirstEntryConfirmationResult.Incomplete:
                    LblStatus.Text = "Indique o modo, o país, a moeda e o NIF.";
                    break;
                default:
                    LblStatus.Text = "Não foi possível carregar os dados de demonstração. Nada foi gravado.";
                    break;
            }
        }
        catch (Exception exception)
        {
            LblStatus.Text = exception.GetBaseException().Message;
        }
        finally
        {
            _busy = false;
            ConfirmButton.IsEnabled = true;
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
