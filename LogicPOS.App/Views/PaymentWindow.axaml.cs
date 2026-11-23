using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using LogicPOS.App.Hardware;
using LogicPOS.Application.Features.System;
using LogicPOS.App.Hardware;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Fiscal;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class PaymentWindow : Window
{
    private const string CashToken = "MONEY";

    private readonly decimal _orderTotal;
    private readonly IReadOnlyList<PosTicketLine> _lines;
    private readonly PosCustomer? _preferredCustomer;
    private readonly List<Button> _methodButtons = new();
    private readonly Dictionary<string, Bitmap> _bitmaps = new();

    private Button? _customerCard;
    private string? _methodToken;
    private bool _cash;
    private decimal _delivered;
    private bool _cashTyping;
    private decimal _cashMoney;
    private bool _filling;
    private bool _confirmPayment;
    private bool _finalConsumer = true;
    private Guid? _customerId;
    private string _defaultCountry = string.Empty;
    private readonly string? _shareLabel;
    private string _saleDocumentType = "FS";
    private int _customerSearch;
    private readonly List<PosCustomer> _customerMatches = new();
    private string? _pendingSeriesDocumentType;
    private IReadOnlyList<PosTicketLine>? _pendingSeriesLines;
    private IReadOnlyList<PartialPaidLine>? _pendingSeriesPartial;
    private bool _pendingSettledSale = true;
    private readonly List<PartialRow> _partialRows = new();
    private CompanyFiscalMarkState _companyMarks = CompanyFiscalMarkState.None;
    private bool _parkingInvoiceRequested;
    private readonly List<MixedPaymentRow> _mixedRows = new();
    private readonly List<(string Token, string Label)> _posPaymentMethods =
    [
        ("MONEY", "Numerário"),
        ("CASH_MACHINE", "MB Way"),
        ("DEBIT_CARD", "Cartão de Débito"),
        ("BANK_CHECK", "Cheque"),
        ("CREDIT_CARD", "Cartão de Crédito"),
        ("CURRENT_ACCOUNT", "Conta Corrente")
    ];

    private bool _mixedPayment;
    private Dictionary<string, Guid> _paymentMethodIds = new(StringComparer.OrdinalIgnoreCase);
    private bool _issuing;
    private bool _closingWithResult;

    public bool PartialPayment { get; private set; }

    public IReadOnlyList<PartialPaidLine> PaidLines { get; private set; } = [];

    public IReadOnlyList<int> PaidLineIndexes => PaidLines.Select(line => line.Index).ToList();

    public PaymentWindow()
        : this(Array.Empty<PosTicketLine>())
    {
    }

    public PaymentWindow(IReadOnlyList<PosTicketLine> lines, PosCustomer? customer = null, string? shareLabel = null)
    {
        _lines = lines;
        _preferredCustomer = customer;
        _shareLabel = shareLabel;
        _orderTotal = lines.Sum(line => line.Total);
        InitializeComponent();
        CustomerSuggest.PlacementTarget = CustomerName;
        if (ZipCode.Parent is Panel zipHost)
        {
            PostalZipSuggest.Attach(ZipCode, zipHost, PostalZipSuggest.SearchCompanyAsync, ApplyPaymentPostal);
        }

        if (Address.Parent is Panel addressHost)
        {
            PostalZipSuggest.Attach(Address, addressHost, PostalZipSuggest.SearchCompanyAsync, ApplyPaymentPostal);
        }

        if (Locality.Parent is Panel localityHost)
        {
            PostalZipSuggest.Attach(Locality, localityHost, PostalZipSuggest.SearchCompanyAsync, ApplyPaymentPostal);
        }

        if (City.Parent is Panel cityHost)
        {
            PostalZipSuggest.Attach(City, cityHost, PostalZipSuggest.SearchCompanyAsync, ApplyPaymentPostal);
        }
        ListingNotice.BindToast(CustomerSearchNotice, this);
        ApplySaleDocumentType();
        BuildMethods();
        FullPaymentButton.Click += OnFullPaymentClick;
        TouchFields.Attach(this);
        // Block Alt+F4 while issuing; after success always Close(true) so PosWindow clears the order.
        Closing += (_, args) =>
        {
            if (PaymentWindowCloseDecision.ShouldCancelWhileIssuing(_issuing, _confirmPayment))
            {
                args.Cancel = true;
                return;
            }

            if (PaymentWindowCloseDecision.ShouldForceSuccessClose(_confirmPayment, _closingWithResult))
            {
                args.Cancel = true;
                _closingWithResult = true;
                Close(true);
            }
        };
        KeyDown += OnWindowKeyDown;
        Opened += (_, _) => CoverOwner();
        Opened += async (_, _) => await LoadCustomerAsync();
        Opened += async (_, _) => await LoadCompanyMarksAsync();
        RefreshTotals();
    }

    private async Task LoadCompanyMarksAsync()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        try
        {
            _companyMarks = await services.GetRequiredService<ICompanyFiscalMarkService>().LoadAsync();
        }
        catch
        {
            _companyMarks = CompanyFiscalMarkState.None;
        }
    }

    private void ApplyPaymentPostal(PostalLocalityMatch match)
        => PostalZipSuggest.Apply(match, ZipCode, Address, Locality, City);

    private void CoverOwner()
    {
        if (Owner is not Window owner)
        {
            return;
        }

        var topLeft = owner.PointToScreen(new Point(0, 0));
        Position = topLeft;
        Width = owner.Bounds.Width;
        Height = owner.Bounds.Height;
    }

    private void BuildMethods()
    {
        AddMethod("MONEY", "Numerário", "icon_pos_payment_type_money.png", 0, 0, 1);
        AddMethod("CASH_MACHINE", "MB Way", "icon_pos_payment_type_cash_machine.png", 1, 0, 1);
        AddMethod("DEBIT_CARD", "Cartão de Débito", "icon_pos_payment_type_debit_card.png", 2, 0, 1);
        AddMethod("BANK_CHECK", "Cheque", "icon_pos_payment_type_bank_check.png", 0, 1, 1);
        AddMethod("CREDIT_CARD", "Cartão de Crédito", "icon_pos_payment_type_credit_card.png", 1, 1, 1);
        AddMethod("CURRENT_ACCOUNT", "Conta Corrente", "icon_pos_payment_type_current_account.png", 2, 1, 1);
        _customerCard = AddMethod("CUSTOMER_CARD", "Cartão Cliente", "icon_pos_payment_type_customer_card.png", 0, 2, 3);
        _customerCard.Classes.Add("pos_pay_method_wide");
        SetEnabled(_customerCard, false);
    }

    private Button AddMethod(string token, string text, string iconFile, int column, int row, int columnSpan)
    {
        var button = new Button
        {
            Classes = { "pos_pay_method" },
            Content = new StackPanel
            {
                Spacing = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new Image
                    {
                        Classes = { "pos_pay_method_icon" },
                        Source = LoadBitmap($"avares://LogicPOS.App/Assets/Images/Pos/Payments/{iconFile}")
                    },
                    new TextBlock { Classes = { "pos_pay_method_text" }, Text = text }
                }
            }
        };
        button.Click += (_, _) => SelectMethod(button, token);
        Grid.SetColumn(button, column);
        Grid.SetRow(button, row);
        Grid.SetColumnSpan(button, columnSpan);
        MethodGrid.Children.Add(button);
        _methodButtons.Add(button);
        return button;
    }

    private void SelectMethod(Button button, string token)
    {
        if (token == "CUSTOMER_CARD" && string.IsNullOrWhiteSpace(CardNumber.Text))
        {
            return;
        }

        foreach (var method in _methodButtons)
        {
            method.Classes.Remove("selected");
            SetEnabled(method, method != _customerCard || HasCard());
        }

        button.Classes.Add("selected");
        _methodToken = token;
        _mixedPayment = false;
        _mixedRows.Clear();
        _cash = token == CashToken;

        if (_cash)
        {
            ShowCashPad();
            return;
        }

        _delivered = Payable;
        RefreshTotals();
    }

    private async Task LoadCustomerAsync()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        try
        {
            var customer = _preferredCustomer ?? await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync();
            if (customer is not null)
            {
                ShowCustomer(customer);
            }
        }
        catch
        {
            // The dialog still works with an empty customer when the lookup fails.
        }
    }

    private void ShowCustomer(PosCustomer customer)
    {
        _customerSearch++;
        HideCustomerSuggest();
        CustomerSearchNotice.IsVisible = false;
        _filling = true;
        _customerId = customer.Id;
        _finalConsumer = customer.IsFinalConsumer;
        _defaultCountry = string.IsNullOrWhiteSpace(customer.Country) ? "Portugal" : customer.Country;
        CustomerName.Text = customer.Name;
        FiscalNumber.Text = customer.FiscalNumber;
        CardNumber.Text = customer.CardNumber ?? string.Empty;
        Discount.Text = customer.Discount.ToString("0.00");
        Address.Text = customer.Address ?? string.Empty;
        Locality.Text = customer.Locality ?? string.Empty;
        ZipCode.Text = customer.ZipCode ?? string.Empty;
        City.Text = customer.City ?? string.Empty;
        Country.Text = _defaultCountry;
        // TextChanged is posted after this call; keep ignoring it until the UI queue drains.
        Dispatcher.UIThread.Post(() => _filling = false, DispatcherPriority.Input);
        SetEnabled(_customerCard, HasCard());
        RefreshTotals();
    }

    private void OnDiscountChanged(object? sender, TextChangedEventArgs e)
    {
        if (_filling)
        {
            return;
        }

        if (_cash == false)
        {
            _delivered = Payable;
        }

        RefreshTotals();
    }

    private void OnCardNumberChanged(object? sender, TextChangedEventArgs e)
    {
        if (_customerCard is null || _customerCard.Classes.Contains("selected"))
        {
            return;
        }

        SetEnabled(_customerCard, HasCard());
    }

    private async void OnClearCustomerClick(object? sender, RoutedEventArgs e)
    {
        Notes.Text = string.Empty;
        try
        {
            var finalConsumer = AppComposition.Services is { } services
                ? await services.GetRequiredService<IPosCustomerService>().GetFinalConsumerAsync()
                : null;
            if (finalConsumer is not null)
            {
                ShowCustomer(finalConsumer);
                return;
            }
        }
        catch
        {
            // Fall back to clearing the fields; IssueAsync will ask for a customer.
        }

        _customerSearch++;
        HideCustomerSuggest();
        CustomerSearchNotice.IsVisible = false;
        _filling = true;
        _customerId = null;
        _finalConsumer = true;
        CustomerName.Text = string.Empty;
        FiscalNumber.Text = string.Empty;
        CardNumber.Text = string.Empty;
        Discount.Text = "0,00";
        Address.Text = string.Empty;
        Locality.Text = string.Empty;
        ZipCode.Text = string.Empty;
        City.Text = string.Empty;
        Country.Text = _defaultCountry;
        Dispatcher.UIThread.Post(() => _filling = false, DispatcherPriority.Input);
        SetEnabled(_customerCard, false);
        RefreshTotals();
    }

    private void OnFullPaymentClick(object? sender, RoutedEventArgs e)
    {
        PartialOverlay.IsVisible = false;
        MixedOverlay.IsVisible = false;
        _mixedPayment = false;
        _mixedRows.Clear();
        FullPaymentButton.Classes.Add("selected");
    }

    private async void OnInvoiceClick(object? sender, RoutedEventArgs e)
    {
        if (BeginIssuing() == false)
        {
            return;
        }

        try
        {
            if (PosDocumentRules.ClosesWithPaymentTicketOnly(CurrentModule()))
            {
                _parkingInvoiceRequested = true;
                ApplySaleDocumentType();
                ShowNotice("Pagamento", "Confirme com Ok para emitir a fatura pedida.", error: false);
                return;
            }

            if (_finalConsumer || string.IsNullOrWhiteSpace(CustomerName.Text) || string.IsNullOrWhiteSpace(FiscalNumber.Text))
            {
                ShowNotice(
                    "Aviso",
                    "Não pode criar faturas com o cliente consumidor final ou com um cliente sem nome/NIF!\nUse um cliente válido e tente novamente.",
                    error: true);
                return;
            }

            await IssueAsync("FT", _lines, partial: null, settledSale: false);
        }
        finally
        {
            ReleaseIssuingIfIdle();
        }
    }

    private void OnPartialPaymentClick(object? sender, RoutedEventArgs e)
    {
        FullPaymentButton.Classes.Remove("selected");
        if (sender is Button partialButton)
        {
            partialButton.Classes.Add("selected");
        }

        PartialLines.Children.Clear();
        _partialRows.Clear();
        for (var index = 0; index < _lines.Count; index++)
        {
            var line = _lines[index];
            var payQuantity = line.Quantity >= 1m ? 1m : line.Quantity;
            var check = new CheckBox
            {
                Classes = { "bo_doc_column_check" },
                Content = line.Designation,
                IsChecked = true,
                VerticalAlignment = VerticalAlignment.Center
            };
            var quantity = new TextBox
            {
                Classes = { "pos_split_amount", "bo_touch_ready" },
                Width = 72,
                Text = payQuantity.ToString("0.###", CultureInfo.CurrentCulture),
                VerticalAlignment = VerticalAlignment.Center
            };
            var keyboard = new Button
            {
                Classes = { "bo_touch_key" },
                Content = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
                {
                    Classes = { "bo_touch_key_icon" },
                    Path = "avares://LogicPOS.App/Assets/Images/Listing/botao_teclado.svg"
                }
            };
            ToolTip.SetTip(keyboard, "Teclado virtual");
            keyboard.Click += (_, _) =>
            {
                quantity.Focus();
                TouchKeyboard.Show(numeric: true);
            };
            var total = new TextBlock
            {
                Classes = { "pos_ticket_cell" },
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Text = MoneyFormat.Format(line.WithQuantity(payQuantity).Total)
            };
            quantity.TextChanged += (_, _) =>
            {
                if (TryPartialQuantity(quantity.Text, line.Quantity, out var typed))
                {
                    total.Text = MoneyFormat.Format(line.WithQuantity(typed).Total);
                }
            };
            var row = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("*,80,36,110"),
                Margin = new Avalonia.Thickness(0, 4)
            };
            Grid.SetColumn(quantity, 1);
            Grid.SetColumn(keyboard, 2);
            Grid.SetColumn(total, 3);
            row.Children.Add(check);
            row.Children.Add(quantity);
            row.Children.Add(keyboard);
            row.Children.Add(total);
            PartialLines.Children.Add(row);
            _partialRows.Add(new PartialRow(index, line.Quantity, check, quantity));
        }

        PartialOverlay.IsVisible = true;
    }

    private async void OnPartialConfirmClick(object? sender, RoutedEventArgs e)
    {
        if (BeginIssuing() == false)
        {
            return;
        }

        try
        {
            var selected = new List<PartialPaidLine>();
            var lines = new List<PosTicketLine>();
            foreach (var row in _partialRows)
            {
                if (row.Check.IsChecked != true)
                {
                    continue;
                }

                if (TryPartialQuantity(row.Quantity.Text, row.Available, out var quantity) == false)
                {
                    ShowNotice("Aviso", $"Indique uma quantidade até {row.Available:0.###}.", error: true);
                    return;
                }

                selected.Add(new PartialPaidLine(row.Index, quantity));
                lines.Add(_lines[row.Index].WithQuantity(quantity));
            }

            if (selected.Count == 0)
            {
                ShowNotice("Aviso", "Selecione pelo menos um artigo.", error: true);
                return;
            }

            PartialOverlay.IsVisible = false;
            ApplySaleDocumentType(lines);
            if (PosDocumentRules.ClosesWithPaymentTicketOnly(CurrentModule()) && _parkingInvoiceRequested == false)
            {
                await CloseParkingWithPaymentTicketAsync(lines, selected);
                return;
            }

            await IssueAsync(_saleDocumentType, lines, selected);
        }
        finally
        {
            ReleaseIssuingIfIdle();
        }
    }

    private static bool TryPartialQuantity(string? text, decimal available, out decimal quantity)
    {
        quantity = 0m;
        return MoneyFormat.TryParse(text, out quantity)
            && quantity > 0m
            && quantity <= available + 0.0000001m;
    }

    private void OnPartialCancelClick(object? sender, RoutedEventArgs e) => PartialOverlay.IsVisible = false;

    private PosDocumentRules.SettledSaleContext BuildSettledSaleContext(IReadOnlyList<PosTicketLine> lines, decimal invoiceTotal)
        => new(
            CurrentModule(),
            invoiceTotal,
            IsAngolaSystem(),
            _companyMarks.RegimeArtigo53,
            _companyMarks.VendedorAmbulante,
            PosDocumentRules.AllLinesAreProducts(lines.Select(line => line.ClassAcronym)),
            PosDocumentRules.ResolveBuyerFiscalNumberForCap(
                FiscalNumber.Text,
                _finalConsumer,
                _finalConsumer ? null : FiscalNumber.Text));

    private void ApplySaleDocumentType(IReadOnlyList<PosTicketLine>? lines = null)
    {
        var basis = lines ?? _lines;
        var total = basis.Sum(line => line.Total);
        var payable = total - (total * DiscountPercent() / 100m);
        var context = BuildSettledSaleContext(basis, payable);
        _saleDocumentType = _parkingInvoiceRequested
            ? PosDocumentRules.ResolveSettledSaleDocumentType(context)
            : PosDocumentRules.ClosesWithPaymentTicketOnly(CurrentModule())
                ? "Talão"
                : PosDocumentRules.ResolveSettledSaleDocumentType(context);
        DocumentTypeValue.Text = _saleDocumentType;
    }

    private static string? CurrentModule()
        => AppComposition.Services?.GetService<IConfiguration>()?["DatabaseSettings:Module"];

    private static bool IsAngolaSystem()
        => AppComposition.Services?.GetService<ISystemInformationService>()?.IsAngola() == true;

    private void HideCustomerSuggest()
    {
        _customerMatches.Clear();
        CustomerSuggestList.Children.Clear();
        CustomerSuggest.IsOpen = false;
    }

    private void OnCustomerTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_filling)
        {
            return;
        }

        _customerId = null;
        _finalConsumer = false;
        _ = SuggestCustomersAsync(sender is TextBox box ? box.Text : CustomerName.Text, delay: true);
    }

    private async void OnCustomerSearchKey(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        e.Handled = true;
        var text = sender is TextBox box ? box.Text : CustomerName.Text;
        await SuggestCustomersAsync(text, delay: false);
    }

    private async Task SuggestCustomersAsync(string? text, bool delay)
    {
        var term = text?.Trim() ?? string.Empty;
        var ticket = ++_customerSearch;
        CustomerSearchNotice.IsVisible = false;
        if (term.Length == 0 || AppComposition.Services is null)
        {
            HideCustomerSuggest();
            return;
        }

        try
        {
            if (delay)
            {
                await Task.Delay(300);
                if (ticket != _customerSearch)
                {
                    return;
                }
            }

            var matches = await AppComposition.Services.GetRequiredService<IPosCustomerService>().SearchAsync(term);
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
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                // Tunnel + Handled so the popup light-dismiss does not eat the first press.
                button.AddHandler(
                    InputElement.PointerPressedEvent,
                    (_, args) =>
                    {
                        if (args.GetCurrentPoint(button).Properties.IsLeftButtonPressed == false)
                        {
                            return;
                        }

                        args.Handled = true;
                        ShowCustomer(customer);
                    },
                    RoutingStrategies.Tunnel);
                CustomerSuggestList.Children.Add(button);
            }

            CustomerSuggest.IsOpen = _customerMatches.Count > 0;
            if (_customerMatches.Count == 0)
            {
                CustomerSearchNotice.Text = "Não foi encontrado nenhum cliente.";
            }
        }
        catch (Exception)
        {
            if (ticket != _customerSearch)
            {
                return;
            }

            HideCustomerSuggest();
            CustomerSearchNotice.Text = "Não foi possível pesquisar os clientes.";
        }
    }

    private async void OnNewCustomerClick(object? sender, RoutedEventArgs e)
    {
        var customerFields = new Dictionary<string, string>
        {
            ["Name"] = CustomerName.Text?.Trim() ?? string.Empty,
            ["FiscalNumber"] = FiscalNumber.Text?.Trim() ?? string.Empty,
            ["Address"] = Address.Text?.Trim() ?? string.Empty,
            ["Locality"] = Locality.Text?.Trim() ?? string.Empty,
            ["ZipCode"] = ZipCode.Text?.Trim() ?? string.Empty,
            ["City"] = City.Text?.Trim() ?? string.Empty,
            ["CardNumber"] = CardNumber.Text?.Trim() ?? string.Empty
        };
        var invalidCustomer = GtkFormRules.Validate("Clientes", customerFields);
        if (invalidCustomer is not null)
        {
            ShowNotice("Aviso", invalidCustomer, error: true);
            return;
        }

        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        var saved = await services.GetRequiredService<IBackOfficeListingService>().SaveAsync("Clientes", null, customerFields);
        if (saved.Succeeded == false)
        {
            ShowNotice("Aviso", saved.Error ?? "Não foi possível criar o cliente.", error: true);
            return;
        }

        services.GetService<PosCustomerService>()?.InvalidateSearch();
        var customer = await services.GetRequiredService<IPosCustomerService>().FindAsync(saved.Id);
        if (customer is not null)
        {
            ShowCustomer(customer);
        }

        ShowNotice("Cliente", "Cliente criado.", error: false);
    }

    private async void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        if (BeginIssuing() == false)
        {
            return;
        }

        try
        {
            ApplySaleDocumentType();
            if (PosDocumentRules.ClosesWithPaymentTicketOnly(CurrentModule()) && _parkingInvoiceRequested == false)
            {
                await CloseParkingWithPaymentTicketAsync(_lines, partial: null);
                return;
            }

            await IssueAsync(_saleDocumentType, _lines, partial: null);
        }
        finally
        {
            ReleaseIssuingIfIdle();
        }
    }

    private async Task IssueAsync(
        string documentType,
        IReadOnlyList<PosTicketLine> lines,
        IReadOnlyList<PartialPaidLine>? partial,
        bool settledSale = true)
    {
        if (settledSale && _mixedPayment == false && _methodToken is null)
        {
            ShowNotice("Aviso", "Selecione um Método de Pagamento", error: true);
            return;
        }

        if (_mixedPayment)
        {
            var payable = SettlementTotal(lines);
            var mixedError = PosMixedPaymentValidation.ValidatePortions(
                payable,
                _mixedRows.Select(row => row.Amount).ToList());
            if (mixedError is not null)
            {
                ShowNotice("Aviso", mixedError, error: true);
                return;
            }

            if (_mixedRows.Count < 2)
            {
                ShowNotice("Aviso", "Indique pelo menos dois métodos de pagamento.", error: true);
                return;
            }
        }

        if (_customerId is null)
        {
            ShowNotice("Aviso", "Selecione um cliente.", error: true);
            return;
        }

        if (AppComposition.Services is null)
        {
            ShowNotice("Aviso", "A aplicação não está pronta para gravar o documento.", error: true);
            return;
        }

        if (settledSale)
        {
            var total = lines.Sum(line => line.Total);
            var payable = total - (total * DiscountPercent() / 100m);
            var context = BuildSettledSaleContext(lines, payable);
            var buyerError = PosDocumentRules.ValidateCompanyBuyerForSettledSale(
                context,
                CustomerName.Text,
                Address.Text,
                FiscalNumber.Text,
                _finalConsumer);
            if (buyerError is not null)
            {
                ShowNotice("Aviso", buyerError, error: true);
                return;
            }
        }

        await IssueWithSeriesAsync(documentType, lines, partial, createMissingSeries: false, settledSale);
    }

    private async Task CloseParkingWithPaymentTicketAsync(
        IReadOnlyList<PosTicketLine> lines,
        IReadOnlyList<PartialPaidLine>? partial)
    {
        if (_methodToken is null)
        {
            ShowNotice("Aviso", "Selecione um Método de Pagamento", error: true);
            return;
        }

        var total = lines.Sum(line => line.Total);
        var payable = total - (total * DiscountPercent() / 100m);
        var printError = await FrontOfficePrinting.PrintParkingPaymentTicketAsync(payable, _methodToken!);
        PartialPayment = partial is not null;
        PaidLines = partial ?? [];
        _confirmPayment = true;
        var message = $"Pagamento registado.{Environment.NewLine}Total: {MoneyFormat.Format(payable)}";
        if (printError is not null)
        {
            message += $"{Environment.NewLine}{Environment.NewLine}Não foi possível imprimir: {printError}";
        }

        ShowNotice("Pagamento", message, error: false);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        if (_issuing)
        {
            return;
        }

        Close(false);
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape)
        {
            return;
        }

        if (NoticeOverlay.IsVisible == false)
        {
            return;
        }

        if (PaymentWindowCloseDecision.ShouldCloseNoticeOnEscape(_confirmPayment, _closingWithResult))
        {
            e.Handled = true;
            NoticeOverlay.IsVisible = false;
            _pendingSeriesDocumentType = null;
            _pendingSeriesLines = null;
            _pendingSeriesPartial = null;
            _pendingSettledSale = true;
            SetNoticeButtons(confirm: false);
            _closingWithResult = true;
            Close(true);
            return;
        }

        if (PaymentWindowCloseDecision.ShouldIgnoreEscapeBeforeSuccess(_confirmPayment))
        {
            // Before issue / series prompt: Escape must not Close(false) or confirm.
            e.Handled = true;
        }
    }

    private void OnNoticeCloseClick(object? sender, RoutedEventArgs e)
    {
        NoticeOverlay.IsVisible = false;
        _pendingSeriesDocumentType = null;
        _pendingSeriesLines = null;
        _pendingSeriesPartial = null;
        _pendingSettledSale = true;
        SetNoticeButtons(confirm: false);
        if (_confirmPayment)
        {
            _closingWithResult = true;
            Close(true);
            return;
        }

        EndIssuing();
    }

    private async void OnNoticeYesClick(object? sender, RoutedEventArgs e)
    {
        // Series «Sim» stays enabled while Confirm/Fatura/Parcial are locked; guard double Yes here.
        if (NoticeYesButton is { IsEnabled: false })
        {
            return;
        }

        if (_issuing == false && BeginIssuing() == false)
        {
            return;
        }

        if (NoticeYesButton is not null)
        {
            NoticeYesButton.IsEnabled = false;
        }

        SetIssueButtonsEnabled(false);
        var documentType = _pendingSeriesDocumentType ?? "FS";
        var lines = _pendingSeriesLines ?? _lines;
        var partial = _pendingSeriesPartial;
        NoticeOverlay.IsVisible = false;
        SetNoticeButtons(confirm: false);
        _pendingSeriesDocumentType = null;
        _pendingSeriesLines = null;
        _pendingSeriesPartial = null;
        var settledSale = _pendingSettledSale;
        _pendingSettledSale = true;
        try
        {
            await IssueWithSeriesAsync(documentType, lines, partial, createMissingSeries: true, settledSale);
        }
        finally
        {
            ReleaseIssuingIfIdle();
        }
    }

    private bool BeginIssuing()
    {
        if (_issuing)
        {
            return false;
        }

        _issuing = true;
        SetIssueButtonsEnabled(false);
        return true;
    }

    private void EndIssuing()
    {
        _issuing = false;
        SetIssueButtonsEnabled(true);
        if (NoticeYesButton is not null)
        {
            NoticeYesButton.IsEnabled = true;
        }
    }

    /// <summary>Keep locked after success (Ok closes) or while waiting for series confirmation.</summary>
    private void ReleaseIssuingIfIdle()
    {
        if (_confirmPayment || string.IsNullOrWhiteSpace(_pendingSeriesDocumentType) == false)
        {
            return;
        }

        EndIssuing();
    }

    private void SetIssueButtonsEnabled(bool enabled)
    {
        if (InvoiceButton is not null)
        {
            InvoiceButton.IsEnabled = enabled;
        }

        if (ConfirmButton is not null)
        {
            ConfirmButton.IsEnabled = enabled;
        }

        if (PartialConfirmButton is not null)
        {
            PartialConfirmButton.IsEnabled = enabled;
        }

        if (CancelButton is not null)
        {
            CancelButton.IsEnabled = enabled;
        }

        if (CloseHeaderButton is not null)
        {
            CloseHeaderButton.IsEnabled = enabled;
        }
    }

    private void ShowNotice(string title, string message, bool error)
    {
        SetNoticeButtons(confirm: false);
        var icon = error ? "error" : "info";
        NoticeTitle.Text = title;
        NoticeMessage.Text = message;
        NoticeIcon.Source = LoadBitmap($"avares://LogicPOS.App/Assets/Images/Dialogs/icon_pos_dialog_{icon}_window.png");
        NoticeSymbol.Source = LoadBitmap($"avares://LogicPOS.App/Assets/Images/Dialogs/icon_pos_dialog_{icon}.png");
        // Above CashOverlay / MixedPayment overlays when those are open.
        NoticeOverlay.ZIndex = 100;
        NoticeOverlay.IsVisible = true;
    }

    private void ShowConfirm(string title, string message)
    {
        SetNoticeButtons(confirm: true);
        NoticeTitle.Text = title;
        NoticeMessage.Text = message;
        NoticeIcon.Source = LoadBitmap("avares://LogicPOS.App/Assets/Images/Dialogs/icon_pos_dialog_info_window.png");
        NoticeSymbol.Source = LoadBitmap("avares://LogicPOS.App/Assets/Images/Dialogs/icon_pos_dialog_info.png");
        NoticeOverlay.IsVisible = true;
    }

    private void SetNoticeButtons(bool confirm)
    {
        NoticeYesButton.IsVisible = confirm;
        NoticeNoButton.IsVisible = confirm;
        NoticeOkButton.IsVisible = confirm == false;
    }

    private async Task IssueWithSeriesAsync(
        string documentType,
        IReadOnlyList<PosTicketLine> lines,
        IReadOnlyList<PartialPaidLine>? partial,
        bool createMissingSeries,
        bool settledSale = true)
    {
        if (_customerId is null || AppComposition.Services is null)
        {
            return;
        }

        if (settledSale && _mixedPayment == false && _methodToken is null)
        {
            return;
        }

        try
        {
            var payable = SettlementTotal(lines);
            var saleLines = lines.Select(line => new PosSaleLine(
                line.ArticleId,
                line.Quantity,
                line.NetUnitPrice,
                line.Discount,
                line.VatRateId,
                unitPrice: line.DisplayUnitPrice,
                priceWithVat: line.PriceIncludesVat)).ToList();
            var documents = AppComposition.Services.GetRequiredService<IPosDocumentService>();
            PosDocumentResult result;
            if (settledSale)
            {
                IReadOnlyList<PosDocumentPaymentMethod>? mixedPayments = null;
                if (_mixedPayment)
                {
                    await EnsurePaymentMethodCatalogAsync();
                    mixedPayments = _mixedRows
                        .Select(row => new PosDocumentPaymentMethod(
                            _paymentMethodIds[row.Token],
                            row.Amount))
                        .ToList();
                }

                result = await documents.IssueSimplifiedInvoiceAsync(
                    _customerId.Value,
                    _mixedPayment ? null : _methodToken,
                    DiscountPercent(),
                    _mixedPayment ? null : _cash ? _delivered : payable,
                    Notes.Text,
                    saleLines,
                    documentType,
                    createMissingSeries,
                    PosDocumentRules.ResolveInvoiceFaceFiscalNumber(FiscalNumber.Text, _finalConsumer, FiscalNumber.Text),
                    mixedPayments);
            }
            else
            {
                var header = new PosDocumentHeader
                {
                    Notes = Notes.Text,
                    DiscountPercent = DiscountPercent(),
                };
                result = await documents.IssueDocumentAsync(
                    documentType,
                    _customerId.Value,
                    saleLines,
                    header,
                    createMissingSeries);
            }

            if (createMissingSeries == false && result.NeedsSeriesConfirmation)
            {
                _pendingSeriesDocumentType = result.MissingSeriesDocumentType ?? documentType;
                _pendingSeriesLines = lines;
                _pendingSeriesPartial = partial;
                _pendingSettledSale = settledSale;
                ShowConfirm(
                    "Série fiscal",
                    $"Não existe série ativa para «{_pendingSeriesDocumentType}».{Environment.NewLine}{Environment.NewLine}Deseja criar o ano fiscal e a série automaticamente?");
                return;
            }

            if (result.Succeeded == false)
            {
                ShowNotice("Aviso", result.Error ?? "Não foi possível gravar o documento.", error: true);
                return;
            }

            PartialPayment = partial is not null;
            PaidLines = partial ?? [];
            string message;
            string title;
            if (settledSale)
            {
                message = $"Pagamento efetuado.{Environment.NewLine}{result.Number}{Environment.NewLine}Total: {MoneyFormat.Format(payable)}";
                if (string.IsNullOrWhiteSpace(_shareLabel) == false)
                {
                    message = $"{_shareLabel}{Environment.NewLine}{message}";
                }

                if (_cash)
                {
                    message += $"{Environment.NewLine}Troco: {MoneyFormat.Format(_delivered - payable)}";
                }

                title = "Pagamento";
            }
            else
            {
                message = $"{result.Number}{Environment.NewLine}Total: {MoneyFormat.Format(payable)}";
                title = "Fatura";
            }

            var printError = await FrontOfficePrinting.PrintInvoiceAsync(result.DocumentId);
            if (printError is not null)
            {
                message += $"{Environment.NewLine}{Environment.NewLine}Não foi possível imprimir: {printError}";
            }

            _confirmPayment = true;
            ShowNotice(title, message, error: false);
        }
        catch (Exception exception)
        {
            ShowNotice("Aviso", exception.Message, error: true);
        }
    }

    private void ShowCashPad()
    {
        _cashTyping = false;
        _cashMoney = 0;
        CashEntry.Text = Payable > 0 ? Payable.ToString("0.00", CultureInfo.CurrentCulture) : string.Empty;
        CashOverlay.IsVisible = true;
    }

    private void OnCashMoneyClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string tag)
        {
            return;
        }

        if (MoneyFormat.TryParse(tag, out var amount) == false)
        {
            return;
        }

        if (_cashTyping)
        {
            if (MoneyFormat.TryParse(CashEntry.Text, out var typed))
            {
                _cashMoney = typed;
            }
            else
            {
                _cashMoney = 0;
            }

            _cashTyping = false;
        }
        else
        {
            _cashMoney = 0;
        }

        _cashMoney += amount;
        CashEntry.Text = _cashMoney.ToString("0.00", CultureInfo.CurrentCulture);
    }

    private void OnCashDigitClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var caption = button.Tag as string ?? button.Content as string ?? string.Empty;
        if (_cashTyping == false)
        {
            CashEntry.Text = caption == "," ? "0," : caption;
            _cashTyping = true;
            return;
        }

        var current = CashEntry.Text ?? string.Empty;
        if (caption == "," && current.Contains(','))
        {
            return;
        }

        CashEntry.Text = current.Length == 0 && caption != "," ? caption : current + caption;
    }

    private void OnCashClearClick(object? sender, RoutedEventArgs e)
    {
        var current = CashEntry.Text ?? string.Empty;
        CashEntry.Text = current.Length <= 1 ? string.Empty : current[..^1];
        _cashTyping = true;
        _cashMoney = 0;
    }

    private void OnCashOkClick(object? sender, RoutedEventArgs e)
    {
        if (MoneyFormat.TryParse(CashEntry.Text, out var value) == false || value <= 0)
        {
            CashEntry.Text = Payable.ToString("0.00", CultureInfo.CurrentCulture);
            return;
        }

        if (value + 0.0001m < Payable)
        {
            // Keep the pad open until the delivered amount covers the total (no negative change).
            ShowNotice(
                "Aviso",
                "O valor entregue é inferior ao total. Indique um valor igual ou superior ao devido.",
                error: true);
            return;
        }

        _delivered = value;
        CashOverlay.IsVisible = false;
        RefreshTotals();
    }

    private void OnCashCancelClick(object? sender, RoutedEventArgs e)
    {
        _delivered = Payable;
        CashOverlay.IsVisible = false;
        RefreshTotals();
    }

    private decimal DiscountPercent()
    {
        if (MoneyFormat.TryParse(Discount.Text, out var discount) == false || discount < 0)
        {
            return 0;
        }

        return discount;
    }

    private decimal Payable => SettlementTotal(_lines);

    private decimal SettlementTotal(IReadOnlyList<PosTicketLine> lines)
    {
        var total = lines.Sum(line => line.Total);
        return total - (total * DiscountPercent() / 100m);
    }

    private void OnMixedPaymentClick(object? sender, RoutedEventArgs e)
    {
        FullPaymentButton.Classes.Remove("selected");
        BuildMixedPaymentOverlay();
        MixedOverlay.IsVisible = true;
    }

    private void BuildMixedPaymentOverlay()
    {
        MixedLines.Children.Clear();
        _mixedRows.Clear();
        AddMixedRow(_methodToken ?? "MONEY", Payable);
        AddMixedRow("DEBIT_CARD", 0);
        UpdateMixedRemaining();
    }

    private void AddMixedRow(string token, decimal amount)
    {
        var label = _posPaymentMethods.FirstOrDefault(item => item.Token == token).Label ?? token;
        var combo = new ComboBox
        {
            ItemsSource = _posPaymentMethods.Select(item => item.Label).ToList(),
            SelectedItem = label,
            Width = 140,
            VerticalAlignment = VerticalAlignment.Center
        };
        var amountBox = new TextBox
        {
            Classes = { "pos_split_amount", "bo_touch_ready" },
            Width = 90,
            Text = amount > 0 ? amount.ToString("0.00", CultureInfo.CurrentCulture) : string.Empty,
            VerticalAlignment = VerticalAlignment.Center
        };
        amountBox.TextChanged += (_, _) => UpdateMixedRemaining();
        combo.SelectionChanged += (_, _) =>
        {
            if (combo.SelectedIndex < 0)
            {
                return;
            }

            var row = _mixedRows.FirstOrDefault(item => ReferenceEquals(item.AmountBox, amountBox));
            if (row is not null)
            {
                row.Token = _posPaymentMethods[combo.SelectedIndex].Token;
            }
        };
        var rowGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("150,*,100"),
            Margin = new Avalonia.Thickness(0, 4)
        };
        Grid.SetColumn(combo, 0);
        Grid.SetColumn(amountBox, 2);
        rowGrid.Children.Add(combo);
        rowGrid.Children.Add(amountBox);
        MixedLines.Children.Add(rowGrid);
        _mixedRows.Add(new MixedPaymentRow(token, amountBox));
    }

    private void OnMixedAddRowClick(object? sender, RoutedEventArgs e)
    {
        AddMixedRow("MONEY", 0);
        UpdateMixedRemaining();
    }

    private void UpdateMixedRemaining()
    {
        decimal entered = 0;
        foreach (var row in _mixedRows)
        {
            if (MoneyFormat.TryParse(row.AmountBox.Text, out var value))
            {
                row.Amount = value;
                entered += value;
            }
        }

        MixedRemainingValue.Text = MoneyFormat.Format(Math.Max(0, Payable - entered));
    }

    private void OnMixedConfirmClick(object? sender, RoutedEventArgs e)
    {
        UpdateMixedRemaining();
        if (_mixedRows.Count < 2)
        {
            ShowNotice("Aviso", "Indique pelo menos dois métodos de pagamento.", error: true);
            return;
        }

        var error = PosMixedPaymentValidation.ValidatePortions(
            Payable,
            _mixedRows.Select(row => row.Amount).ToList());
        if (error is not null)
        {
            ShowNotice("Aviso", error, error: true);
            return;
        }

        _mixedPayment = true;
        _methodToken = null;
        _cash = false;
        foreach (var method in _methodButtons)
        {
            method.Classes.Remove("selected");
        }

        MixedPaymentButton.Classes.Add("selected");
        MixedOverlay.IsVisible = false;
        RefreshTotals();
    }

    private void OnMixedCancelClick(object? sender, RoutedEventArgs e)
    {
        MixedOverlay.IsVisible = false;
        _mixedRows.Clear();
        _mixedPayment = false;
    }

    private async Task EnsurePaymentMethodCatalogAsync()
    {
        if (_paymentMethodIds.Count > 0 || AppComposition.Services is null)
        {
            return;
        }

        var methods = await AppComposition.Services.GetRequiredService<IPosDocumentService>().ListPaymentMethodsAsync();
        _paymentMethodIds = methods
            .Where(item => string.IsNullOrWhiteSpace(item.Code) == false)
            .ToDictionary(item => item.Code, item => item.Id, StringComparer.OrdinalIgnoreCase);
    }

    private void RefreshTotals()
    {
        if (_cash == false || _mixedPayment)
        {
            _delivered = Payable;
        }

        var change = _cash && _mixedPayment == false ? _delivered - Payable : 0;
        TotalValue.Text = MoneyFormat.Format(Payable);
        DeliveryValue.Text = MoneyFormat.Format(_delivered);
        ChangeValue.Text = MoneyFormat.Format(change);
        ApplySaleDocumentType();
    }

    private bool HasCard()
    {
        return string.IsNullOrWhiteSpace(CardNumber.Text) == false;
    }

    private static void SetEnabled(Button? button, bool enabled)
    {
        if (button is null)
        {
            return;
        }

        button.IsEnabled = enabled;
        if (button.Classes.Contains("selected") == false)
        {
            button.Opacity = enabled ? 1 : 0.45;
        }
    }

    private Bitmap LoadBitmap(string uri)
    {
        if (_bitmaps.TryGetValue(uri, out var cached))
        {
            return cached;
        }

        var bitmap = new Bitmap(AssetLoader.Open(new Uri(uri)));
        _bitmaps[uri] = bitmap;
        return bitmap;
    }

    private sealed class MixedPaymentRow
    {
        public MixedPaymentRow(string token, TextBox amountBox)
        {
            Token = token;
            AmountBox = amountBox;
        }

        public string Token { get; set; }

        public TextBox AmountBox { get; }

        public decimal Amount { get; set; }
    }

    private sealed class PartialRow
    {
        public PartialRow(int index, decimal available, CheckBox check, TextBox quantity)
        {
            Index = index;
            Available = available;
            Check = check;
            Quantity = quantity;
        }

        public int Index { get; }

        public decimal Available { get; }

        public CheckBox Check { get; }

        public TextBox Quantity { get; }
    }
}

public readonly record struct PartialPaidLine(int Index, decimal Quantity);
