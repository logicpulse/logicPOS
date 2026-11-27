using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

/// <summary>
/// Primeira entrada: tipo (POS / Faturação), modos POS + demo, ou empresa directo na faturação.
/// Adapter only; the rules live in <see cref="IFirstEntryService"/>. Closing leaves it pending.
/// </summary>
public partial class FirstEntryWindow : Window
{
    private const string InvoiceMode = "default";

    private static readonly string[] PosSteps = ["Tipo", "Modo", "Demonstração", "Empresa"];
    private static readonly string[] InvoiceSteps = ["Tipo", "Empresa"];

    private readonly IFirstEntryService _service;
    private readonly Dictionary<string, Bitmap> _modeBitmaps = new(StringComparer.OrdinalIgnoreCase);
    private IReadOnlyList<string> _posModes = [];
    private string? _selectedMode;
    private int _stepIndex;
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
        EnglishUi.Apply(this);
        TouchFields.Attach(this);
        ListingNotice.BindToast(LblStatus, this);
        RbProductPos.IsCheckedChanged += OnProductChanged;
        RbProductInvoice.IsCheckedChanged += OnProductChanged;
        CmbCountry.SelectionChanged += async (_, _) => await OnCountryChangedAsync();
        ShowStep();
        Opened += async (_, _) => await LoadAsync();
    }

    private bool IsInvoiceSelected => RbProductInvoice.IsChecked == true;

    private string[] ActiveSteps => IsInvoiceSelected ? InvoiceSteps : PosSteps;

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
        _posModes = pending.Modes
            .Where(mode => string.Equals(mode, InvoiceMode, StringComparison.OrdinalIgnoreCase) == false)
            .ToList();

        var mode = pending.OperationMode?.Trim();
        if (string.Equals(mode, InvoiceMode, StringComparison.OrdinalIgnoreCase))
        {
            RbProductInvoice.IsChecked = true;
            RbProductPos.IsChecked = false;
            _selectedMode = null;
        }
        else if (string.IsNullOrWhiteSpace(mode) == false)
        {
            RbProductPos.IsChecked = true;
            RbProductInvoice.IsChecked = false;
            _selectedMode = _posModes.FirstOrDefault(item =>
                string.Equals(item, mode, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            RbProductPos.IsChecked = false;
            RbProductInvoice.IsChecked = false;
            _selectedMode = null;
        }

        ShowModes();

        RbDemoAccept.IsChecked = pending.DemoAccepted && IsInvoiceSelected == false;
        RbDemoRefuse.IsChecked = RbDemoAccept.IsChecked != true;

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

        ShowStep();
    }

    private void OnProductChanged(object? sender, RoutedEventArgs e)
    {
        if (_loaded == false || _busy)
        {
            return;
        }

        // Both radios fire on a group change; react only to the newly checked one.
        if (sender is RadioButton { IsChecked: not true })
        {
            return;
        }

        if (IsInvoiceSelected)
        {
            _selectedMode = null;
            RbDemoRefuse.IsChecked = true;
            RbDemoAccept.IsChecked = false;
        }
        else if (string.IsNullOrEmpty(_selectedMode))
        {
            _selectedMode = _posModes.FirstOrDefault();
        }

        ShowModes();

        if (_stepIndex >= ActiveSteps.Length)
        {
            _stepIndex = ActiveSteps.Length - 1;
        }

        LblStatus.Text = string.Empty;
        ShowStep();
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
        OperationMode = IsInvoiceSelected ? InvoiceMode : _selectedMode,
        DemoAccepted = IsInvoiceSelected == false && RbDemoAccept.IsChecked == true,
        CountryId = (CmbCountry.SelectedItem as CompanySetupChoice)?.Id,
        CurrencyId = (CmbCurrency.SelectedItem as CompanySetupChoice)?.Id,
        FiscalNumber = TxtFiscalNumber.Text
    };

    private void ShowStep()
    {
        var steps = ActiveSteps;
        var current = steps[_stepIndex];
        ProductStep.IsVisible = current == "Tipo";
        ModeStep.IsVisible = current == "Modo";
        DemoStep.IsVisible = current == "Demonstração";
        CompanyStep.IsVisible = current == "Empresa";
        BackButton.IsVisible = _stepIndex > 0;
        NextButton.IsVisible = _stepIndex < steps.Length - 1;
        ConfirmButton.IsVisible = _stepIndex == steps.Length - 1;

        TabBar.Children.Clear();
        for (var index = 0; index < steps.Length; index++)
        {
            var tab = new Button
            {
                Classes = { "bo_fiscal_wizard_tab" },
                Content = $"{index + 1}. {steps[index]}"
            };
            if (index == _stepIndex)
            {
                tab.Classes.Add("bo_fiscal_wizard_tab_on");
            }
            else if (index < _stepIndex)
            {
                tab.Classes.Add("bo_fiscal_wizard_tab_done");
            }

            var target = index;
            tab.Click += (_, _) =>
            {
                if (_busy == false && target <= _stepIndex)
                {
                    _stepIndex = target;
                    LblStatus.Text = string.Empty;
                    ShowStep();
                }
            };
            TabBar.Children.Add(tab);
        }
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        if (_busy || _stepIndex == 0)
        {
            return;
        }

        _stepIndex--;
        LblStatus.Text = string.Empty;
        ShowStep();
    }

    private void OnNextClick(object? sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        var steps = ActiveSteps;
        var current = steps[_stepIndex];
        if (current == "Tipo" && RbProductPos.IsChecked != true && RbProductInvoice.IsChecked != true)
        {
            LblStatus.Text = "Escolha POS ou Faturação.";
            return;
        }

        if (current == "Modo" && string.IsNullOrEmpty(_selectedMode))
        {
            LblStatus.Text = "Selecione o modo de operação.";
            return;
        }

        LblStatus.Text = string.Empty;
        _stepIndex++;
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
                case FirstEntryConfirmationResult.InvalidFiscalNumber:
                    LblStatus.Text = GtkFormRules.InvalidFiscalNumberUserMessage;
                    break;
                case FirstEntryConfirmationResult.Incomplete:
                    LblStatus.Text = "Indique o tipo, o país, a moeda e o NIF.";
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

    private void ShowModes()
    {
        ModeGrid.Children.Clear();
        foreach (var mode in _posModes)
        {
            var selected = string.Equals(mode, _selectedMode, StringComparison.OrdinalIgnoreCase);
            var card = new Border
            {
                Classes = { "bo_first_entry_mode" },
                Tag = mode
            };
            if (selected)
            {
                card.Classes.Add("bo_first_entry_mode_on");
            }

            card.PointerPressed += OnModePressed;

            var body = new StackPanel { Spacing = 6 };
            var bitmap = ModeBitmap(mode);
            if (bitmap is not null)
            {
                body.Children.Add(new Border
                {
                    Classes = { "bo_first_entry_mode_shot" },
                    Child = new Image
                    {
                        Classes = { "bo_first_entry_mode_image" },
                        Source = bitmap
                    }
                });
            }

            body.Children.Add(new TextBlock
            {
                Classes = { "bo_first_entry_mode_label" },
                Text = FirstEntryModeText.Label(mode)
            });
            card.Child = body;
            ModeGrid.Children.Add(card);
        }
    }

    private void OnModePressed(object? sender, PointerPressedEventArgs e)
    {
        if (_busy || sender is not Border { Tag: string mode })
        {
            return;
        }

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == false)
        {
            return;
        }

        _selectedMode = mode;
        LblStatus.Text = string.Empty;
        ShowModes();
        e.Handled = true;
    }

    private Bitmap? ModeBitmap(string mode)
    {
        var token = mode.Trim().ToLowerInvariant();
        if (_modeBitmaps.TryGetValue(token, out var cached))
        {
            return cached;
        }

        var bitmap = OpenModeBitmap(token) ?? OpenModeBitmap("default");
        if (bitmap is not null)
        {
            _modeBitmaps[token] = bitmap;
        }

        return bitmap;
    }

    private static Bitmap? OpenModeBitmap(string token)
    {
        var uri = new Uri($"avares://LogicPOS.App/Assets/Images/OperationModes/{token}.bmp");
        if (AssetLoader.Exists(uri) == false)
        {
            return null;
        }

        return new Bitmap(AssetLoader.Open(uri));
    }

    private void OnTitlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }
}
