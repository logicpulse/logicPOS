using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LogicPOS.App.Hardware;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class DocumentsListing : UserControl
{
    private readonly List<PosDocumentRow> _rows = new();
    private readonly HashSet<Guid> _picked = new();
    private readonly List<Guid> _pendingPrintIds = new();
    private readonly List<(DataGridColumn Column, bool Visible)> _columnSnapshot = new();
    private int _page = 1;
    private string? _promptKind;
    private bool _datesReady;
    private bool _updatingColumnChecks;
    private bool _updatingPick;
    private ListingLoad _load = null!;

    public DocumentsListing()
    {
        InitializeComponent();
        _load = new ListingLoad(Busy);
        ListingNotice.HideWhenEmpty(ActionNotice);
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

    private async void OnFilterClick(object? sender, RoutedEventArgs e)
    {
        _page = 1;
        await LoadAsync();
    }

    private void OnSearchChanged(object? sender, TextChangedEventArgs e)
    {
        _page = 1;
        ShowPage();
    }

    private void OnSearchClick(object? sender, RoutedEventArgs e)
    {
        _page = 1;
        ShowPage();
    }

    private async void OnClearFilterClick(object? sender, RoutedEventArgs e) => await ClearAndReloadAsync();

    private async void OnResetClick(object? sender, RoutedEventArgs e)
    {
        foreach (var column in ChoosableColumns())
        {
            column.IsVisible = true;
        }

        await ClearAndReloadAsync();
    }

    private async Task ClearAndReloadAsync()
    {
        SearchBox.Text = string.Empty;
        StartDate.SelectedDate = new DateTime(DateTime.Today.Year, 1, 1);
        EndDate.SelectedDate = DateTime.Today;
        _page = 1;
        await LoadAsync();
    }

    private async void OnExportExcelClick(object? sender, RoutedEventArgs e) => await ExportAsync(true);

    private async void OnExportPdfClick(object? sender, RoutedEventArgs e) => await ExportAsync(false);

    private void OnPageSizeChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DocumentGrid is null)
        {
            return;
        }

        _page = 1;
        ShowPage();
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => RefreshSelectionLabel();

    private void OnPickAllClick(object? sender, RoutedEventArgs e)
    {
        if (_updatingPick || sender is not CheckBox)
        {
            return;
        }

        var select = PickAll.IsChecked == true;
        foreach (var row in Filtered())
        {
            if (select)
            {
                _picked.Add(row.Id);
            }
            else
            {
                _picked.Remove(row.Id);
            }
        }

        if (DocumentGrid.ItemsSource is IEnumerable<CheckedDocument> visible)
        {
            foreach (var item in visible)
            {
                item.ApplySelected(_picked.Contains(item.Source.Id));
            }
        }

        RefreshSelectionLabel();
        RefreshPickAll();
    }

    internal void OnRowPicked(Guid id, bool selected)
    {
        if (selected)
        {
            _picked.Add(id);
        }
        else
        {
            _picked.Remove(id);
        }

        RefreshSelectionLabel();
        RefreshPickAll();
    }

    private void OnFirstPageClick(object? sender, RoutedEventArgs e) => GoToPage(1);

    private void OnPreviousPageClick(object? sender, RoutedEventArgs e) => GoToPage(_page - 1);

    private void OnNextPageClick(object? sender, RoutedEventArgs e) => GoToPage(_page + 1);

    private void OnLastPageClick(object? sender, RoutedEventArgs e) => GoToPage(PageCount(Filtered().Count));

    private async void OnEditDraftClick(object? sender, RoutedEventArgs e)
    {
        if (FocusedRow() is not PosDocumentRow row || row.CanEdit == false)
        {
            ActionNotice.Text = "Selecione um rascunho.";
            return;
        }

        await EditDraftAsync(row);
    }

    private async void OnRowEditClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: CheckedDocument item } || item.Source.CanEdit == false)
        {
            return;
        }

        await EditDraftAsync(item.Source);
    }

    private async Task EditDraftAsync(PosDocumentRow row)
    {
        if (TopLevel.GetTopLevel(this) is not IOfficeSurface office)
        {
            return;
        }

        try
        {
            await office.ShowNewDocumentAsync(row.Id);
        }
        catch (Exception)
        {
            ActionNotice.Text = "Não foi possível abrir o rascunho.";
        }
    }

    private async void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        if (FocusedRow() is not PosDocumentRow row || row.CanDelete == false)
        {
            ActionNotice.Text = "Selecione um documento que possa ser anulado.";
            return;
        }

        if (row.CanEdit)
        {
            var documents = AppComposition.Services?.GetService<IPosDocumentService>();
            if (documents is null)
            {
                return;
            }

            await documents.DeleteDraftAsync(row.Id);
            Toast.Success(this, "Rascunho eliminado com sucesso.");
            await ReloadAsync();
            return;
        }

        OpenPrompt("cancel", "Anular documento", "Motivo da anulação");
    }

    private void OnEmailClick(object? sender, RoutedEventArgs e)
    {
        if (SelectedIds().Count == 0)
        {
            ActionNotice.Text = "Selecione um ou mais documentos.";
            return;
        }

        OpenPrompt("email", "Enviar email", "Email de destino");
    }

    private void OpenPrompt(string kind, string title, string label)
    {
        _promptKind = kind;
        PromptTitle.Text = title;
        PromptLabel.Text = label;
        PromptValue.Text = string.Empty;
        PromptValue.IsVisible = kind != "print";
        PrinterBox.IsVisible = kind == "print";
        PromptOverlay.IsVisible = true;
    }

    private async void OnPromptConfirmClick(object? sender, RoutedEventArgs e)
    {
        var kind = _promptKind;
        var value = PromptValue.Text?.Trim() ?? string.Empty;
        PromptOverlay.IsVisible = false;
        var documents = AppComposition.Services?.GetService<IPosDocumentService>();
        if (documents is null || kind is null)
        {
            return;
        }

        if (kind == "print")
        {
            var printer = PrinterBox.SelectedItem as string;
            var rows = _rows.Where(row => _pendingPrintIds.Contains(row.Id)).ToList();
            if (string.IsNullOrWhiteSpace(printer) || rows.Count == 0)
            {
                ActionNotice.Text = "Selecione a impressora.";
                return;
            }

            await PrintRowsAsync(rows, printer);
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            ActionNotice.Text = kind == "email" ? "Indique o email." : "Indique o motivo da anulação.";
            return;
        }

        if (kind == "email" && GtkFormRules.Validate("Clientes", new Dictionary<string, string> { ["Email"] = value, ["Name"] = "Cliente", ["FiscalNumber"] = "999999990" }) is string invalid)
        {
            ActionNotice.Text = invalid;
            return;
        }

        if (kind == "cancel")
        {
            if (FocusedRow() is not PosDocumentRow row)
            {
                return;
            }

            ActionNotice.Text = await documents.CancelDocumentAsync(row.Id, value) ?? "Documento anulado.";
            await ReloadAsync();
            return;
        }

        var error = await documents.SendDocumentsByEmailAsync(SelectedIds(), value);
        ActionNotice.Text = error ?? "Email enviado.";
    }

    private void OnPromptCancelClick(object? sender, RoutedEventArgs e) => PromptOverlay.IsVisible = false;

    private List<Guid> SelectedIds() => ChosenRows().Select(row => row.Id).ToList();

    private PosDocumentRow? FocusedRow()
    {
        return DocumentGrid.SelectedItem is CheckedDocument item ? item.Source : null;
    }

    private List<PosDocumentRow> ChosenRows()
    {
        var picked = _rows.Where(row => _picked.Contains(row.Id)).ToList();
        if (picked.Count > 0)
        {
            return picked;
        }

        return DocumentGrid.SelectedItems?.OfType<CheckedDocument>().Select(item => item.Source).ToList() ?? [];
    }

    private async void OnNewClick(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is IOfficeSurface office)
        {
            await office.ShowNewDocumentAsync();
        }
    }

    private async void OnOpenA4Click(object? sender, RoutedEventArgs e)
    {
        if (FocusedRow() is not PosDocumentRow row)
        {
            ActionNotice.Text = "Selecione um documento.";
            return;
        }

        await OpenPdfAsync(row);
    }

    private async void OnPrintClick(object? sender, RoutedEventArgs e)
    {
        var rows = ChosenRows();
        if (rows.Count == 0)
        {
            ActionNotice.Text = "Selecione um ou mais documentos.";
            return;
        }

        await PrintRowsAsync(rows, null);
    }

    private void OnPrintAsClick(object? sender, RoutedEventArgs e)
    {
        var rows = ChosenRows();
        if (rows.Count == 0)
        {
            ActionNotice.Text = "Selecione um ou mais documentos.";
            return;
        }

        var printers = InstalledPrinters();
        if (printers.Count == 0)
        {
            ActionNotice.Text = "Não há impressoras instaladas.";
            return;
        }

        _pendingPrintIds.Clear();
        _pendingPrintIds.AddRange(rows.Select(row => row.Id));
        PrinterBox.ItemsSource = printers;
        PrinterBox.SelectedIndex = 0;
        OpenPrompt("print", "Imprimir como", "Impressora");
    }

    private async Task PrintRowsAsync(IReadOnlyList<PosDocumentRow> rows, string? printer)
    {
        var sent = 0;
        var thermal = 0;
        string? thermalError = null;
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(printer))
            {
                var (handled, error) = await FrontOfficePrinting.TryPrintInvoiceAsync(row.Id);
                if (handled)
                {
                    if (error is null)
                    {
                        sent++;
                        thermal++;
                    }
                    else
                    {
                        thermalError ??= error;
                    }

                    continue;
                }
            }

            var path = await CreatePdfAsync(row);
            if (path is not null && PrintFile(path, printer))
            {
                sent++;
            }
        }

        var destination = string.IsNullOrWhiteSpace(printer)
            ? thermal == sent ? "a impressora térmica" : "a impressora predefinida"
            : printer;
        ActionNotice.Text = sent == rows.Count
            ? $"{sent} documento(s) enviado(s) para {destination}."
            : $"Enviados {sent} de {rows.Count} documentos."
              + (thermalError is null ? string.Empty : $" Erro ao imprimir: {thermalError}");
    }

    private static bool PrintFile(string path, string? printer)
    {
        try
        {
            var start = new System.Diagnostics.ProcessStartInfo(path)
            {
                UseShellExecute = true,
                Verb = string.IsNullOrWhiteSpace(printer) ? "print" : "printto"
            };
            if (string.IsNullOrWhiteSpace(printer) == false)
            {
                start.Arguments = "\"" + printer + "\"";
            }

            System.Diagnostics.Process.Start(start);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static List<string> InstalledPrinters()
    {
        try
        {
            return WindowsPrinters.Installed().ToList();
        }
        catch (Exception)
        {
            return [];
        }
    }

    private async void OnRowViewClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: CheckedDocument item })
        {
            return;
        }

        await OpenPdfAsync(item.Source);
    }

    private void OnChooseColumnsClick(object? sender, RoutedEventArgs e)
    {
        ColumnSearch.Text = string.Empty;
        _columnSnapshot.Clear();
        ColumnChecks.Children.Clear();
        var columns = ChoosableColumns().ToList();
        var selectAll = new CheckBox
        {
            Content = "Selecionar tudo",
            IsChecked = columns.All(column => column.IsVisible)
        };
        selectAll.IsCheckedChanged += OnSelectAllColumnsChanged;
        ColumnChecks.Children.Add(selectAll);
        foreach (var column in columns)
        {
            _columnSnapshot.Add((column, column.IsVisible));
            var box = new CheckBox
            {
                Content = column.Header?.ToString(),
                IsChecked = column.IsVisible,
                Tag = column,
                Classes = { "bo_doc_column_check" }
            };
            box.IsCheckedChanged += (_, _) => RefreshSelectAll();
            ColumnChecks.Children.Add(box);
        }

        ColumnChooser.IsVisible = true;
    }

    private void OnColumnSearchChanged(object? sender, TextChangedEventArgs e)
    {
        var term = ColumnSearch.Text?.Trim();
        foreach (var box in ColumnChecks.Children.OfType<CheckBox>())
        {
            if (box.Tag is not DataGridColumn)
            {
                continue;
            }

            var text = box.Content?.ToString() ?? string.Empty;
            box.IsVisible = string.IsNullOrEmpty(term) || text.Contains(term, StringComparison.OrdinalIgnoreCase);
        }

        RefreshSelectAll();
    }

    private void OnSelectAllColumnsChanged(object? sender, RoutedEventArgs e)
    {
        if (_updatingColumnChecks || sender is not CheckBox selectAll)
        {
            return;
        }

        _updatingColumnChecks = true;
        var check = selectAll.IsChecked == true;
        foreach (var box in ColumnChecks.Children.OfType<CheckBox>())
        {
            if (box.IsVisible && box.Tag is DataGridColumn)
            {
                box.IsChecked = check;
            }
        }

        _updatingColumnChecks = false;
    }

    private void OnColumnsOkClick(object? sender, RoutedEventArgs e)
    {
        foreach (var box in ColumnChecks.Children.OfType<CheckBox>())
        {
            if (box.Tag is DataGridColumn column)
            {
                column.IsVisible = box.IsChecked == true;
            }
        }

        ColumnChooser.IsVisible = false;
    }

    private void OnColumnsCancelClick(object? sender, RoutedEventArgs e)
    {
        foreach (var (column, visible) in _columnSnapshot)
        {
            column.IsVisible = visible;
        }

        ColumnChooser.IsVisible = false;
    }

    private void RefreshSelectAll()
    {
        if (_updatingColumnChecks)
        {
            return;
        }

        var boxes = ColumnChecks.Children.OfType<CheckBox>().Where(box => box.IsVisible && box.Tag is DataGridColumn).ToList();
        if (ColumnChecks.Children.OfType<CheckBox>().FirstOrDefault(box => box.Tag is not DataGridColumn) is not CheckBox selectAll)
        {
            return;
        }

        _updatingColumnChecks = true;
        selectAll.IsChecked = boxes.Count > 0 && boxes.All(box => box.IsChecked == true);
        _updatingColumnChecks = false;
    }

    private IEnumerable<DataGridColumn> ChoosableColumns()
    {
        return DocumentGrid.Columns.Where(column => column.Header is string header && header.Length > 0);
    }

    private async Task<string?> CreatePdfAsync(PosDocumentRow row)
    {
        var service = AppComposition.Services?.GetService<IPosDocumentService>();
        if (service is null)
        {
            return null;
        }

        try
        {
            var path = await service.CreateA4FileAsync(row.Id);
            if (string.IsNullOrWhiteSpace(path))
            {
                ActionNotice.Text = "Não foi possível gerar o PDF.";
                return null;
            }

            return path;
        }
        catch (Exception)
        {
            ActionNotice.Text = "Não foi possível gerar o PDF.";
            return null;
        }
    }

    private async Task OpenPdfAsync(PosDocumentRow row)
    {
        var path = await CreatePdfAsync(row);
        if (path is null)
        {
            return;
        }

        ActionNotice.Text = string.Empty;
        if (TopLevel.GetTopLevel(this) is IOfficeSurface office)
        {
            await office.ShowPdfAsync(path, row.Number);
        }
    }

    private Task LoadAsync() => _load.RunAsync(LoadCoreAsync);

    private async Task LoadCoreAsync()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        var start = StartDate.SelectedDate?.Date ?? new DateTime(DateTime.Today.Year, 1, 1);
        var end = EndDate.SelectedDate?.Date ?? DateTime.Today;
        if (end < start)
        {
            end = start;
            EndDate.SelectedDate = end;
        }

        try
        {
            var rows = await services.GetRequiredService<IPosDocumentService>().ListByPeriodAsync(start, end);
            _rows.Clear();
            _rows.AddRange(rows);
            _picked.RemoveWhere(id => _rows.Exists(row => row.Id == id) == false);
        }
        catch (Exception exception)
        {
            _rows.Clear();
            _picked.Clear();
            EmptyLabel.Text = exception.Message;
        }

        ActionNotice.Text = string.Empty;
        ShowPage();
    }

    private void GoToPage(int page)
    {
        var pages = PageCount(Filtered().Count);
        _page = Math.Clamp(page, 1, pages);
        ShowPage();
    }

    private void ShowPage()
    {
        var filtered = Filtered();
        var pages = PageCount(filtered.Count);
        if (_page > pages)
        {
            _page = pages;
        }

        var size = PageSize();
        DocumentGrid.ItemsSource = filtered
            .Skip((_page - 1) * size)
            .Take(size)
            .Select(row => new CheckedDocument(this, row, _picked.Contains(row.Id)))
            .ToList();
        PageLabel.Text = $"{_page} / {pages}";
        EmptyLabel.IsVisible = filtered.Count == 0;
        EmptyLabel.Text = "Não existem documentos neste período.";
        RefreshSelectionLabel();
        RefreshPickAll();
    }

    private List<PosDocumentRow> Filtered()
    {
        var term = SearchBox.Text?.Trim();
        if (string.IsNullOrEmpty(term))
        {
            return _rows;
        }

        return _rows.Where(row =>
                row.Number.Contains(term, StringComparison.OrdinalIgnoreCase)
                || row.EntityName.Contains(term, StringComparison.OrdinalIgnoreCase)
                || row.TaxId.Contains(term, StringComparison.OrdinalIgnoreCase)
                || row.Status.Contains(term, StringComparison.OrdinalIgnoreCase)
                || row.DocumentDateText.Contains(term, StringComparison.OrdinalIgnoreCase)
                || row.FinalTotalText.Contains(term, StringComparison.OrdinalIgnoreCase)
                || row.PaidAmountText.Contains(term, StringComparison.OrdinalIgnoreCase)
                || row.DebitAmountText.Contains(term, StringComparison.OrdinalIgnoreCase)
                || row.AssociatedDocuments.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private async Task ExportAsync(bool excel)
    {
        var columns = ChoosableColumns().Where(column => column.IsVisible).ToList();
        var headers = columns.Select(column => column.Header?.ToString() ?? string.Empty).ToList();
        var rows = Filtered().Select(row => (IReadOnlyList<string>)headers.Select(header => DocumentCell(row, header)).ToList()).ToList();
        await ListingFileExport.SaveAsync(TopLevel.GetTopLevel(this), "Documentos", excel, headers, rows);
    }

    private static string DocumentCell(PosDocumentRow row, string header) => header switch
    {
        "Data do documento" => row.DocumentDateText,
        "Número do Doc." => row.Number,
        "Estado" => row.Status,
        "Entidade" => row.EntityName,
        "NIF" => row.TaxId,
        "Total Final" => row.FinalTotalText,
        "Valor Pago" => row.PaidAmountText,
        "Valor em Dívida" => row.DebitAmountText,
        "Documentos Associados" => row.AssociatedDocuments,
        _ => string.Empty
    };

    private int PageSize()
    {
        var text = (PageSizeBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
        return int.TryParse(text, out var size) && size > 0 ? size : 20;
    }

    private int PageCount(int count)
    {
        var size = PageSize();
        return Math.Max(1, (int)Math.Ceiling(count / (double)size));
    }

    private void RefreshSelectionLabel()
    {
        var count = _picked.Count > 0 ? _rows.Count(row => _picked.Contains(row.Id)) : DocumentGrid.SelectedItems?.Count ?? 0;
        SelectionLabel.Text = $"{count} documento(s) selecionado(s)";
    }

    private void RefreshPickAll()
    {
        var filtered = Filtered();
        var selected = filtered.Count(row => _picked.Contains(row.Id));
        _updatingPick = true;
        PickAll.IsChecked = filtered.Count == 0 || selected == 0
            ? false
            : selected == filtered.Count
                ? true
                : null;
        _updatingPick = false;
    }

    private sealed class CheckedDocument : INotifyPropertyChanged
    {
        private readonly DocumentsListing _owner;
        private bool _selected;

        public CheckedDocument(DocumentsListing owner, PosDocumentRow source, bool selected)
        {
            _owner = owner;
            Source = source;
            _selected = selected;
        }

        public PosDocumentRow Source { get; }

        public bool IsSelected
        {
            get => _selected;
            set
            {
                if (_selected == value)
                {
                    return;
                }

                _selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                _owner.OnRowPicked(Source.Id, value);
            }
        }

        public string DocumentDateText => Source.DocumentDateText;

        public string Number => Source.Number;

        public string Status => Source.Status;

        public string EntityName => Source.EntityName;

        public string TaxId => Source.TaxId;

        public string FinalTotalText => Source.FinalTotalText;

        public string PaidAmountText => Source.PaidAmountText;

        public string DebitAmountText => Source.DebitAmountText;

        public string AssociatedDocuments => Source.AssociatedDocuments;

        public bool CanEdit => Source.CanEdit;

        public bool CanView => Source.CanView;

        public void ApplySelected(bool selected)
        {
            if (_selected == selected)
            {
                return;
            }

            _selected = selected;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
