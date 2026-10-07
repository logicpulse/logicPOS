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
    private bool _replaceCash;
    private bool _filling;
    private bool _confirmPayment;
    private bool _finalConsumer = true;
    private Guid? _customerId;
    private string _defaultCountry = string.Empty;
    private TextBlock? _cashValue;
    private readonly string? _shareLabel;

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
        BuildMethods();
        BuildCashKeypad();
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
                        Source = LoadBitmap($"avares://logicpos/Assets/Images/Pos/Payments/{iconFile}")
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
                Content = $"{line.Designation}  {line.Quantity:0.##}  {line.Total:C}",
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
        await IssueAsync("FS", lines, indexes);
    }

    private void OnPartialCancelClick(object? sender, RoutedEventArgs e) => PartialOverlay.IsVisible = false;

    private async void OnCustomerSearchKey(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        var text = sender is TextBox box ? box.Text : CustomerName.Text;
        await SearchCustomersAsync(text);
    }

    private async Task SearchCustomersAsync(string? text)
    {
        var services = AppComposition.Services;
        if (services is null || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var matches = await services.GetRequiredService<IPosCustomerService>().SearchAsync(text);
        if (matches.Count == 0)
        {
            ShowNotice("Aviso", "Não foi encontrado nenhum cliente.", error: true);
            return;
        }

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

        var customer = await services.GetRequiredService<IPosCustomerService>().FindAsync(saved.Id);
        if (customer is not null)
        {
            ShowCustomer(customer);
        }

        ShowNotice("Cliente", "Cliente criado.", error: false);
    }

    private async void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        await IssueAsync("FS", _lines, partialIndexes: null);
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

        var services = AppComposition.Services;
        if (services is null)
        {
            ShowNotice("Aviso", "A aplicação não está pronta para gravar o documento.", error: true);
            return;
        }

        try
        {
            var total = lines.Sum(line => line.Total);
            var payable = total - (total * DiscountPercent() / 100m);
            var result = await services.GetRequiredService<IPosDocumentService>().IssueSimplifiedInvoiceAsync(
                _customerId.Value,
                _methodToken,
                DiscountPercent(),
                _cash ? _delivered : payable,
                Notes.Text,
                lines.Select(line => new PosSaleLine(line.ArticleId, line.Quantity, line.NetUnitPrice, line.Discount, line.VatRateId)).ToList(),
                documentType);

            if (result.Succeeded == false)
            {
                ShowNotice("Aviso", result.Error ?? "Não foi possível gravar o documento.", error: true);
                return;
            }

            PartialPayment = partialIndexes is not null;
            PaidLineIndexes = partialIndexes ?? [];
            var message = $"Pagamento efetuado.{Environment.NewLine}{result.Number}{Environment.NewLine}Total: {payable:C}";
            if (string.IsNullOrWhiteSpace(_shareLabel) == false)
            {
                message = $"{_shareLabel}{Environment.NewLine}{message}";
            }
            if (_cash)
            {
                message += $"{Environment.NewLine}Troco: {_delivered - payable:C}";
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

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }

    private void OnNoticeCloseClick(object? sender, RoutedEventArgs e)
    {
        NoticeOverlay.IsVisible = false;
        if (_confirmPayment)
        {
            Close(true);
        }
    }

    private void ShowNotice(string title, string message, bool error)
    {
        var icon = error ? "error" : "info";
        NoticeTitle.Text = title;
        NoticeMessage.Text = message;
        NoticeIcon.Source = LoadBitmap($"avares://logicpos/Assets/Images/Dialogs/icon_pos_dialog_{icon}_window.png");
        NoticeSymbol.Source = LoadBitmap($"avares://logicpos/Assets/Images/Dialogs/icon_pos_dialog_{icon}.png");
        NoticeOverlay.IsVisible = true;
    }

    private void ShowCashPad()
    {
        _replaceCash = true;
        if (_cashValue is not null)
        {
            _cashValue.Text = Payable.ToString("0.00");
        }

        CashOverlay.IsVisible = true;
    }

    private void BuildCashKeypad()
    {
        _cashValue = new TextBlock
        {
            Classes = { "pos_ticket_cell" },
            FontSize = 16,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 8, 0, 4)
        };
        CashKeypad.Children.Add(_cashValue);

        var keys = new WrapPanel { Orientation = Orientation.Horizontal, Width = 315 };
        foreach (var caption in new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", ",", "0" })
        {
            var digit = caption;
            var key = new Button { Classes = { "login_pin_key", "login_pin_digit" }, Content = digit };
            key.Click += (_, _) => AppendCash(digit);
            keys.Children.Add(key);
        }

        var clear = new Button { Classes = { "login_pin_key", "login_pin_ce" }, Content = "CE" };
        clear.Click += (_, _) =>
        {
            _cashValue.Text = "0";
            _replaceCash = true;
        };
        keys.Children.Add(clear);
        CashKeypad.Children.Add(keys);

        var ok = new Button { Classes = { "login_pin_ok" }, Content = "Ok" };
        ok.Click += (_, _) => AcceptCash();
        CashKeypad.Children.Add(ok);
    }

    private void AppendCash(string caption)
    {
        if (_cashValue is null)
        {
            return;
        }

        if (_replaceCash)
        {
            _cashValue.Text = caption == "," ? "0," : caption;
            _replaceCash = false;
            return;
        }

        var current = _cashValue.Text ?? string.Empty;
        if (caption == "," && current.Contains(','))
        {
            return;
        }

        _cashValue.Text = current == "0" && caption != "," ? caption : current + caption;
    }

    private void AcceptCash()
    {
        if (decimal.TryParse(_cashValue?.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var value))
        {
            _delivered = value;
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
        if (decimal.TryParse(Discount.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var discount) == false || discount < 0)
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
        TotalValue.Text = Payable.ToString("C");
        DeliveryValue.Text = _delivered.ToString("C");
        ChangeValue.Text = change.ToString("C");
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
