using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
    private bool _saving;

    private sealed record Card(string Title, string Icon, int Columns, string[] Tokens);

    /// <summary>Section cards of the company form; fields no card claims land in "Outros".</summary>
    private static readonly Card[] Cards =
    [
        new("Identificação", "tag", 3, ["COMPANY_NAME", "COMPANY_BUSINESS_NAME", "COMPANY_FISCALNUMBER"]),
        new("Morada", "pin", 3, ["COMPANY_ADDRESS", "COMPANY_CITY", "COMPANY_POSTALCODE"]),
        new("Contactos", "phone", 3, ["COMPANY_TELEPHONE", "COMPANY_MOBILEPHONE", "COMPANY_FAX", "COMPANY_EMAIL", "COMPANY_WEBSITE"]),
        new("Dados fiscais", "layers", 2, ["COMPANY_STOCK_CAPITAL", "COMPANY_TAX_ENTITY"]),
        new("AGT-FE", "lock", 1, ["AGT_FE_QRCODE_LOGO", AgtFePrivateKeyBox.Token])
    ];

    private static readonly Dictionary<string, string> FieldIcons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["COMPANY_NAME"] = CustomerFieldIcons.FiscalNumber,
        ["COMPANY_BUSINESS_NAME"] = CustomerFieldIcons.PriceType,
        ["COMPANY_FISCALNUMBER"] = "avares://LogicPOS.App/Assets/Images/Articles/icon_barcode_b.svg",
        ["COMPANY_ADDRESS"] = CustomerFieldIcons.Address,
        ["COMPANY_CITY"] = CustomerFieldIcons.City,
        ["COMPANY_POSTALCODE"] = CustomerFieldIcons.ZipCode,
        ["COMPANY_TELEPHONE"] = CustomerFieldIcons.Phone,
        ["COMPANY_MOBILEPHONE"] = CustomerFieldIcons.Phone,
        ["COMPANY_FAX"] = CustomerFieldIcons.Phone,
        ["COMPANY_EMAIL"] = CustomerFieldIcons.Email,
        ["COMPANY_STOCK_CAPITAL"] = CustomerFieldIcons.Discount
    };

    public CompanySetupWindow()
        : this(AppComposition.Services?.GetService<ICompanySetupService>()
               ?? throw new InvalidOperationException("ICompanySetupService is not registered."))
    {
    }

    public CompanySetupWindow(ICompanySetupService service)
    {
        _service = service;
        InitializeComponent();
        ListingNotice.BindToast(Notice, this, skipProgress: true);
        // Banner lives on the owner shell (LoginShell / POS); avoid stacking a second strip.
        Opened += (_, _) => CoverOwner();
        Opened += async (_, _) => await LoadAsync();
        SizeChanged += (_, _) => PortraitChrome.FitToContent(DialogFrame, this, 1000, lockWidth: true);
        // Opened from the onboarding step: ✕ / ESC cancel without saving (the step stays pending).
        Closing += (_, args) => args.Cancel = _saving;
        KeyDown += (_, e) =>
        {
            if (e.Key == Avalonia.Input.Key.Escape)
            {
                e.Handled = true;
                Close();
            }
        };
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

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

        var portrait = Owner is Window owner && owner.Bounds.Height > owner.Bounds.Width;
        _countryBox = CreateCombo(_draft.Countries, _draft.CountryId);
        _currencyBox = CreateCombo(_draft.Currencies, _draft.CurrencyId);
        _countryBox.SelectionChanged += (_, _) => RefreshValidation();
        _currencyBox.SelectionChanged += (_, _) => RefreshValidation();
        FieldsHost.Children.Add(EntityListing.ArticleSection("País e moeda", "coins", CardGrid(
        [
            (Labelled(new TextBlock { Classes = { "bo_new_doc_caption" }, Text = PreferenceLabels.Text("global_country", "País") },
                CustomerFieldIcons.Country, _countryBox), false),
            (Labelled(new TextBlock { Classes = { "bo_new_doc_caption" }, Text = PreferenceLabels.Text("global_currency", "Moeda") },
                CustomerFieldIcons.Discount, _currencyBox), false)
        ], portrait ? 1 : 2)));

        var blocks = new Dictionary<string, (Control Block, bool Wide)>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in _draft.Fields)
        {
            var caption = new TextBlock { Classes = { "bo_new_doc_caption" } };
            // AGT-FE private key: expand-on-focus multiline for the full PEM, full card width.
            var wide = AgtFePrivateKeyBox.IsToken(field.Token);
            var box = wide
                ? AgtFePrivateKeyBox.Create(field.Value)
                : new TextBox { Classes = { "bo_entity_input" }, Text = field.Value };
            box.TextChanged += (_, _) => RefreshValidation();
            _inputs[field.Token] = box;
            _captions[field.Token] = caption;
            blocks[field.Token] = (Labelled(caption, FieldIcons.GetValueOrDefault(field.Token), box), wide);
        }

        var claimed = Cards.SelectMany(card => card.Tokens).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var others = new Card("Outros", "gear", 3,
            _draft.Fields.Select(field => field.Token).Where(token => claimed.Contains(token) == false).ToArray());
        foreach (var card in Cards.Append(others))
        {
            var cardBlocks = card.Tokens
                .Where(blocks.ContainsKey)
                .Select(token => blocks[token])
                .ToList();
            if (cardBlocks.Count > 0)
            {
                FieldsHost.Children.Add(EntityListing.ArticleSection(card.Title, card.Icon,
                    CardGrid(cardBlocks, portrait ? 1 : card.Columns)));
            }
        }

        TouchFields.Attach(FieldsHost);
        RefreshValidation();
    }

    private static ComboBox CreateCombo(IReadOnlyList<CompanySetupChoice> options, Guid? selectedId)
    {
        var combo = new ComboBox
        {
            Classes = { "bo_entity_input" },
            ItemsSource = options,
            DisplayMemberBinding = new Avalonia.Data.Binding(nameof(CompanySetupChoice.Label))
        };
        combo.SelectedItem = options.FirstOrDefault(item => item.Id == selectedId) ?? options.FirstOrDefault();
        return combo;
    }

    internal static StackPanel Labelled(TextBlock caption, string? icon, Control input)
    {
        var label = new StackPanel { Classes = { "bo_new_doc_label" } };
        if (icon is not null)
        {
            label.Children.Add(icon.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
                ? new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/")) { Classes = { "bo_new_doc_label_icon" }, Path = icon }
                : new Image { Classes = { "bo_new_doc_label_icon" }, Source = new Bitmap(AssetLoader.Open(new Uri(icon))) });
        }

        label.Children.Add(caption);
        var stack = new StackPanel();
        stack.Children.Add(label);
        stack.Children.Add(input);
        return stack;
    }

    /// <summary>Card field grid: left to right in <paramref name="columns"/>; wide fields take a full row.</summary>
    private static Grid CardGrid(IReadOnlyList<(Control Block, bool Wide)> blocks, int columns)
    {
        var grid = new Grid { Classes = { "bo_entity_columns" } };
        for (var index = 0; index < columns; index++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        }

        var row = 0;
        var column = 0;
        foreach (var (block, wide) in blocks)
        {
            if (wide && column > 0)
            {
                column = 0;
                row++;
            }

            if (grid.RowDefinitions.Count <= row)
            {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }

            Grid.SetRow(block, row);
            Grid.SetColumn(block, column);
            if (wide)
            {
                Grid.SetColumnSpan(block, columns);
            }

            grid.Children.Add(block);
            column = wide ? columns : column + 1;
            if (column >= columns)
            {
                column = 0;
                row++;
            }
        }

        return grid;
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
        _saving = true;
        try
        {
            var result = await _service.SaveAsync(new CompanySetupRequest
            {
                CountryId = country.Id,
                CurrencyId = currency.Id,
                Values = values
            });
            _saving = false;
            if (result.Succeeded == false)
            {
                Notice.Text = result.Error ?? "Não foi possível gravar.";
                DemoButton.IsEnabled = true;
                RefreshValidation();
                return;
            }

            Close();
        }
        catch (Exception ex)
        {
            _saving = false;
            Notice.Text = ex.Message;
            DemoButton.IsEnabled = true;
            RefreshValidation();
        }
    }
}
