using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class CompanySetupWindow : Window
{
    private readonly ICompanySetupService _service;
    private readonly Dictionary<string, TextBox> _inputs = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, TextBlock> _captions = new(StringComparer.OrdinalIgnoreCase);
    private CompanySetupDraft? _draft;
    private ComboBox? _countryBox;
    private ComboBox? _currencyBox;
    private bool _saved;

    public CompanySetupWindow()
        : this(AppComposition.Services?.GetService<ICompanySetupService>()
               ?? throw new InvalidOperationException("ICompanySetupService is not registered."))
    {
    }

    public CompanySetupWindow(ICompanySetupService service)
    {
        _service = service;
        InitializeComponent();
        // Banner lives on the owner shell (LoginShell / POS); avoid stacking a second strip.
        Opened += (_, _) => CoverOwner();
        Opened += async (_, _) => await LoadAsync();
        Closing += (_, args) =>
        {
            // GTK blocks ESC until OK — keep the same behaviour for a fresh database.
            if (_saved == false)
            {
                args.Cancel = true;
                Notice.Text = "Preencha e confirme os dados da empresa para continuar.";
            }
        };
        KeyDown += (_, e) =>
        {
            if (_saved == false && e.Key == Avalonia.Input.Key.Escape)
            {
                e.Handled = true;
                Notice.Text = "Preencha e confirme os dados da empresa para continuar.";
            }
        };
    }

    private void CoverOwner()
    {
        if (Owner is not Window owner)
        {
            return;
        }

        Position = owner.PointToScreen(new Point(0, 0));
        Width = owner.Bounds.Width;
        Height = owner.Bounds.Height;
    }

    private async Task LoadAsync()
    {
        Notice.Text = "Indique os dados da empresa (obrigatório na primeira utilização).";
        FieldsHost.Children.Clear();
        _inputs.Clear();
        _captions.Clear();

        try
        {
            _draft = await _service.LoadAsync();
        }
        catch (Exception ex)
        {
            Notice.Text = "Não foi possível carregar os dados da empresa.";
            System.Diagnostics.Debug.WriteLine(ex);
            return;
        }

        _countryBox = AddCombo(
            PreferenceLabels.Text("global_country", "País"),
            _draft.Countries,
            _draft.CountryId);
        _currencyBox = AddCombo(
            PreferenceLabels.Text("global_currency", "Moeda"),
            _draft.Currencies,
            _draft.CurrencyId);
        _countryBox.SelectionChanged += (_, _) => RefreshValidation();
        _currencyBox.SelectionChanged += (_, _) => RefreshValidation();

        // AGT-FE private key last: expand-on-focus multiline for the full PEM.
        foreach (var field in _draft.Fields.OrderBy(item => AgtFePrivateKeyBox.IsToken(item.Token) ? 1 : 0))
        {
            var caption = new TextBlock { Classes = { "bo_new_doc_caption" } };
            var box = AgtFePrivateKeyBox.IsToken(field.Token)
                ? AgtFePrivateKeyBox.Create(field.Value)
                : new TextBox { Classes = { "bo_entity_input" }, Text = field.Value };
            box.TextChanged += (_, _) => RefreshValidation();
            _inputs[field.Token] = box;
            _captions[field.Token] = caption;
            var stack = new StackPanel { Spacing = 4 };
            stack.Children.Add(caption);
            stack.Children.Add(box);
            FieldsHost.Children.Add(stack);
        }

        TouchFields.Attach(FieldsHost);
        RefreshValidation();
    }

    private ComboBox AddCombo(string caption, IReadOnlyList<CompanySetupChoice> options, Guid? selectedId)
    {
        var combo = new ComboBox
        {
            Classes = { "bo_entity_input" },
            ItemsSource = options,
            DisplayMemberBinding = new Avalonia.Data.Binding(nameof(CompanySetupChoice.Label))
        };
        combo.SelectedItem = options.FirstOrDefault(item => item.Id == selectedId) ?? options.FirstOrDefault();
        FieldsHost.Children.Add(Wrap(caption, combo));
        return combo;
    }

    private static StackPanel Wrap(string caption, Control input)
    {
        var stack = new StackPanel { Spacing = 4 };
        stack.Children.Add(new TextBlock { Classes = { "bo_new_doc_caption" }, Text = caption });
        stack.Children.Add(input);
        return stack;
    }

    private void RefreshValidation()
    {
        RefreshFieldCaptions();

        var country = _countryBox?.SelectedItem as CompanySetupChoice;
        var currency = _currencyBox?.SelectedItem as CompanySetupChoice;
        var ok = country is not null && currency is not null;
        foreach (var field in _draft?.Fields ?? [])
        {
            if (IsRequired(field) == false)
            {
                continue;
            }

            if (_inputs.TryGetValue(field.Token, out var box) == false ||
                string.IsNullOrWhiteSpace(box.Text))
            {
                ok = false;
                break;
            }
        }

        OkButton.IsEnabled = ok;
    }

    private void RefreshFieldCaptions()
    {
        foreach (var field in _draft?.Fields ?? [])
        {
            if (_captions.TryGetValue(field.Token, out var caption) == false)
            {
                continue;
            }

            var label = PreferenceLabels.Get(field.Label, field.Token);
            caption.Text = IsRequired(field)
                ? $"{PreferenceLabels.Title(label)} *"
                : PreferenceLabels.Title(label);
        }
    }

    private bool IsRequired(CompanySetupField field)
    {
        // AGT-FE private key is never mandatory for Angola (seed marks it required).
        if (AgtFePrivateKeyBox.IsToken(field.Token) && IsAngolaSelected())
        {
            return false;
        }

        return field.Required;
    }

    private bool IsAngolaSelected()
        => _countryBox?.SelectedItem is CompanySetupChoice country
           && string.Equals(country.Code, "AO", StringComparison.OrdinalIgnoreCase);

    private void OnDemoClick(object? sender, RoutedEventArgs e)
    {
        // Demo button: refill company fields with generic sample data.
        if (_countryBox is not null && _draft is not null)
        {
            _countryBox.SelectedItem = _draft.Countries.FirstOrDefault(item => item.Code == "PT")
                ?? _countryBox.SelectedItem;
        }

        if (_currencyBox is not null && _draft is not null)
        {
            _currencyBox.SelectedItem = _draft.Currencies.FirstOrDefault(item => item.Code == "EUR")
                ?? _currencyBox.SelectedItem;
        }

        foreach (var pair in LocalCompanySetupService.DemoCompanyValues)
        {
            if (_inputs.TryGetValue(pair.Key, out var box))
            {
                box.Text = pair.Value;
            }
        }

        Notice.Text = "Dados demo preenchidos. Confirme com OK.";
        RefreshValidation();
    }

    private async void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (_countryBox?.SelectedItem is not CompanySetupChoice country ||
            _currencyBox?.SelectedItem is not CompanySetupChoice currency)
        {
            Notice.Text = "Escolha o país e a moeda.";
            return;
        }

        var values = _inputs.ToDictionary(
            item => item.Key,
            item => AgtFePrivateKeyBox.TryRead(item.Value, out var pem)
                ? pem
                : item.Value.Text?.Trim() ?? string.Empty,
            StringComparer.OrdinalIgnoreCase);

        Notice.Text = "A gravar...";
        OkButton.IsEnabled = false;
        DemoButton.IsEnabled = false;
        try
        {
            var result = await _service.SaveAsync(new CompanySetupRequest
            {
                CountryId = country.Id,
                CurrencyId = currency.Id,
                Values = values
            });
            if (result.Succeeded == false)
            {
                Notice.Text = result.Error ?? "Não foi possível gravar.";
                DemoButton.IsEnabled = true;
                RefreshValidation();
                return;
            }

            _saved = true;
            Close();
        }
        catch (Exception ex)
        {
            Notice.Text = ex.Message;
            DemoButton.IsEnabled = true;
            RefreshValidation();
        }
    }
}
