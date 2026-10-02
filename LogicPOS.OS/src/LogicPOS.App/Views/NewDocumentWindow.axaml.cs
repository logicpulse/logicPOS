using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class NewDocumentWindow : UserControl
{
    private readonly List<PosDocumentTypeOption> _types = new();
    private readonly List<NewDocumentLine> _lines = new();
    private readonly List<CatalogChoice> _catalog = new();
    private readonly List<CatalogChoice> _matches = new();
    private readonly List<PosCustomer> _customerMatches = new();
    private int _customerSearch;
    private PosCatalog _posCatalog = PosCatalog.Empty;
    private Guid? _padFamilyId;
    private Guid? _padSubfamilyId;
    private readonly List<PosLookupItem> _conditions = new();
    private readonly List<PosLookupItem> _currencies = new();
    private readonly List<PosLookupItem> _documents = new();
    private readonly List<PosVatOption> _vatRates = new();
    private TaskCompletionSource<bool>? _closed;
    private bool _filling;
    private int _customerLock;
    private bool _editorOpen;
    private NewDocumentLine? _editing;
    private LineSnapshot? _editSnapshot;

    public string? CreatedPdfPath { get; private set; }

    public string? CreatedPdfTitle { get; private set; }

    public event Func<string, string?, Task>? PreviewRequested;

    private Guid? _previewDraftId;
    private Guid? _seedDraftId;
    private Guid? _customerId;
    private bool _customerIsFinal;
    private PosCustomer? _customer;
    private PosCompanyAddress _company = new();
    private Guid? _editorCustomerId;
    private readonly List<LookupOption> _customerTypes = new();
    private readonly List<LookupOption> _priceTypes = new();
    private readonly List<LookupOption> _countries = new();

    public NewDocumentWindow()
    {
        InitializeComponent();
        TouchFields.Attach(this);
    }

    public Task<bool> ShowAsync(Guid? draftId = null)
    {
        _seedDraftId = draftId;
        _lines.Clear();
        LinesGrid.ItemsSource = null;
        ArticleSearch.Text = string.Empty;
        NotesBox.Text = string.Empty;
        ErrorLabel.Text = string.Empty;
        LineEditor.IsVisible = false;
        _editorOpen = false;
        _editing = null;
        _previewDraftId = null;
        DraftBox.IsChecked = false;
        RefreshIssueLabel();
        TransportBox.IsChecked = false;
        TransportOverlay.IsVisible = false;
        TransportButton.IsVisible = false;
        CustomerOverlay.IsVisible = false;
        CustomerEditor.IsVisible = false;
        ArticlePad.IsVisible = false;
        ArticleSuggest.IsVisible = false;
        CustomerSuggest.IsVisible = false;
        CustomerEditButton.IsVisible = false;
        _customerId = null;
        _customer = null;
        CustomerName.Text = string.Empty;
        CustomerFiscal.Text = string.Empty;
        DateLabel.Text = DateTime.Now.ToString("dd/MM/yyyy");
        RefreshTotal();
        CreatedPdfPath = null;
        CreatedPdfTitle = null;
        _closed = new TaskCompletionSource<bool>();
        _ = LoadAsync();
        return _closed.Task;
    }

    private async Task LoadAsync()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        _posCatalog = await services.GetRequiredService<IPosCatalogService>().LoadAsync();
        _catalog.Clear();
        _catalog.AddRange(_posCatalog.Articles.Select(article => new CatalogChoice(article, FamilyName(_posCatalog, article))));
        ApplyFilter();
        var documents = services.GetRequiredService<IPosDocumentService>();
        _filling = true;
        await Fill(async () =>
        {
            var types = await documents.ListDocumentTypesAsync();
            _types.Clear();
            _types.AddRange(types);
            TypeBox.ItemsSource = _types.Select(type => type.Designation).ToList();
            var preferred = DocumentDefaultStore.Read();
            var index = string.IsNullOrWhiteSpace(preferred)
                ? -1
                : _types.FindIndex(type => string.Equals(type.Acronym, preferred, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                index = _types.FindIndex(type => type.Acronym == "FS");
            }

            TypeBox.SelectedIndex = index >= 0 ? index : 0;
        });
        await Fill(async () =>
        {
            _conditions.Clear();
            _conditions.AddRange(await documents.ListPaymentConditionsAsync());
            PaymentBox.ItemsSource = Optional(_conditions);
            SelectCode(PaymentBox, "PP");
        });
        await Fill(async () =>
        {
            _currencies.Clear();
            _currencies.AddRange(await documents.ListCurrenciesAsync());
            CurrencyBox.ItemsSource = Optional(_currencies);
            SelectCode(CurrencyBox, "EUR");
        });
        await Fill(async () =>
        {
            _documents.Clear();
            _documents.AddRange(await documents.ListSourceDocumentsAsync());
            OriginBox.ItemsSource = Optional(_documents);
            CopyBox.ItemsSource = Optional(_documents);
            OriginBox.SelectedIndex = 0;
            CopyBox.SelectedIndex = 0;
        });
        await Fill(async () =>
        {
            _vatRates.Clear();
            _vatRates.AddRange(await documents.ListVatRatesAsync());
            EditVat.ItemsSource = _vatRates.ToList();
        });
        await Fill(async () => await ApplyPortugalAddressesAsync(documents));
        await Fill(async () => await SeedDraftAsync(documents, services.GetRequiredService<IPosCustomerService>()));
        await Fill(async () =>
        {
            if (_customerId is not null)
            {
                return;
            }

            var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync();
            if (customer is not null)
            {
                ShowCustomer(customer);
            }
        });
        _filling = false;
        UpdateTransport(openPopup: PosDocumentRules.IsWayBill(SelectedType()?.Acronym));
    }

    private static void SelectCode(ComboBox box, string code)
    {
        if (box.ItemsSource is not IEnumerable<PosLookupItem> items)
        {
            return;
        }

        var match = items.FirstOrDefault(item => string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase));
        box.SelectedItem = match ?? items.FirstOrDefault();
    }

    private async Task ApplyPortugalAddressesAsync(IPosDocumentService documents)
    {
        _company = await documents.GetCompanyAddressAsync();
        var now = DateTime.Now;
        FillShip(ToAddress, ToRegion, ToPostal, ToCity, ToCountry, PosDocumentRules.Unknown, PosDocumentRules.Unknown, "0000-000", PosDocumentRules.Unknown, "Portugal");
        // Delivery must be after dispatch (date/time); default arrival one hour later.
        SetWhen(ToDate, ToTime, now.AddHours(1));
        FillShip(
            FromAddress,
            FromRegion,
            FromPostal,
            FromCity,
            FromCountry,
            string.IsNullOrWhiteSpace(_company.Address) ? PosDocumentRules.Unknown : _company.Address,
            PosDocumentRules.Unknown,
            string.IsNullOrWhiteSpace(_company.PostalCode) ? "0000-000" : _company.PostalCode,
            string.IsNullOrWhiteSpace(_company.City) ? PosDocumentRules.Unknown : _company.City,
            string.IsNullOrWhiteSpace(_company.Country) ? "Portugal" : _company.Country);
        SetWhen(FromDate, FromTime, now);
    }

    private async Task SeedDraftAsync(IPosDocumentService documents, IPosCustomerService customers)
    {
        if (_seedDraftId is not Guid documentId)
        {
            return;
        }

        _previewDraftId = documentId;
        DraftBox.IsChecked = true;
        var stored = LocalDraftStore.Find(documentId);
        if (stored is not null)
        {
            NotesBox.Text = stored.Notes;
            var typeIndex = _types.FindIndex(type => string.Equals(type.Acronym, stored.Acronym, StringComparison.OrdinalIgnoreCase));
            if (typeIndex >= 0)
            {
                TypeBox.SelectedIndex = typeIndex;
            }

            SelectId(PaymentBox, stored.PaymentConditionId);
            SelectId(CurrencyBox, stored.CurrencyId);
            SelectId(OriginBox, stored.ParentDocumentId);
            if (stored.CustomerId != Guid.Empty)
            {
                var customer = await customers.FindAsync(stored.CustomerId);
                if (customer is not null)
                {
                    ShowCustomer(customer);
                }
            }

            if (stored.ShipTo is not null || stored.ShipFrom is not null)
            {
                TransportBox.IsChecked = true;
                ApplyStoredShip(stored.ShipTo, ToAddress, ToRegion, ToPostal, ToCity, ToCountry, ToDate, ToTime);
                ApplyStoredShip(stored.ShipFrom, FromAddress, FromRegion, FromPostal, FromCity, FromCountry, FromDate, FromTime);
            }
        }

        var copied = await documents.LoadDocumentLinesAsync(documentId);
        _lines.Clear();
        foreach (var line in copied)
        {
            var article = _catalog.FirstOrDefault(item => item.Article.Id == line.ArticleId)?.Article;
            _lines.Add(article is null
                ? NewDocumentLine.FromCopy(line, string.Empty)
                : NewDocumentLine.FromCopy(line, FamilyNameOf(article)));
        }

        RefreshLines();
    }

    private async Task Fill(Func<Task> load)
    {
        try
        {
            await load();
        }
        catch (Exception exception)
        {
            ErrorLabel.Text = exception.Message;
        }
    }

    private static List<PosLookupItem> Optional(IEnumerable<PosLookupItem> items)
    {
        var list = new List<PosLookupItem> { new(Guid.Empty, "—") };
        list.AddRange(items);
        return list;
    }

    private async void OnCopyChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_filling || SelectedId(CopyBox) is not Guid documentId)
        {
            return;
        }

        var service = AppComposition.Services?.GetService<IPosDocumentService>();
        if (service is null)
        {
            return;
        }

        var copied = await service.LoadDocumentLinesAsync(documentId);
        _lines.Clear();
        foreach (var line in copied)
        {
            var article = _catalog.FirstOrDefault(item => item.Article.Id == line.ArticleId)?.Article;
            _lines.Add(article is null
                ? NewDocumentLine.FromCopy(line, string.Empty)
                : NewDocumentLine.FromCopy(line, FamilyNameOf(article)));
        }

        RefreshLines();
    }

    private async void OnTypeChanged(object? sender, SelectionChangedEventArgs e)
    {
        var type = SelectedType();
        if (type is null)
        {
            SeriesLabel.Text = string.Empty;
            return;
        }

        var service = AppComposition.Services?.GetService<IPosDocumentService>();
        SeriesLabel.Text = service is null ? string.Empty : await service.GetSeriesLabelAsync(type.Acronym);
        UpdateTransport(openPopup: _filling == false && PosDocumentRules.IsWayBill(type.Acronym));
        if (_filling == false && PosDocumentRules.RequiresNamedCustomer(type.Acronym) && ShowsFinalConsumer())
        {
            ClearCustomer();
            ErrorLabel.Text = "Escolha um cliente para este documento.";
        }
    }

    private void OnTransportChanged(object? sender, RoutedEventArgs e)
    {
        if (_filling)
        {
            return;
        }

        UpdateTransport(openPopup: TransportBox.IsChecked == true);
    }

    private void OnOpenTransportClick(object? sender, RoutedEventArgs e)
    {
        if (PosDocumentRules.SupportsOptionalTransport(SelectedType()?.Acronym) && TransportBox.IsChecked != true)
        {
            TransportBox.IsChecked = true;
            return;
        }

        TransportOverlay.IsVisible = true;
    }

    private void OnCloseTransportClick(object? sender, RoutedEventArgs e)
    {
        TransportOverlay.IsVisible = false;
    }

    private void UpdateTransport(bool openPopup)
    {
        var acronym = SelectedType()?.Acronym;
        var optional = PosDocumentRules.SupportsOptionalTransport(acronym);
        var waybill = PosDocumentRules.IsWayBill(acronym);
        TransportBox.IsVisible = optional;
        TransportButton.IsVisible = waybill || optional;
        if (optional == false && waybill == false)
        {
            TransportBox.IsChecked = false;
        }

        var active = waybill || (optional && TransportBox.IsChecked == true);
        if (active == false)
        {
            TransportOverlay.IsVisible = false;
            return;
        }

        if (openPopup)
        {
            TransportOverlay.IsVisible = true;
        }

        RefreshTransportDates();
    }

    private void OnTransportDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        RefreshTransportDates();
    }

    private void OnTransportTimeChanged(object? sender, TimePickerSelectedValueChangedEventArgs e)
    {
        RefreshTransportDates();
    }

    private void RefreshTransportDates()
    {
        var check = PosDocumentRules.CheckTransportDates(CombineWhen(FromDate, FromTime), CombineWhen(ToDate, ToTime));
        SetDateState(FromDate, check.DispatchValid);
        SetDateState(ToDate, check.ArrivalValid);
        TransportDateError.Text = check.Message ?? string.Empty;
        TransportDateError.IsVisible = check.Message is not null;
    }

    private static DateTime? CombineWhen(CalendarDatePicker date, TimePicker time)
    {
        if (date.SelectedDate is null)
        {
            return null;
        }

        var clock = time.SelectedTime ?? TimeSpan.Zero;
        return date.SelectedDate.Value.Date.Add(clock);
    }

    private static void SetDateState(CalendarDatePicker picker, bool valid)
    {
        if (valid)
        {
            picker.Classes.Remove("bo_new_doc_date_invalid");
            return;
        }

        if (picker.Classes.Contains("bo_new_doc_date_invalid") == false)
        {
            picker.Classes.Add("bo_new_doc_date_invalid");
        }
    }

    private void OnSearchChanged(object? sender, TextChangedEventArgs e)
    {
        ApplyFilter();
    }

    private void OnSearchKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && _matches.Count > 0)
        {
            AddChoice(_matches[0]);
        }
    }

    private void OnEditRowClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: NewDocumentLine line })
        {
            OpenEditor(line);
        }
    }

    private void OnRemoveRowClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: NewDocumentLine line })
        {
            RemoveLine(line);
        }
    }

    private void RemoveLine(NewDocumentLine line)
    {
        _lines.Remove(line);
        ErrorLabel.Text = string.Empty;
        RefreshLines();
    }

    private void OnLineOpen(object? sender, TappedEventArgs e)
    {
        if (_editorOpen == false && LinesGrid.SelectedItem is NewDocumentLine line)
        {
            OpenEditor(line);
        }
    }

    private void AddChoice(CatalogChoice choice)
    {
        var article = choice.Article;
        var existing = _lines.FirstOrDefault(line => line.ArticleId == article.Id);
        if (existing is not null)
        {
            existing.IncreaseQuantity(article.QuantityStep);
        }
        else
        {
            _lines.Add(new NewDocumentLine(article, choice.Family));
        }

        ErrorLabel.Text = string.Empty;
        ArticleSearch.Text = string.Empty;
        RefreshLines();
    }

    private void OnOpenArticlePad(object? sender, RoutedEventArgs e)
    {
        _padFamilyId = _posCatalog.Families.FirstOrDefault()?.Id;
        _padSubfamilyId = SubfamiliesForPad().FirstOrDefault()?.Id;
        BuildArticlePad();
        ArticlePad.IsVisible = true;
    }

    private void OnArticlePadClose(object? sender, RoutedEventArgs e)
    {
        ArticlePad.IsVisible = false;
    }

    private void BuildArticlePad()
    {
        PadFamilies.Children.Clear();
        foreach (var family in _posCatalog.Families)
        {
            var selected = family.Id == _padFamilyId;
            var button = PadButton(family.Text, selected, () =>
            {
                _padFamilyId = family.Id;
                _padSubfamilyId = SubfamiliesForPad().FirstOrDefault()?.Id;
                BuildArticlePad();
            });
            PadFamilies.Children.Add(button);
        }

        PadSubfamilies.Children.Clear();
        var subfamilies = SubfamiliesForPad();
        PadSubfamilyScroll.IsVisible = subfamilies.Count > 0;
        foreach (var subfamily in subfamilies)
        {
            var selected = subfamily.Id == _padSubfamilyId;
            var button = PadButton(subfamily.Text, selected, () =>
            {
                _padSubfamilyId = subfamily.Id;
                BuildArticlePad();
            });
            PadSubfamilies.Children.Add(button);
        }

        PadArticles.Children.Clear();
        foreach (var article in PadArticleChoices())
        {
            var choice = article;
            var button = PadButton(choice.Designation, false, () => AddChoice(choice));
            button.Classes.Remove("pos_menu_button_green");
            button.Classes.Add("pos_menu_button_grey");
            PadArticles.Children.Add(button);
        }
    }

    private Button PadButton(string text, bool selected, Action click)
    {
        var button = new Button
        {
            Classes = { "pos_menu_button", "pos_menu_button_green", "bo_new_doc_pad_button" },
            Content = new TextBlock { Classes = { "pos_menu_caption" }, Text = text }
        };
        if (selected)
        {
            button.Classes.Add("pos_menu_button_selected");
        }

        button.Click += (_, _) => click();
        return button;
    }

    private IReadOnlyList<PosMenuItem> SubfamiliesForPad()
    {
        if (_padFamilyId is null)
        {
            return Array.Empty<PosMenuItem>();
        }

        return _posCatalog.Subfamilies.Where(item => item.ParentId == _padFamilyId.Value).ToList();
    }

    private IEnumerable<CatalogChoice> PadArticleChoices()
    {
        if (_padSubfamilyId is Guid subfamilyId)
        {
            return _catalog.Where(item => item.Article.SubfamilyId == subfamilyId);
        }

        var subfamilies = SubfamiliesForPad().Select(item => item.Id).ToHashSet();
        if (subfamilies.Count == 0)
        {
            return _catalog;
        }

        return _catalog.Where(item => subfamilies.Contains(item.Article.SubfamilyId));
    }

    private void OpenEditor(NewDocumentLine line)
    {
        _editing = line;
        _editSnapshot = LineSnapshot.From(line);
        _editorOpen = true;
        FillEditor(line.Code, line.Designation, line.Family, line.Price, line.Quantity, line.Discount, line.VatRateId, line.Notes);
        LineEditor.IsVisible = true;
    }

    private void FillEditor(
        string code,
        string designation,
        string family,
        decimal price,
        decimal quantity,
        decimal discount,
        Guid vatRateId,
        string? notes)
    {
        EditCode.Text = code;
        EditDesignation.Text = designation;
        EditFamily.Text = family;
        EditPrice.Text = price.ToString("0.##");
        EditQuantity.Text = quantity.ToString("0.##");
        EditDiscount.Text = discount.ToString("0.##");
        EditNotes.Text = notes ?? string.Empty;
        EditError.Text = string.Empty;
        var vatIndex = _vatRates.FindIndex(item => item.Id == vatRateId);
        EditVat.SelectedIndex = vatIndex >= 0 ? vatIndex : 0;
        RefreshEditTotals();
    }

    private void OnEditValuesChanged(object? sender, TextChangedEventArgs e)
    {
        RefreshEditTotals();
    }

    private void OnEditVatChanged(object? sender, SelectionChangedEventArgs e)
    {
        RefreshEditTotals();
    }

    private void RefreshEditTotals()
    {
        if (_editing is null || EditTotal is null)
        {
            return;
        }

        var price = ParseDecimal(EditPrice.Text);
        var quantity = ParseDecimal(EditQuantity.Text);
        var discount = ParseDecimal(EditDiscount.Text);
        var vat = SelectedVat()?.Percentage ?? _editing.VatPercentage;
        var totals = NewDocumentLine.Totals(_editing.PriceIncludesVat, price, quantity, discount, vat);
        EditTotal.Text = Money(totals.Net);
        EditTotalVat.Text = Money(totals.Gross);
    }

    private void OnLineOk(object? sender, RoutedEventArgs e)
    {
        if (_editing is null)
        {
            CloseEditor();
            return;
        }

        var price = ParseDecimal(EditPrice.Text);
        var quantity = ParseDecimal(EditQuantity.Text);
        var discount = ParseDecimal(EditDiscount.Text);
        if (quantity <= 0)
        {
            EditError.Text = "A quantidade tem de ser maior que zero.";
            return;
        }

        if (discount < 0 || discount > 100)
        {
            EditError.Text = "O desconto tem de estar entre 0 e 100.";
            return;
        }

        var vat = SelectedVat();
        _editing.Apply(
            EditDesignation.Text?.Trim() ?? _editing.Designation,
            price,
            quantity,
            discount,
            vat?.Id ?? _editing.VatRateId,
            vat?.Percentage ?? _editing.VatPercentage,
            EditNotes.Text);
        CloseEditor();
        RefreshLines();
    }

    private void OnLineCancel(object? sender, RoutedEventArgs e)
    {
        CloseEditor();
    }

    private void OnLineRemove(object? sender, RoutedEventArgs e)
    {
        if (_editing is null)
        {
            return;
        }

        _lines.Remove(_editing);
        CloseEditor();
        RefreshLines();
    }

    private void OnLineClear(object? sender, RoutedEventArgs e)
    {
        if (_editing is null || _editSnapshot is null)
        {
            return;
        }

        FillEditor(
            _editSnapshot.Code,
            _editSnapshot.Designation,
            _editSnapshot.Family,
            _editSnapshot.Price,
            _editSnapshot.Quantity,
            _editSnapshot.Discount,
            _editSnapshot.VatRateId,
            _editSnapshot.Notes);
    }

    private void CloseEditor()
    {
        LineEditor.IsVisible = false;
        _editorOpen = false;
        _editing = null;
        LinesGrid.SelectedItem = null;
    }

    private void RefreshLines()
    {
        LinesGrid.ItemsSource = null;
        LinesGrid.ItemsSource = _lines.ToList();
        LinesGrid.SelectedItem = null;
        RefreshTotal();
    }

    private void RefreshTotal()
    {
        TotalLabel.Text = "Total: " + Money(_lines.Sum(line => line.GrossTotal));
    }

    private PosVatOption? SelectedVat()
    {
        return EditVat.SelectedItem as PosVatOption;
    }

    private static Guid? SelectedId(ComboBox box)
    {
        return box.SelectedItem is PosLookupItem item && item.Id != Guid.Empty ? item.Id : null;
    }

    private static decimal ParseDecimal(string? text)
    {
        return decimal.TryParse(text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out var value)
            ? value
            : 0m;
    }

    private static string Money(decimal value)
    {
        return value.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("pt-PT"));
    }

    private string FamilyNameOf(PosArticle article)
    {
        return _catalog.FirstOrDefault(item => item.Article.Id == article.Id)?.Family ?? string.Empty;
    }

    private void ApplyFilter()
    {
        _matches.Clear();
        ArticleSuggestList.Children.Clear();
        var term = ArticleSearch.Text?.Trim();
        if (string.IsNullOrEmpty(term))
        {
            ArticleSuggest.IsVisible = false;
            return;
        }

        _matches.AddRange(_catalog.Where(item => item.SearchText.Contains(term, StringComparison.OrdinalIgnoreCase)).Take(8));
        foreach (var choice in _matches)
        {
            var match = choice;
            var button = new Button
            {
                Classes = { "bo_report_row" },
                Content = $"{match.Code}  {match.Designation}",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
            };
            button.Click += (_, _) => AddChoice(match);
            ArticleSuggestList.Children.Add(button);
        }

        ArticleSuggest.IsVisible = _matches.Count > 0;
    }

    private static string FamilyName(PosCatalog catalog, PosArticle article)
    {
        var subfamily = catalog.Subfamilies.FirstOrDefault(item => item.Id == article.SubfamilyId);
        if (subfamily is null)
        {
            return string.Empty;
        }

        var family = catalog.Families.FirstOrDefault(item => item.Id == subfamily.ParentId);
        return family is null ? subfamily.Text : $"{family.Text} / {subfamily.Text}";
    }

    private async void OnPreviewClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            await IssueAsync(draft: true, close: false);
        }
        catch (Exception exception)
        {
            ErrorLabel.Text = exception.Message;
        }
    }

    private void OnCustomerEdited(object? sender, TextChangedEventArgs e)
    {
        if (_filling || _customerLock > 0)
        {
            return;
        }

        _customerId = null;
        _customer = null;
        _customerIsFinal = true;
        CustomerEditButton.IsVisible = false;
        _ = SuggestCustomersAsync(sender is TextBox box ? box.Text : CustomerName.Text);
    }

    private void ShowCustomer(PosCustomer customer)
    {
        var wasFilling = _filling;
        _filling = true;
        _customerLock++;
        _customerId = customer.Id;
        _customer = customer;
        _customerIsFinal = customer.IsFinalConsumer;
        CustomerName.Text = customer.Name;
        CustomerFiscal.Text = customer.FiscalNumber;
        if (customer.IsFinalConsumer == false && string.IsNullOrWhiteSpace(customer.Address) == false)
        {
            ApplyCustomerAddress();
        }

        _filling = wasFilling;
        CustomerEditButton.IsVisible = customer.IsFinalConsumer == false;
        HideCustomerSuggest();
        Dispatcher.UIThread.Post(() => _customerLock--, DispatcherPriority.Background);
    }

    private bool ShowsFinalConsumer()
    {
        return _customerIsFinal
            || string.Equals(CustomerName.Text?.Trim(), "Consumidor Final", StringComparison.OrdinalIgnoreCase);
    }

    private void ClearCustomer()
    {
        _customerLock++;
        _customerId = null;
        _customer = null;
        _customerIsFinal = true;
        CustomerName.Text = string.Empty;
        CustomerFiscal.Text = string.Empty;
        CustomerEditButton.IsVisible = false;
        HideCustomerSuggest();
        Dispatcher.UIThread.Post(() => _customerLock--, DispatcherPriority.Background);
    }

    private void HideCustomerSuggest()
    {
        _customerMatches.Clear();
        CustomerSuggestList.Children.Clear();
        CustomerSuggest.IsVisible = false;
    }

    private async Task SuggestCustomersAsync(string? text)
    {
        var term = text?.Trim() ?? string.Empty;
        var ticket = ++_customerSearch;
        if (term.Length == 0)
        {
            HideCustomerSuggest();
            return;
        }

        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        var matches = await services.GetRequiredService<IPosCustomerService>().SearchAsync(term);
        if (ticket != _customerSearch)
        {
            return;
        }

        _customerMatches.Clear();
        CustomerSuggestList.Children.Clear();
        _customerMatches.AddRange(matches.Take(8));
        foreach (var match in _customerMatches)
        {
            var customer = match;
            var button = new Button
            {
                Classes = { "bo_report_row" },
                Content = $"{customer.Name}  {customer.FiscalNumber}",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
            };
            button.Click += (_, _) => ShowCustomer(customer);
            CustomerSuggestList.Children.Add(button);
        }

        CustomerSuggest.IsVisible = _customerMatches.Count > 0;
    }

    private async void OnCustomerSearchKey(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        if (_customerMatches.Count > 0)
        {
            ShowCustomer(_customerMatches[0]);
            return;
        }

        var text = sender is TextBox box ? box.Text : CustomerName.Text;
        await SearchCustomersAsync(text);
    }

    private async void OnCustomerSearchClick(object? sender, RoutedEventArgs e)
    {
        var text = string.IsNullOrWhiteSpace(CustomerName.Text) ? CustomerFiscal.Text : CustomerName.Text;
        await SearchCustomersAsync(text);
    }

    private async Task SearchCustomersAsync(string? text)
    {
        var services = AppComposition.Services;
        if (services is null || string.IsNullOrWhiteSpace(text))
        {
            ErrorLabel.Text = "Indique o nome, o NIF ou o cartão do cliente.";
            return;
        }

        var matches = await services.GetRequiredService<IPosCustomerService>().SearchAsync(text);
        if (matches.Count == 0)
        {
            ErrorLabel.Text = "Não foi encontrado nenhum cliente.";
            return;
        }

        ErrorLabel.Text = string.Empty;
        if (matches.Count == 1)
        {
            ShowCustomer(matches[0]);
            return;
        }

        CustomerList.Children.Clear();
        foreach (var match in matches)
        {
            var customer = match;
            var button = new Button
            {
                Classes = { "pos_list_item" },
                Content = $"{customer.Name}  {customer.FiscalNumber}"
            };
            button.Click += (_, _) =>
            {
                ShowCustomer(customer);
                CustomerOverlay.IsVisible = false;
            };
            CustomerList.Children.Add(button);
        }

        CustomerOverlay.IsVisible = true;
    }

    private void OnCustomerListCloseClick(object? sender, RoutedEventArgs e) => CustomerOverlay.IsVisible = false;

    private async void OnClearCustomerClick(object? sender, RoutedEventArgs e)
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        var customer = await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync();
        if (customer is null)
        {
            _customerId = null;
            _customer = null;
            CustomerName.Text = string.Empty;
            CustomerFiscal.Text = string.Empty;
            ErrorLabel.Text = "Cliente consumidor final não encontrado.";
            return;
        }

        ShowCustomer(customer);
        ErrorLabel.Text = string.Empty;
    }

    private async void OnNewCustomerClick(object? sender, RoutedEventArgs e)
    {
        await OpenCustomerEditorAsync(create: true);
    }

    private async void OnEditCustomerClick(object? sender, RoutedEventArgs e)
    {
        if (_customerId is null)
        {
            return;
        }

        await OpenCustomerEditorAsync(create: false);
    }

    private async Task OpenCustomerEditorAsync(bool create)
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        await EnsureCustomerLookupsAsync(services);
        PosCustomer? customer = null;
        if (create == false && _customerId is Guid id)
        {
            customer = await services.GetRequiredService<IPosCustomerService>().FindAsync(id);
            if (customer is null)
            {
                ErrorLabel.Text = "Não foi possível abrir o cliente.";
                return;
            }
        }

        _filling = true;
        _editorCustomerId = customer?.Id;
        CustomerEditorTitle.Text = create ? "Novo cliente" : "Editar cliente";
        EditorName.Text = customer?.Name ?? CustomerName.Text ?? string.Empty;
        EditorFiscal.Text = customer?.FiscalNumber ?? CustomerFiscal.Text ?? string.Empty;
        EditorCard.Text = customer?.CardNumber ?? string.Empty;
        EditorDiscount.Text = (customer?.Discount ?? 0).ToString("0.00");
        EditorAddress.Text = customer?.Address ?? string.Empty;
        EditorLocality.Text = customer?.Locality ?? string.Empty;
        EditorZip.Text = customer?.ZipCode ?? string.Empty;
        EditorCity.Text = customer?.City ?? string.Empty;
        EditorPhone.Text = customer?.Phone ?? string.Empty;
        EditorEmail.Text = customer?.Email ?? string.Empty;
        EditorNotes.Text = customer?.Notes ?? string.Empty;
        SelectLookup(EditorType, _customerTypes, customer?.CustomerTypeId);
        SelectLookup(EditorPrice, _priceTypes, customer?.PriceTypeId);
        SelectLookup(EditorCountry, _countries, customer?.CountryId, customer?.Country ?? "Portugal");
        EditorError.Text = string.Empty;
        _filling = false;
        ErrorLabel.Text = string.Empty;
        CustomerEditor.IsVisible = true;
    }

    private async Task EnsureCustomerLookupsAsync(IServiceProvider services)
    {
        var listings = services.GetRequiredService<IBackOfficeListingService>();
        _customerTypes.Clear();
        _customerTypes.AddRange(await listings.LookupAsync("Tipo de clientes"));
        _priceTypes.Clear();
        _priceTypes.AddRange(await listings.LookupAsync("Tipo de Preço"));
        _countries.Clear();
        _countries.AddRange(await listings.LookupAsync("País"));
        EditorType.ItemsSource = _customerTypes.Select(item => item.Label).ToList();
        EditorPrice.ItemsSource = _priceTypes.Select(item => item.Label).ToList();
        EditorCountry.ItemsSource = _countries.Select(item => item.Label).ToList();
    }

    private static void SelectLookup(ComboBox box, IReadOnlyList<LookupOption> items, Guid? id, string? label = null)
    {
        var index = id is Guid value && value != Guid.Empty
            ? items.ToList().FindIndex(item => item.Id == value)
            : -1;
        if (index < 0 && string.IsNullOrWhiteSpace(label) == false)
        {
            index = items.ToList().FindIndex(item => item.Label.Contains(label, StringComparison.OrdinalIgnoreCase));
        }

        box.SelectedIndex = index >= 0 ? index : (items.Count > 0 ? 0 : -1);
    }

    private void OnCustomerEditorCancel(object? sender, RoutedEventArgs e) => CustomerEditor.IsVisible = false;

    private async void OnCustomerEditorOk(object? sender, RoutedEventArgs e)
    {
        if (decimal.TryParse(EditorDiscount.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var discount) == false
            && decimal.TryParse(EditorDiscount.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out discount) == false)
        {
            EditorError.Text = "O desconto é inválido.";
            return;
        }

        var customerFields = new Dictionary<string, string>
        {
            ["Name"] = EditorName.Text?.Trim() ?? string.Empty,
            ["FiscalNumber"] = EditorFiscal.Text?.Trim() ?? string.Empty,
            ["CardNumber"] = EditorCard.Text?.Trim() ?? string.Empty,
            ["Discount"] = EditorDiscount.Text?.Trim() ?? string.Empty,
            ["Address"] = EditorAddress.Text?.Trim() ?? string.Empty,
            ["Locality"] = EditorLocality.Text?.Trim() ?? string.Empty,
            ["ZipCode"] = EditorZip.Text?.Trim() ?? string.Empty,
            ["City"] = EditorCity.Text?.Trim() ?? string.Empty,
            ["Phone"] = EditorPhone.Text?.Trim() ?? string.Empty,
            ["Email"] = EditorEmail.Text?.Trim() ?? string.Empty
        };
        var invalidCustomer = GtkFormRules.Validate("Clientes", customerFields);
        if (invalidCustomer is not null)
        {
            EditorError.Text = invalidCustomer;
            return;
        }

        var typeId = LookupId(EditorType, _customerTypes);
        var priceId = LookupId(EditorPrice, _priceTypes);
        var countryId = LookupId(EditorCountry, _countries);
        if (typeId is null)
        {
            EditorError.Text = "Não há tipos de cliente na API.";
            return;
        }

        if (priceId is null)
        {
            EditorError.Text = "Não há tipos de preço na API.";
            return;
        }

        if (countryId is null)
        {
            EditorError.Text = "Não há países na API.";
            return;
        }

        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        var saved = await services.GetRequiredService<IBackOfficeListingService>().SaveAsync(
            "Clientes",
            _editorCustomerId,
            new Dictionary<string, string>
            {
                ["Name"] = customerFields["Name"],
                ["FiscalNumber"] = customerFields["FiscalNumber"],
                ["CardNumber"] = customerFields["CardNumber"],
                ["Discount"] = discount.ToString(CultureInfo.InvariantCulture),
                ["Address"] = customerFields["Address"],
                ["Locality"] = customerFields["Locality"],
                ["ZipCode"] = customerFields["ZipCode"],
                ["City"] = customerFields["City"],
                ["Phone"] = customerFields["Phone"],
                ["Email"] = customerFields["Email"],
                ["Notes"] = EditorNotes.Text?.Trim() ?? string.Empty,
                ["CustomerTypeId"] = typeId.Value.ToString(),
                ["PriceTypeId"] = priceId.Value.ToString(),
                ["CountryId"] = countryId.Value.ToString()
            });
        if (saved.Succeeded == false)
        {
            EditorError.Text = saved.Error ?? "Não foi possível gravar o cliente.";
            return;
        }

        PosCustomer? customer = null;
        if (saved.Id != Guid.Empty)
        {
            try
            {
                customer = await services.GetRequiredService<IPosCustomerService>().FindAsync(saved.Id);
            }
            catch (Exception)
            {
                customer = null;
            }
        }

        customer ??= saved.Id == Guid.Empty
            ? null
            : new PosCustomer(
                saved.Id,
                customerFields["Name"],
                customerFields["FiscalNumber"],
                customerFields["CardNumber"],
                discount,
                customerFields["Address"],
                customerFields["Locality"],
                customerFields["ZipCode"],
                customerFields["City"],
                "Portugal",
                false,
                customerFields["Phone"],
                customerFields["Email"],
                EditorNotes.Text?.Trim());
        if (customer is null)
        {
            EditorError.Text = "O cliente foi gravado, mas não foi possível selecioná-lo.";
            return;
        }

        ShowCustomer(customer);
        CustomerEditor.IsVisible = false;
    }

    private static Guid? LookupId(ComboBox box, IReadOnlyList<LookupOption> items)
    {
        var index = box.SelectedIndex;
        return index >= 0 && index < items.Count ? items[index].Id : null;
    }

    private async void OnIssueClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            await IssueAsync(draft: DraftBox.IsChecked == true, close: true);
        }
        catch (Exception exception)
        {
            ErrorLabel.Text = exception.Message;
        }
    }

    private async Task IssueAsync(bool draft, bool close)
    {
        var type = SelectedType();
        var services = AppComposition.Services;
        if (type is null || services is null)
        {
            return;
        }

        if (_lines.Count == 0)
        {
            ErrorLabel.Text = "Adicione artigos ao documento.";
            return;
        }

        var transport = RequiresTransport();
        if (transport && draft == false)
        {
            var transportError = PosDocumentRules.ValidateTransport(ReadDestination(), ReadOrigin());
            if (transportError is not null)
            {
                ErrorLabel.Text = transportError;
                return;
            }
        }

        if (_customerId is not Guid customerId)
        {
            ErrorLabel.Text = "Escolha ou crie um cliente.";
            return;
        }

        if (draft == false
            && PosDocumentRules.RequiresNamedCustomer(type.Acronym)
            && (_customerIsFinal || string.IsNullOrWhiteSpace(CustomerName.Text) || string.IsNullOrWhiteSpace(CustomerFiscal.Text)))
        {
            ErrorLabel.Text = "Não pode criar faturas com o cliente consumidor final ou com um cliente sem nome/NIF. Use um cliente válido e tente novamente.";
            return;
        }

        var documents = services.GetRequiredService<IPosDocumentService>();
        if (close == false && _previewDraftId is Guid previous)
        {
            await documents.DeleteDraftAsync(previous);
            _previewDraftId = null;
        }

        var saleLines = _lines.Select(line => new PosSaleLine(
            line.ArticleId,
            line.Quantity,
            line.NetUnitPrice,
            line.Discount,
            line.VatRateId,
            line.Notes)).ToList();
        var header = new PosDocumentHeader
        {
            PaymentConditionId = SelectedId(PaymentBox),
            CurrencyId = SelectedId(CurrencyBox),
            ParentDocumentId = SelectedId(OriginBox),
            Notes = NotesBox.Text,
            IsDraft = draft,
            ShipTo = transport ? ReadDestination() : null,
            ShipFrom = transport ? ReadOrigin() : null
        };
        var issued = await documents.IssueDocumentAsync(type.Acronym, customerId, saleLines, header);
        if (issued.Succeeded == false || issued.DocumentId == Guid.Empty)
        {
            if (draft)
            {
                var draftId = SaveLocalDraft(type, customerId, _lines.Sum(line => line.GrossTotal));
                _previewDraftId = draftId;
                if (close == false)
                {
                    var preview = await WriteLocalPreviewAsync(documents);
                    if (preview is not null && PreviewRequested is not null)
                    {
                        ErrorLabel.Text = string.Empty;
                        await PreviewRequested(preview, "Pré-visualização");
                    }

                    return;
                }

                ErrorLabel.Text = string.Empty;
                _closed?.TrySetResult(true);
                return;
            }

            if (close == false && SeriesNotCommunicated(issued.Error))
            {
                var preview = await WriteLocalPreviewAsync(documents);
                if (preview is not null && PreviewRequested is not null)
                {
                    ErrorLabel.Text = string.Empty;
                    await PreviewRequested(preview, "Pré-visualização");
                    return;
                }
            }

            ErrorLabel.Text = issued.Error ?? "Não foi possível emitir o documento.";
            return;
        }

        if (_previewDraftId is Guid previousLocal)
        {
            LocalDraftStore.Delete(previousLocal);
        }

        if (_seedDraftId is Guid seededLocal)
        {
            LocalDraftStore.Delete(seededLocal);
        }

        if (close && _previewDraftId is Guid leftover && leftover != issued.DocumentId)
        {
            await documents.DeleteDraftAsync(leftover);
            _previewDraftId = null;
        }

        if (close == false)
        {
            _previewDraftId = issued.DocumentId;
        }

        var path = await documents.CreateA4FileAsync(issued.DocumentId);
        if (close)
        {
            CreatedPdfPath = path;
            CreatedPdfTitle = issued.Number;
            _closed?.TrySetResult(true);
            return;
        }

        ErrorLabel.Text = string.Empty;
        if (path is not null && PreviewRequested is not null)
        {
            await PreviewRequested(path, "Pré-visualização");
        }
    }

    private void OnDraftChanged(object? sender, RoutedEventArgs e)
    {
        RefreshIssueLabel();
    }

    private void RefreshIssueLabel()
    {
        IssueLabel.Text = DraftBox.IsChecked == true ? "Gravar" : "Emitir A4";
    }

    private Guid SaveLocalDraft(PosDocumentTypeOption type, Guid customerId, decimal total)
    {
        var existing = _previewDraftId ?? _seedDraftId;
        var kept = existing is Guid id && LocalDraftStore.Find(id) is not null ? id : Guid.Empty;
        var transport = RequiresTransport();
        return LocalDraftStore.Save(new StoredDraft
        {
            Id = kept,
            Acronym = type.Acronym,
            CustomerId = customerId,
            CustomerName = CustomerName.Text?.Trim() ?? string.Empty,
            FiscalNumber = CustomerFiscal.Text?.Trim() ?? string.Empty,
            Notes = NotesBox.Text,
            PaymentConditionId = SelectedId(PaymentBox),
            CurrencyId = SelectedId(CurrencyBox),
            ParentDocumentId = SelectedId(OriginBox),
            Total = total,
            ShipTo = transport ? ReadDestination() : null,
            ShipFrom = transport ? ReadOrigin() : null,
            Lines = _lines.Select(line => new StoredDraftLine
            {
                ArticleId = line.ArticleId,
                Code = line.Code,
                Designation = line.Designation,
                Quantity = line.Quantity,
                Price = line.Price,
                Discount = line.Discount,
                VatRateId = line.VatRateId,
                VatPercentage = line.VatPercentage,
                Notes = line.Notes
            }).ToList()
        });
    }

    private static void SelectId(ComboBox box, Guid? id)
    {
        if (id is null || id == Guid.Empty || box.ItemsSource is not IEnumerable<PosLookupItem> items)
        {
            return;
        }

        var match = items.FirstOrDefault(item => item.Id == id);
        if (match is not null)
        {
            box.SelectedItem = match;
        }
    }

    private static void ApplyStoredShip(
        PosShipAddress? address,
        TextBox addressBox,
        TextBox region,
        TextBox postal,
        TextBox city,
        TextBox country,
        CalendarDatePicker date,
        TimePicker time)
    {
        if (address is null)
        {
            return;
        }

        FillShip(addressBox, region, postal, city, country, address.Address, address.Region, address.PostalCode, address.City, address.Country);
        if (DateTime.TryParse(address.When, out var when))
        {
            SetWhen(date, time, when);
        }
    }

    private static bool SeriesNotCommunicated(string? message)
    {
        return message?.Contains("não foi comunicada", StringComparison.OrdinalIgnoreCase) == true;
    }

    private async Task<string?> WriteLocalPreviewAsync(IPosDocumentService documents)
    {
        var type = SelectedType();
        if (type is null)
        {
            return null;
        }

        var company = await documents.GetCompanyAddressAsync();
        var currency = CurrencyBox.SelectedItem is PosLookupItem selected ? selected.Code : "EUR";
        var transport = RequiresTransport();
        return DocumentPreviewFile.Write(new DocumentPreview
        {
            Acronym = type.Acronym,
            TypeName = type.Designation,
            CustomerName = CustomerName.Text ?? string.Empty,
            FiscalNumber = CustomerFiscal.Text ?? string.Empty,
            Address = _customer?.Address,
            City = _customer?.City,
            PostalCode = _customer?.ZipCode,
            Country = _customer?.Country,
            Notes = NotesBox.Text,
            PaymentCondition = PaymentBox.SelectedItem?.ToString(),
            Currency = string.IsNullOrWhiteSpace(currency) ? "EUR" : currency,
            CompanyAddress = company.Address,
            CompanyCity = company.City,
            CompanyPostalCode = company.PostalCode,
            CompanyCountry = company.Country,
            ShipTo = transport ? PosDocumentRules.ToAddress(ReadDestination()) : null,
            ShipFrom = transport ? PosDocumentRules.ToAddress(ReadOrigin()) : null,
            Lines = _lines.Select(line =>
            {
                var totals = NewDocumentLine.Totals(line.PriceIncludesVat, line.Price, line.Quantity, line.Discount, line.VatPercentage);
                return new DocumentPreviewLine
                {
                    Code = line.Code,
                    Designation = line.Designation,
                    Quantity = line.Quantity,
                    NetUnitPrice = line.NetUnitPrice,
                    Discount = line.Discount,
                    VatPercentage = line.VatPercentage,
                    TotalNet = totals.Net,
                    TotalFinal = totals.Gross,
                    Notes = line.Notes
                };
            }).ToList()
        });
    }

    private bool RequiresTransport()
    {
        var acronym = SelectedType()?.Acronym;
        return PosDocumentRules.IsWayBill(acronym)
            || (PosDocumentRules.SupportsOptionalTransport(acronym) && TransportBox.IsChecked == true);
    }

    private void OnUseCustomerAddress(object? sender, RoutedEventArgs e)
    {
        if (_customer is null || _customer.IsFinalConsumer || string.IsNullOrWhiteSpace(_customer.Address))
        {
            ErrorLabel.Text = "O cliente seleccionado não tem morada.";
            return;
        }

        ApplyCustomerAddress();
        ErrorLabel.Text = string.Empty;
    }

    private void OnUseCompanyAddress(object? sender, RoutedEventArgs e)
    {
        FillShip(
            FromAddress,
            FromRegion,
            FromPostal,
            FromCity,
            FromCountry,
            string.IsNullOrWhiteSpace(_company.Address) ? PosDocumentRules.Unknown : _company.Address,
            PosDocumentRules.Unknown,
            string.IsNullOrWhiteSpace(_company.PostalCode) ? "0000-000" : _company.PostalCode,
            string.IsNullOrWhiteSpace(_company.City) ? PosDocumentRules.Unknown : _company.City,
            string.IsNullOrWhiteSpace(_company.Country) ? "Portugal" : _company.Country);
        ErrorLabel.Text = string.Empty;
    }

    private void OnCopyDestinationToOrigin(object? sender, RoutedEventArgs e)
    {
        CopyShip(ToAddress, ToRegion, ToPostal, ToCity, ToCountry, ToDate, ToTime, FromAddress, FromRegion, FromPostal, FromCity, FromCountry, FromDate, FromTime);
    }

    private void ApplyCustomerAddress()
    {
        if (_customer is null)
        {
            return;
        }

        FillShip(
            ToAddress,
            ToRegion,
            ToPostal,
            ToCity,
            ToCountry,
            _customer.Address ?? string.Empty,
            string.IsNullOrWhiteSpace(_customer.Locality) ? PosDocumentRules.Unknown : _customer.Locality,
            string.IsNullOrWhiteSpace(_customer.ZipCode) ? "0000-000" : _customer.ZipCode,
            string.IsNullOrWhiteSpace(_customer.City) ? PosDocumentRules.Unknown : _customer.City,
            string.IsNullOrWhiteSpace(_customer.Country) ? "Portugal" : _customer.Country);
    }

    private static void CopyShip(
        TextBox fromAddress,
        TextBox fromRegion,
        TextBox fromPostal,
        TextBox fromCity,
        TextBox fromCountry,
        CalendarDatePicker fromDate,
        TimePicker fromTime,
        TextBox toAddress,
        TextBox toRegion,
        TextBox toPostal,
        TextBox toCity,
        TextBox toCountry,
        CalendarDatePicker toDate,
        TimePicker toTime)
    {
        FillShip(toAddress, toRegion, toPostal, toCity, toCountry, fromAddress.Text, fromRegion.Text, fromPostal.Text, fromCity.Text, fromCountry.Text);
        toDate.SelectedDate = fromDate.SelectedDate;
        toTime.SelectedTime = fromTime.SelectedTime;
    }

    private static void FillShip(TextBox address, TextBox region, TextBox postal, TextBox city, TextBox country, string? addressText, string? regionText, string? postalText, string? cityText, string? countryText)
    {
        address.Text = addressText ?? string.Empty;
        region.Text = regionText ?? string.Empty;
        postal.Text = postalText ?? string.Empty;
        city.Text = cityText ?? string.Empty;
        country.Text = countryText ?? string.Empty;
    }

    private static void SetWhen(CalendarDatePicker date, TimePicker time, DateTime value)
    {
        date.SelectedDate = value.Date;
        time.SelectedTime = value.TimeOfDay;
    }

    private static string FormatWhen(CalendarDatePicker date, TimePicker time)
    {
        var day = date.SelectedDate?.Date ?? DateTime.Today;
        var clock = time.SelectedTime ?? TimeSpan.Zero;
        return day.Add(clock).ToString("yyyy-MM-ddTHH:mm:ss");
    }

    private PosShipAddress ReadDestination()
    {
        return ReadShip(ToAddress, ToRegion, ToPostal, ToCity, ToCountry, ToDate, ToTime);
    }

    private PosShipAddress ReadOrigin()
    {
        return ReadShip(FromAddress, FromRegion, FromPostal, FromCity, FromCountry, FromDate, FromTime);
    }

    private static PosShipAddress ReadShip(TextBox address, TextBox region, TextBox postal, TextBox city, TextBox country, CalendarDatePicker date, TimePicker time)
    {
        return new PosShipAddress
        {
            Address = address.Text,
            Region = region.Text,
            PostalCode = postal.Text,
            City = city.Text,
            Country = country.Text,
            When = FormatWhen(date, time)
        };
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        _closed?.TrySetResult(false);
    }

    private PosDocumentTypeOption? SelectedType()
    {
        var index = TypeBox.SelectedIndex;
        return index >= 0 && index < _types.Count ? _types[index] : null;
    }

    private sealed class CatalogChoice
    {
        public CatalogChoice(PosArticle article, string family)
        {
            Article = article;
            Family = family;
            SearchText = $"{article.Code} {article.Designation} {article.Text} {family}";
        }

        public PosArticle Article { get; }

        public string Code => Article.Code;

        public string Designation => Article.Designation;

        public string Family { get; }

        public string SearchText { get; }
    }

    private sealed class LineSnapshot
    {
        public required string Code { get; init; }

        public required string Designation { get; init; }

        public required string Family { get; init; }

        public decimal Price { get; init; }

        public decimal Quantity { get; init; }

        public decimal Discount { get; init; }

        public Guid VatRateId { get; init; }

        public string? Notes { get; init; }

        public static LineSnapshot From(NewDocumentLine line)
        {
            return new LineSnapshot
            {
                Code = line.Code,
                Designation = line.Designation,
                Family = line.Family,
                Price = line.Price,
                Quantity = line.Quantity,
                Discount = line.Discount,
                VatRateId = line.VatRateId,
                Notes = line.Notes
            };
        }
    }

    private sealed class NewDocumentLine
    {
        public NewDocumentLine(PosArticle article, string family)
        {
            ArticleId = article.Id;
            VatRateId = article.VatRateId;
            Code = article.Code;
            Designation = article.Designation;
            Family = family;
            Quantity = article.QuantityStep;
            Price = article.CatalogPrice;
            Discount = article.Discount;
            PriceIncludesVat = article.PriceIncludesVat;
            VatPercentage = article.VatPercentage;
            RecalculateNet();
        }

        private NewDocumentLine(PosCopiedLine line, string family, bool priceIncludesVat)
        {
            ArticleId = line.ArticleId;
            VatRateId = line.VatRateId;
            Code = line.Code;
            Designation = line.Designation;
            Family = family;
            Quantity = line.Quantity;
            Price = line.Price;
            Discount = line.Discount;
            PriceIncludesVat = priceIncludesVat;
            VatPercentage = line.VatPercentage;
            RecalculateNet();
        }

        public static NewDocumentLine FromCopy(PosCopiedLine line, string family)
        {
            return new NewDocumentLine(line, family, false);
        }

        public Guid ArticleId { get; }

        public Guid VatRateId { get; private set; }

        public string Code { get; }

        public string Designation { get; private set; }

        public string Family { get; }

        public decimal Quantity { get; private set; }

        public decimal Price { get; private set; }

        public decimal Discount { get; private set; }

        public decimal NetUnitPrice { get; private set; }

        public bool PriceIncludesVat { get; }

        public decimal VatPercentage { get; private set; }

        public string? Notes { get; private set; }

        public decimal GrossTotal => Totals(PriceIncludesVat, Price, Quantity, Discount, VatPercentage).Gross;

        public string QuantityText => Quantity.ToString("0.##");

        public string PriceText => Price.ToString("N2");

        public string DiscountText => Discount.ToString("0.##");

        public string TotalText => GrossTotal.ToString("N2");

        public void IncreaseQuantity(decimal step)
        {
            Quantity += step > 0 ? step : 1m;
        }

        public void Apply(string designation, decimal price, decimal quantity, decimal discount, Guid vatRateId, decimal vatPercentage, string? notes)
        {
            Designation = string.IsNullOrWhiteSpace(designation) ? Designation : designation;
            Price = price;
            Quantity = quantity;
            Discount = discount;
            VatRateId = vatRateId;
            VatPercentage = vatPercentage;
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            RecalculateNet();
        }

        public static (decimal Net, decimal Gross) Totals(bool priceIncludesVat, decimal price, decimal quantity, decimal discount, decimal vatPercentage)
        {
            var netUnit = PosTicketLine.ToNetUnitPrice(price, vatPercentage, priceIncludesVat);
            var net = quantity * netUnit * (1m - discount / 100m);
            var gross = priceIncludesVat
                ? quantity * price * (1m - discount / 100m)
                : net * (1m + vatPercentage / 100m);
            return (net, gross);
        }

        private void RecalculateNet()
        {
            NetUnitPrice = PosTicketLine.ToNetUnitPrice(Price, VatPercentage, PriceIncludesVat);
        }
    }
}
