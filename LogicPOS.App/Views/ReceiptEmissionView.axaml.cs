using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using LogicPOS.App;
using LogicPOS.Application.Features.System;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.Entities.Finance.Calculations;
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
    private bool _fillingFilters;
    private bool _filtersRequested;
    private Guid? _appliedCustomer;
    private int _page = 1;
    private int _totalPages = 1;
    private int _loadGeneration;
    private string _loadedSearch = string.Empty;
    private CancellationTokenSource? _searchDelay;
    private decimal _net;
    private bool _syncingSelection;
    private bool _portrait;
    private UiViewport _viewport;
    private Panel? _receiptFiltersHome;
    private ListingLoad _load = null!;

    public ReceiptEmissionView()
    {
        InitializeComponent();
        ListingFilters.EnableCustomerSearch(CustomerBox);
        _load = new ListingLoad(Busy);
        ListingNotice.HideWhenEmpty(Notice);
        DocumentGrid.ItemsSource = _visible;
        TouchFields.Attach(this);
        CardList.ItemsSource = _visible;
        CardList.ItemTemplate = new FuncDataTemplate<ReceiptGridRow>((item, _) =>
            PortraitChrome.Card(
                item.Number,
                $"{item.DateText} · {item.Status}",
                $"{item.CustomerName} · {item.TotalToPayText}",
                item), true);
        AttachedToVisualTree += (_, _) =>
        {
            RelayoutForViewport();
            LayoutUpdated += OnReceiptLayoutUpdated;
        };
        DetachedFromVisualTree += (_, _) => LayoutUpdated -= OnReceiptLayoutUpdated;
    }

    public async Task ReloadAsync()
    {
        if (_datesReady == false)
        {
            StartDate.SelectedDate = new DateTime(DateTime.Today.Year, 1, 1);
            EndDate.SelectedDate = DateTime.Today;
            _datesReady = true;
        }

        EnsureFilters();
        await LoadAsync();
    }

    private void OnCustomerFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_fillingFilters || _datesReady == false)
        {
            return;
        }

        var next = ListingFilters.CustomerId(CustomerBox);
        if (next == _appliedCustomer)
        {
            return;
        }

        _appliedCustomer = next;
        _page = 1;
        _ = LoadAsync();
    }

    private void EnsureFilters()
    {
        if (_filtersRequested)
        {
            return;
        }

        _filtersRequested = true;
        _ = LoadFiltersAsync();
    }

    private async Task LoadFiltersAsync()
    {
        try
        {
            var customers = await ListingFilters.CustomersAsync();
            _fillingFilters = true;
            CustomerBox.ItemsSource = customers;
            ListingFilters.SelectEveryone(CustomerBox);
            _appliedCustomer = null;
        }
        catch (Exception exception)
        {
            Notice.Text = exception.Message;
        }
        finally
        {
            _fillingFilters = false;
        }
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

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_syncingSelection)
        {
            return;
        }

        SyncPicksFromGridSelection();
    }

    internal void OnRowPicked()
    {
        if (_syncingSelection)
        {
            return;
        }

        SyncGridSelectionFromPicks();
    }

    private void SyncPicksFromGridSelection()
    {
        _syncingSelection = true;
        ReceiptGridRow? rejected = null;
        try
        {
            var selected = DocumentGrid.SelectedItems?.OfType<ReceiptGridRow>().ToHashSet() ?? [];
            var rejectedRows = new List<ReceiptGridRow>();
            foreach (var item in _visible)
            {
                if (selected.Contains(item) == false)
                {
                    item.ApplySelected(false);
                }
            }

            foreach (var item in _visible)
            {
                if (selected.Contains(item) && item.ApplySelected(true) == false)
                {
                    rejectedRows.Add(item);
                }
            }

            foreach (var item in rejectedRows)
            {
                DocumentGrid.SelectedItems?.Remove(item);
            }

            rejected = rejectedRows.FirstOrDefault();
        }
        finally
        {
            _syncingSelection = false;
        }

        RefreshSelection();
        if (rejected is not null)
        {
            Allow(rejected);
        }
    }

    private void SyncGridSelectionFromPicks()
    {
        _syncingSelection = true;
        try
        {
            DocumentGrid.SelectedItems.Clear();
            foreach (var item in _visible)
            {
                if (item.IsSelected)
                {
                    DocumentGrid.SelectedItems.Add(item);
                }
            }
        }
        finally
        {
            _syncingSelection = false;
        }
    }

    private async void OnFilterClick(object? sender, RoutedEventArgs e)
    {
        _page = 1;
        await LoadAsync();
    }

    private void OnSearchChanged(object? sender, TextChangedEventArgs e) => QueueSearch();

    private void OnSearchKeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Enter)
        {
            _searchDelay?.Cancel();
            _page = 1;
            _ = LoadAsync();
        }
    }

    private void OnSearchClick(object? sender, RoutedEventArgs e)
    {
        _searchDelay?.Cancel();
        _page = 1;
        _ = LoadAsync();
    }

    private void OnPageSizeChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DocumentGrid is null || _datesReady == false)
        {
            return;
        }

        _page = 1;
        _ = LoadAsync();
    }

    private void OnFirstPageClick(object? sender, RoutedEventArgs e) => GoToPage(1);

    private void OnPreviousPageClick(object? sender, RoutedEventArgs e) => GoToPage(_page - 1);

    private void OnNextPageClick(object? sender, RoutedEventArgs e) => GoToPage(_page + 1);

    private void OnLastPageClick(object? sender, RoutedEventArgs e) => GoToPage(_totalPages);

    private void GoToPage(int page)
    {
        var next = Math.Clamp(page, 1, Math.Max(1, _totalPages));
        if (next == _page)
        {
            return;
        }

        _page = next;
        _ = LoadAsync();
    }

    private int PageSize()
    {
        var text = (PageSizeBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
        return int.TryParse(text, out var size) && size > 0 ? size : 20;
    }

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
        var portugal = AppComposition.Services?.GetService<ISystemInformationService>()?.IsPortugal() == true;
        var pay = ReceiptPayDefaults.For(portugal, _net, selected[0].CurrencyId, _catalog.Currencies);
        ExchangeFields.IsVisible = ReceiptPayDefaults.ShowExchange(portugal);
        SystemAmount.IsVisible = ExchangeFields.IsVisible;
        _filling = true;
        PaymentMethodBox.ItemsSource = _catalog.PaymentMethods;
        PaymentMethodBox.SelectedItem = null;
        CurrencyBox.ItemsSource = _catalog.Currencies;
        CurrencyBox.SelectedItem = pay.Currency;
        ExchangeRate.Text = pay.Rate.ToString("0.00", CultureInfo.CurrentCulture);
        TotalPaid.Text = pay.Delivered.ToString("0.00", CultureInfo.CurrentCulture);
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

        var amount = DocumentLineCalculator.ReceiptAmount(_net, currencyAmount, rate);
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
            if (string.IsNullOrWhiteSpace(result.PdfPath) == false && OfficeSurface.Find(this) is { } office)
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

    private async void OnGridDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Visual visual && visual.FindAncestorOfType<Button>() is not null)
        {
            return;
        }

        if (DocumentGrid.SelectedItem is ReceiptGridRow row)
        {
            await OpenDocumentAsync(row);
        }
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

            if (OfficeSurface.Find(this) is { } office)
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

    private void QueueSearch()
    {
        _searchDelay?.Cancel();
        _searchDelay = new CancellationTokenSource();
        var token = _searchDelay.Token;
        _ = SearchAfterPauseAsync(token);
    }

    private async Task SearchAfterPauseAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(400, token);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if ((SearchBox.Text?.Trim() ?? string.Empty) == _loadedSearch)
        {
            return;
        }

        _page = 1;
        await LoadAsync();
    }

    private async Task LoadCoreAsync()
    {
        var start = StartDate.SelectedDate?.Date ?? new DateTime(DateTime.Today.Year, 1, 1);
        var end = EndDate.SelectedDate?.Date ?? DateTime.Today;
        if (start > end)
        {
            Notice.Text = "A data inicial não pode ser posterior à data final.";
            return;
        }

        var generation = ++_loadGeneration;
        var search = SearchBox.Text?.Trim();
        try
        {
            var service = AppComposition.Services?.GetService<IReceiptEmission>();
            _rows.Clear();
            if (service is not null)
            {
                var page = await service.ListAsync(start, end, _page, PageSize(), search, customerId: ListingFilters.CustomerId(CustomerBox));
                if (generation != _loadGeneration)
                {
                    return;
                }

                _rows.AddRange(page.Items);
                _page = Math.Max(1, page.Page);
                _totalPages = Math.Max(1, page.TotalPages);
            }

            _loadedSearch = search ?? string.Empty;
            Notice.Text = string.Empty;
            ApplySearch();
        }
        catch (Exception exception)
        {
            if (generation != _loadGeneration)
            {
                return;
            }

            Notice.Text = exception.Message;
        }
    }

    private void ApplySearch()
    {
        _syncingSelection = true;
        _visible.Clear();
        foreach (var row in _rows)
        {
            _visible.Add(new ReceiptGridRow(row, this));
        }

        _syncingSelection = false;
        PageLabel.Text = $"{_page} / {_totalPages}";
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

        var system = DocumentLineCalculator.ReceiptAmount(_net, paid, rate);
        if (system > _net && _net >= 0)
        {
            _filling = true;
            TotalPaid.Text = rate <= 0 ? "0.00" : DocumentLineCalculator.FullCurrencyAmount(_net, rate).ToString("0.00", CultureInfo.CurrentCulture);
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

        var amount = DocumentLineCalculator.ReceiptAmount(_net, currencyAmount, rate);
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

    private static bool TryMoney(string? text, out decimal value) => MoneyFormat.TryParse(text, out value);

    // -- Portrait layout --

    private void OnReceiptLayoutUpdated(object? sender, EventArgs e) => RelayoutForViewport();

    private void RelayoutForViewport()
    {
        if (PortraitChrome.TryUpdate(this, ref _viewport, out _portrait) == false)
        {
            return;
        }

        MobileBar.IsVisible = _portrait;
        DesktopTools.IsVisible = _portrait == false;
        DocumentGrid.IsVisible = _portrait == false;
        CardList.IsVisible = _portrait;
        PortraitChrome.FitOverlay(PayFrame, this, 1080, 720);
        if (_portrait == false)
        {
            FilterSheet.IsVisible = false;
            RestoreReceiptFilters();
        }
    }

    private void OnMobileSearchChanged(object? sender, TextChangedEventArgs e)
    {
        SearchBox.Text = MobileSearchBox.Text;
        OnSearchChanged(sender, e);
    }

    private void OnMobileFilterClick(object? sender, RoutedEventArgs e)
    {
        _receiptFiltersHome ??= ReceiptFilters.Parent as Panel;
        if (ReceiptFilters.Parent is Panel parent)
        {
            parent.Children.Remove(ReceiptFilters);
        }

        if (FilterSheetHost.Children.Contains(ReceiptFilters) == false)
        {
            FilterSheetHost.Children.Add(ReceiptFilters);
        }

        FilterSheet.IsVisible = true;
    }

    private void OnMobileFilterApplyClick(object? sender, RoutedEventArgs e)
    {
        OnFilterClick(sender, e);
        FilterSheet.IsVisible = false;
        RestoreReceiptFilters();
    }

    private void OnMobileFilterCloseClick(object? sender, RoutedEventArgs e)
    {
        FilterSheet.IsVisible = false;
        RestoreReceiptFilters();
    }

    private void RestoreReceiptFilters()
    {
        if (ReceiptFilters.Parent != FilterSheetHost)
        {
            return;
        }

        FilterSheetHost.Children.Remove(ReceiptFilters);
        _receiptFiltersHome?.Children.Insert(0, ReceiptFilters);
        DockPanel.SetDock(ReceiptFilters, Dock.Left);
    }

    private async void OnCardTapped(object? sender, TappedEventArgs e)
    {
        ReceiptGridRow? row = null;
        if (e.Source is Visual visual)
        {
            row = visual.FindAncestorOfType<Border>()?.Tag as ReceiptGridRow
                ?? visual.FindAncestorOfType<ListBoxItem>()?.Content as ReceiptGridRow;
        }

        row ??= CardList.SelectedItem as ReceiptGridRow;
        if (row is not null)
        {
            await OpenDocumentAsync(row);
        }
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
            _owner.OnRowPicked();
        }
    }

    public bool ApplySelected(bool selected)
    {
        if (selected && _selected == false && _owner.Allow(this) == false)
        {
            OnPropertyChanged(nameof(IsSelected));
            return false;
        }

        if (_selected == selected)
        {
            return true;
        }

        _selected = selected;
        OnPropertyChanged(nameof(IsSelected));
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
