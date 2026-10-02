using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class ReceiptEmissionView : UserControl
{
    private readonly ObservableCollection<ReceiptGridRow> _visible = new();
    private readonly List<ReceiptEmissionRow> _rows = new();
    private ReceiptEmissionCatalog _catalog = new();
    private bool _datesReady;
    private bool _filling;
    private decimal _net;
    private ListingLoad _load = null!;

    public ReceiptEmissionView()
    {
        InitializeComponent();
        _load = new ListingLoad(Busy);
        ListingNotice.HideWhenEmpty(Notice);
        DocumentGrid.ItemsSource = _visible;
        TouchFields.Attach(this);
    }

    public async Task ReloadAsync()
    {
        if (_datesReady == false)
        {
            StartDate.SelectedDate = new DateTime(DateTime.Today.Year, 1, 1);
            EndDate.SelectedDate = DateTime.Today;
            _datesReady = true;
        }

        await LoadAsync();
    }

    public bool Allow(ReceiptGridRow row)
    {
        if (row.Source.CustomerId == Guid.Empty)
        {
            Notice.Text = "Os documentos devem pertencer ao mesmo cliente.";
            return false;
        }

        var selected = _visible.Where(item => item.IsSelected && ReferenceEquals(item, row) == false).ToList();
        if (selected.Count == 0)
        {
            return true;
        }

        if (selected.Any(item => item.Source.CustomerId != row.Source.CustomerId)
            && selected.Any(item => string.Equals(item.Source.FiscalNumber, row.Source.FiscalNumber, StringComparison.OrdinalIgnoreCase)) == false)
        {
            Notice.Text = "Os documentos devem pertencer ao mesmo cliente.";
            return false;
        }

        return true;
    }

    public void RefreshSelection()
    {
        var selected = Selected().ToList();
        _net = ReceiptSettlement.Net(selected.Select(item => item.Source));
        SelectionLabel.Text = $"({selected.Count}) = {_net.ToString("0.00", CultureInfo.CurrentCulture)}";
        if (selected.Count == 0)
        {
            Notice.Text = string.Empty;
        }
    }

    private async void OnFilterClick(object? sender, RoutedEventArgs e) => await LoadAsync();

    private void OnSearchChanged(object? sender, TextChangedEventArgs e) => ApplySearch();

    private void OnSearchClick(object? sender, RoutedEventArgs e) => ApplySearch();

    private async void OnPayClick(object? sender, RoutedEventArgs e)
    {
        var selected = Selected().Select(item => item.Source).ToList();
        var error = ValidateSelection(selected);
        if (error is not null)
        {
            Notice.Text = error;
            return;
        }

        Notice.Text = string.Empty;
        await EnsureCatalogAsync();
        _net = ReceiptSettlement.Net(selected);
        _filling = true;
        PaymentMethodBox.ItemsSource = _catalog.PaymentMethods;
        PaymentMethodBox.SelectedItem = null;
        CurrencyBox.ItemsSource = _catalog.Currencies;
        var currency = _catalog.Currencies.FirstOrDefault(item => item.Id == selected[0].CurrencyId) ?? _catalog.Currencies.FirstOrDefault();
        CurrencyBox.SelectedItem = currency;
        var rate = currency is null || currency.ExchangeRate <= 0 ? 1m : currency.ExchangeRate;
        ExchangeRate.Text = rate.ToString("0.00", CultureInfo.CurrentCulture);
        TotalPaid.Text = (rate == 0 ? _net : _net / rate).ToString("0.00", CultureInfo.CurrentCulture);
        PayDate.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        PayNotes.Text = string.Empty;
        PayError.Text = string.Empty;
        _filling = false;
        ApplyAmounts();
        PayOverlay.IsVisible = true;
    }

    private void OnPayCancelClick(object? sender, RoutedEventArgs e) => PayOverlay.IsVisible = false;

    private async void OnPayOkClick(object? sender, RoutedEventArgs e)
    {
        var selected = Selected().Select(item => item.Source).ToList();
        var error = ValidateSelection(selected) ?? ValidatePayFields();
        if (error is not null)
        {
            PayError.Text = error;
            return;
        }

        if (TryMoney(TotalPaid.Text, out var currencyAmount) == false || TryMoney(ExchangeRate.Text, out var rate) == false)
        {
            PayError.Text = "A taxa de câmbio tem de ser superior a zero.";
            return;
        }

        var amount = ReceiptSettlement.Round(currencyAmount * rate);
        var method = PaymentMethodBox.SelectedItem as ReceiptPayOption;
        var currency = CurrencyBox.SelectedItem as ReceiptPayOption;
        PayOkButton.IsEnabled = false;
        try
        {
            var service = AppComposition.Services?.GetService<IReceiptEmission>();
            if (service is null)
            {
                PayError.Text = "Emissão de recibos indisponível.";
                return;
            }

            var result = await service.PayAsync(new ReceiptPayRequest
            {
                DocumentIds = selected.Select(item => item.Id).ToList(),
                PaymentMethodId = method!.Id,
                CurrencyId = currency!.Id,
                Amount = amount,
                CurrencyAmount = ReceiptSettlement.Round(currencyAmount),
                ExchangeRate = rate,
                Notes = PayNotes.Text?.Trim() ?? string.Empty
            });
            if (result.Ok == false)
            {
                PayError.Text = result.Message;
                return;
            }

            PayOverlay.IsVisible = false;
            Notice.Text = result.Message;
            await LoadAsync();
            if (string.IsNullOrWhiteSpace(result.PdfPath) == false && TopLevel.GetTopLevel(this) is IOfficeSurface office)
            {
                await office.ShowPdfAsync(result.PdfPath, "Recibo");
            }
        }
        catch (Exception exception)
        {
            PayError.Text = exception.Message;
        }
        finally
        {
            PayOkButton.IsEnabled = true;
        }
    }

    private async void OnOpenClick(object? sender, RoutedEventArgs e)
    {
        var selected = Selected().ToList();
        if (selected.Count != 1)
        {
            Notice.Text = "Seleccione um documento para abrir.";
            return;
        }

        await OpenDocumentAsync(selected[0]);
    }

    private async void OnRowViewClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: ReceiptGridRow row })
        {
            return;
        }

        await OpenDocumentAsync(row);
    }

    private async Task OpenDocumentAsync(ReceiptGridRow row)
    {
        try
        {
            var documents = AppComposition.Services?.GetService<IPosDocumentService>();
            var path = documents is null ? null : await documents.CreateA4FileAsync(row.Source.Id);
            if (string.IsNullOrWhiteSpace(path))
            {
                Notice.Text = "Não foi possível abrir o documento.";
                return;
            }

            if (TopLevel.GetTopLevel(this) is IOfficeSurface office)
            {
                await office.ShowPdfAsync(path, row.Number);
            }
        }
        catch (Exception exception)
        {
            Notice.Text = exception.Message;
        }
    }

    private void OnCurrencyChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_filling || CurrencyBox.SelectedItem is not ReceiptPayOption currency)
        {
            return;
        }

        _filling = true;
        ExchangeRate.Text = (currency.ExchangeRate <= 0 ? 1m : currency.ExchangeRate).ToString("0.00", CultureInfo.CurrentCulture);
        _filling = false;
        ApplyAmounts();
    }

    private void OnAmountChanged(object? sender, TextChangedEventArgs e) => ApplyAmounts();

    private Task LoadAsync() => _load.RunAsync(LoadCoreAsync);

    private async Task LoadCoreAsync()
    {
        var start = StartDate.SelectedDate?.Date ?? new DateTime(DateTime.Today.Year, 1, 1);
        var end = EndDate.SelectedDate?.Date ?? DateTime.Today;
        if (start > end)
        {
            Notice.Text = "A data inicial não pode ser posterior à data final.";
            return;
        }

        try
        {
            var service = AppComposition.Services?.GetService<IReceiptEmission>();
            _rows.Clear();
            if (service is not null)
            {
                _rows.AddRange(await service.ListAsync(start, end));
            }

            Notice.Text = string.Empty;
            ApplySearch();
        }
        catch (Exception exception)
        {
            Notice.Text = exception.Message;
        }
    }

    private void ApplySearch()
    {
        var search = SearchBox.Text?.Trim() ?? string.Empty;
        _visible.Clear();
        foreach (var row in _rows)
        {
            if (search.Length > 0
                && row.Number.Contains(search, StringComparison.CurrentCultureIgnoreCase) == false
                && row.CustomerName.Contains(search, StringComparison.CurrentCultureIgnoreCase) == false
                && row.FiscalNumber.Contains(search, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                continue;
            }

            _visible.Add(new ReceiptGridRow(row, this));
        }

        RefreshSelection();
    }

    private async Task EnsureCatalogAsync()
    {
        if (_catalog.PaymentMethods.Count > 0 && _catalog.Currencies.Count > 0)
        {
            return;
        }

        var service = AppComposition.Services?.GetService<IReceiptEmission>();
        _catalog = service is null ? new ReceiptEmissionCatalog() : await service.CatalogAsync();
    }

    private IEnumerable<ReceiptGridRow> Selected() => _visible.Where(item => item.IsSelected);

    private void ApplyAmounts()
    {
        if (_filling)
        {
            return;
        }

        if (TryMoney(ExchangeRate.Text, out var rate) == false || rate <= 0 || TryMoney(TotalPaid.Text, out var paid) == false)
        {
            UpdatePayTitle(0);
            return;
        }

        var system = ReceiptSettlement.Round(paid * rate);
        if (system > _net && _net >= 0)
        {
            _filling = true;
            TotalPaid.Text = rate == 0 ? "0.00" : (_net / rate).ToString("0.00", CultureInfo.CurrentCulture);
            _filling = false;
            system = _net;
        }

        SystemAmount.Text = system.ToString("0.00", CultureInfo.CurrentCulture);
        UpdatePayTitle(system);
    }

    private void UpdatePayTitle(decimal systemAmount)
    {
        var count = Selected().Count();
        var percent = _net > 0 ? systemAmount / _net * 100m : 100m;
        PayTitle.Text = $"Liquidar Faturas ({count} = {_net.ToString("0.00", CultureInfo.CurrentCulture)}) - {percent.ToString("0.00", CultureInfo.CurrentCulture)}%";
    }

    private string? ValidateSelection(IReadOnlyList<ReceiptEmissionRow> selected)
    {
        if (selected.Count == 0)
        {
            return "É necessário seleccionar pelo menos um documento.";
        }

        if (selected.Any(item => item.IsDraft))
        {
            return "Rascunhos não podem ser liquidados";
        }

        var paid = string.Join(",", selected.Where(item => item.Paid).Select(item => item.Number));
        if (paid.Length > 0)
        {
            return $"Os seguintes documentos já foram pagos: {paid}";
        }

        if (selected.All(item => item.Type == "NC"))
        {
            return "É necessário pelo menos uma fatura ou nota de débito para liquidar.";
        }

        if (ReceiptSettlement.Net(selected) < 0)
        {
            return "O total das notas de crédito seleccionadas excede a dívida das faturas.";
        }

        if (selected.Select(item => item.CustomerId).Distinct().Count() > 1)
        {
            return "Os documentos devem pertencer ao mesmo cliente.";
        }

        return null;
    }

    private string? ValidatePayFields()
    {
        if (PaymentMethodBox.SelectedItem is not ReceiptPayOption)
        {
            return "O método de pagamento é obrigatório.";
        }

        if (CurrencyBox.SelectedItem is not ReceiptPayOption)
        {
            return "A moeda é obrigatória.";
        }

        if (TryMoney(ExchangeRate.Text, out var rate) == false || rate <= 0)
        {
            return "A taxa de câmbio tem de ser superior a zero.";
        }

        if (TryMoney(TotalPaid.Text, out var currencyAmount) == false || currencyAmount < 0)
        {
            return "O montante em moeda não pode ser negativo.";
        }

        var amount = ReceiptSettlement.Round(currencyAmount * rate);
        if (_net > 0 && amount <= 0)
        {
            return "O valor do pagamento tem de ser superior a zero.";
        }

        if (amount > _net)
        {
            return "O valor do pagamento é superior ao total em dívida dos documentos seleccionados.";
        }

        if (DateTime.TryParseExact(PayDate.Text?.Trim(), "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) == false)
        {
            return "Data inválida.";
        }

        return null;
    }

    private static bool TryMoney(string? text, out decimal value)
    {
        text = (text ?? string.Empty).Trim();
        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value))
        {
            return true;
        }

        return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }
}

public sealed class ReceiptGridRow : INotifyPropertyChanged
{
    private readonly ReceiptEmissionView _owner;
    private bool _selected;

    public ReceiptGridRow(ReceiptEmissionRow source, ReceiptEmissionView owner)
    {
        Source = source;
        _owner = owner;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ReceiptEmissionRow Source { get; }

    public string Number => Source.Number;

    public string Status => Source.Status;

    public string CustomerName => Source.CustomerName;

    public string FiscalNumber => Source.FiscalNumber;

    public string RelatedDocuments => Source.RelatedDocuments;

    public string DateText => Source.CreatedAt.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);

    public string TotalFinalText => Source.TotalFinal.ToString("0.00", CultureInfo.CurrentCulture);

    public string TotalPaidText => Source.TotalPaid.ToString("0.00", CultureInfo.CurrentCulture);

    public string TotalToPayText => Source.TotalToPay.ToString("0.00", CultureInfo.CurrentCulture);

    public bool IsSelected
    {
        get => _selected;
        set
        {
            if (value && _selected == false && _owner.Allow(this) == false)
            {
                OnPropertyChanged();
                return;
            }

            if (_selected == value)
            {
                return;
            }

            _selected = value;
            OnPropertyChanged();
            _owner.RefreshSelection();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
