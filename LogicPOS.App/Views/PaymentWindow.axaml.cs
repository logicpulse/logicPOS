using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LogicPOS.App.Hardware;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
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
    private IReadOnlyList<int>? _pendingSeriesPartialIndexes;

    public bool PartialPayment { get; private set; }

    public IReadOnlyList<int> PaidLineIndexes { get; private set; } = [];

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
        SelectSaleDocument("FS");
        BuildMethods();
        TouchFields.Attach(this);
        Opened += (_, _) => CoverOwner();
        Opened += async (_, _) => await LoadCustomerAsync();
        RefreshTotals();
    }

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
        AddMethod("CASH_MACHINE", "MB", "icon_pos_payment_type_cash_machine.png", 1, 0, 1);
        AddMethod("DEBIT_CARD", "Cartão Débito", "icon_pos_payment_type_debit_card.png", 2, 0, 1);
        AddMethod("BANK_CHECK", "Cheque", "icon_pos_payment_type_bank_check.png", 0, 1, 1);
        AddMethod("CREDIT_CARD", "Cartão Crédito", "icon_pos_payment_type_credit_card.png", 1, 1, 1);
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
        CustomerMatches.Children.Clear();
        _customerMatches.Clear();
        CustomerSearchNotice.IsVisible = false;
        _filling = true;
        _customerId = customer.Id;
        _finalConsumer = customer.IsFinalConsumer;
        _defaultCountry = customer.Country ?? string.Empty;
        CustomerName.Text = customer.Name;
        FiscalNumber.Text = customer.FiscalNumber;
        CardNumber.Text = customer.CardNumber ?? string.Empty;
        Discount.Text = customer.Discount.ToString("0.00");
        Address.Text = customer.Address ?? string.Empty;
        Locality.Text = customer.Locality ?? string.Empty;
        ZipCode.Text = customer.ZipCode ?? string.Empty;
        City.Text = customer.City ?? string.Empty;
        Country.Text = _defaultCountry;
        _filling = false;
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

    private void OnClearCustomerClick(object? sender, RoutedEventArgs e)
    {
        _customerSearch++;
        CustomerMatches.Children.Clear();
        _customerMatches.Clear();
        CustomerSearchNotice.IsVisible = false;
        _filling = true;
        _customerId = null;
        _finalConsumer = false;
        CustomerName.Text = string.Empty;
        FiscalNumber.Text = string.Empty;
        CardNumber.Text = string.Empty;
        Discount.Text = "0,00";
        Address.Text = string.Empty;
        Locality.Text = string.Empty;
        ZipCode.Text = string.Empty;
        City.Text = string.Empty;
        Country.Text = _defaultCountry;
        Notes.Text = string.Empty;
        _filling = false;
        SetEnabled(_customerCard, false);
        RefreshTotals();
    }

    private async void OnInvoiceClick(object? sender, RoutedEventArgs e)
    {
        if (_finalConsumer || string.IsNullOrWhiteSpace(CustomerName.Text) || string.IsNullOrWhiteSpace(FiscalNumber.Text))
        {
            ShowNotice(
                "Aviso",
                "Não pode criar faturas com o cliente consumidor final ou com um cliente sem nome/NIF!\nUse um cliente válido e tente novamente.",
                error: true);
            return;
        }

        await IssueAsync("FT", _lines, partialIndexes: null);
    }

    private void OnPartialPaymentClick(object? sender, RoutedEventArgs e)
    {
        PartialLines.Children.Clear();
        for (var index = 0; index < _lines.Count; index++)
        {
            var line = _lines[index];
            PartialLines.Children.Add(new CheckBox
            {
                Classes = { "bo_doc_column_check" },
                Content = $"{line.Designation}  {line.Quantity:0.##}  {MoneyFormat.Format(line.Total)}",
                IsChecked = true,
                Tag = index
            });
        }

        PartialOverlay.IsVisible = true;
    }

    private async void OnPartialConfirmClick(object? sender, RoutedEventArgs e)
    {
        var indexes = PartialLines.Children
            .OfType<CheckBox>()
            .Where(box => box.IsChecked == true && box.Tag is int)
            .Select(box => (int)box.Tag!)
            .ToList();
        if (indexes.Count == 0)
        {
            ShowNotice("Aviso", "Selecione pelo menos um artigo.", error: true);
            return;
        }

        PartialOverlay.IsVisible = false;
        var lines = indexes.Select(index => _lines[index]).ToList();
        await IssueAsync(_saleDocumentType, lines, indexes);
    }

    private void OnPartialCancelClick(object? sender, RoutedEventArgs e) => PartialOverlay.IsVisible = false;

    private void OnSaleDocumentClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string tag })
        {
            SelectSaleDocument(tag);
        }
    }

    private void SelectSaleDocument(string tag)
    {
        _saleDocumentType = tag is "FR" ? "FR" : "FS";
        DocumentFs.Classes.Set("selected", _saleDocumentType == "FS");
        DocumentFr.Classes.Set("selected", _saleDocumentType == "FR");
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
        CustomerMatches.Children.Clear();
        _customerMatches.Clear();
        CustomerSearchNotice.IsVisible = false;
        if (term.Length == 0 || AppComposition.Services is null)
        {
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

            _customerMatches.AddRange(matches);
            foreach (var match in _customerMatches)
            {
                var customer = match;
                var button = new Button
                {
                    Classes = { "pos_list_item" },
                    Content = $"{customer.Name}  {customer.FiscalNumber}"
                };
                button.Click += (_, _) => ShowCustomer(customer);
                CustomerMatches.Children.Add(button);
            }

            if (_customerMatches.Count == 0)
            {
                CustomerSearchNotice.Text = "Não foi encontrado nenhum cliente.";
                CustomerSearchNotice.IsVisible = true;
            }
        }
        catch (Exception)
        {
            if (ticket != _customerSearch)
            {
                return;
            }

            CustomerSearchNotice.Text = "Não foi possível pesquisar os clientes.";
            CustomerSearchNotice.IsVisible = true;
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
        await IssueAsync(_saleDocumentType, _lines, partialIndexes: null);
    }

    private async Task IssueAsync(string documentType, IReadOnlyList<PosTicketLine> lines, IReadOnlyList<int>? partialIndexes)
    {
        if (_methodToken is null)
        {
            ShowNotice("Aviso", "Selecione um Método de Pagamento", error: true);
            return;
        }

        if (_customerId is null)
        {
            ShowNotice("Aviso", "Selecione um cliente.", error: true);
            return;
        }

        if (documentType == "FR" && (_finalConsumer || string.IsNullOrWhiteSpace(CustomerName.Text) || string.IsNullOrWhiteSpace(FiscalNumber.Text)))
        {
            ShowNotice("Aviso", "A fatura-recibo exige um cliente com nome e NIF.", error: true);
            return;
        }

        if (AppComposition.Services is null)
        {
            ShowNotice("Aviso", "A aplicação não está pronta para gravar o documento.", error: true);
            return;
        }

        await IssueWithSeriesAsync(documentType, lines, partialIndexes, createMissingSeries: false);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }

    private void OnNoticeCloseClick(object? sender, RoutedEventArgs e)
    {
        NoticeOverlay.IsVisible = false;
        _pendingSeriesDocumentType = null;
        _pendingSeriesLines = null;
        _pendingSeriesPartialIndexes = null;
        SetNoticeButtons(confirm: false);
        if (_confirmPayment)
        {
            Close(true);
        }
    }

    private async void OnNoticeYesClick(object? sender, RoutedEventArgs e)
    {
        var documentType = _pendingSeriesDocumentType ?? "FS";
        var lines = _pendingSeriesLines ?? _lines;
        var partial = _pendingSeriesPartialIndexes;
        NoticeOverlay.IsVisible = false;
        SetNoticeButtons(confirm: false);
        _pendingSeriesDocumentType = null;
        _pendingSeriesLines = null;
        _pendingSeriesPartialIndexes = null;
        await IssueWithSeriesAsync(documentType, lines, partial, createMissingSeries: true);
    }

    private void ShowNotice(string title, string message, bool error)
    {
        SetNoticeButtons(confirm: false);
        var icon = error ? "error" : "info";
        NoticeTitle.Text = title;
        NoticeMessage.Text = message;
        NoticeIcon.Source = LoadBitmap($"avares://LogicPOS.App/Assets/Images/Dialogs/icon_pos_dialog_{icon}_window.png");
        NoticeSymbol.Source = LoadBitmap($"avares://LogicPOS.App/Assets/Images/Dialogs/icon_pos_dialog_{icon}.png");
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
        IReadOnlyList<int>? partialIndexes,
        bool createMissingSeries)
    {
        if (_methodToken is null || _customerId is null || AppComposition.Services is null)
        {
            return;
        }

        try
        {
            var total = lines.Sum(line => line.Total);
            var payable = total - (total * DiscountPercent() / 100m);
            var result = await AppComposition.Services.GetRequiredService<IPosDocumentService>().IssueSimplifiedInvoiceAsync(
                _customerId.Value,
                _methodToken,
                DiscountPercent(),
                _cash ? _delivered : payable,
                Notes.Text,
                lines.Select(line => new PosSaleLine(
                    line.ArticleId,
                    line.Quantity,
                    line.NetUnitPrice,
                    line.Discount,
                    line.VatRateId,
                    unitPrice: line.DisplayUnitPrice,
                    priceWithVat: line.PriceIncludesVat)).ToList(),
                documentType,
                createMissingSeries);

            if (createMissingSeries == false && result.NeedsSeriesConfirmation)
            {
                _pendingSeriesDocumentType = result.MissingSeriesDocumentType ?? documentType;
                _pendingSeriesLines = lines;
                _pendingSeriesPartialIndexes = partialIndexes;
                ShowConfirm(
                    "Série fiscal",
                    $"Não existe série activa para «{_pendingSeriesDocumentType}».{Environment.NewLine}{Environment.NewLine}Deseja criar o ano fiscal e a série automaticamente?");
                return;
            }

            if (result.Succeeded == false)
            {
                ShowNotice("Aviso", result.Error ?? "Não foi possível gravar o documento.", error: true);
                return;
            }

            PartialPayment = partialIndexes is not null;
            PaidLineIndexes = partialIndexes ?? [];
            var message = $"Pagamento efetuado.{Environment.NewLine}{result.Number}{Environment.NewLine}Total: {MoneyFormat.Format(payable)}";
            if (string.IsNullOrWhiteSpace(_shareLabel) == false)
            {
                message = $"{_shareLabel}{Environment.NewLine}{message}";
            }

            if (_cash)
            {
                message += $"{Environment.NewLine}Troco: {MoneyFormat.Format(_delivered - payable)}";
            }

            var printError = await FrontOfficePrinting.PrintInvoiceAsync(result.DocumentId);
            if (printError is not null)
            {
                message += $"{Environment.NewLine}{Environment.NewLine}Erro ao imprimir: {printError}";
            }

            _confirmPayment = true;
            ShowNotice("Pagamento", message, error: false);
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
            _cashMoney = 0;
            _cashTyping = false;
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
        if (MoneyFormat.TryParse(CashEntry.Text, out var value) && value > 0)
        {
            _delivered = value;
        }
        else
        {
            _delivered = Payable;
        }

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

    private decimal Payable
    {
        get
        {
            return _orderTotal - (_orderTotal * DiscountPercent() / 100m);
        }
    }

    private void RefreshTotals()
    {
        if (_cash == false)
        {
            _delivered = Payable;
        }

        var change = _cash ? _delivered - Payable : 0;
        TotalValue.Text = MoneyFormat.Format(Payable);
        DeliveryValue.Text = MoneyFormat.Format(_delivered);
        ChangeValue.Text = MoneyFormat.Format(change);
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
}
