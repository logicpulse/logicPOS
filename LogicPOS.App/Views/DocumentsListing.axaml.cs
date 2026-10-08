using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using LogicPOS.App.Hardware;
using LogicPOS.Core;
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
    private int _totalPages = 1;
    private int _loadGeneration;
    private string _loadedSearch = string.Empty;
    private CancellationTokenSource? _searchDelay;
    private string? _promptKind;
    private bool _sendingEmail;
    private bool _datesReady;
    private bool _fillingFilters;
    private bool _filtersRequested;
    private Guid? _appliedCustomer;
    private bool _updatingColumnChecks;
    private bool _updatingPick;
    private ListingLoad _load = null!;

    public DocumentsListing()
    {
        InitializeComponent();
        ListingFilters.EnableCustomerSearch(CustomerBox);
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
        ReloadFromFilter();
    }

    private void OnTypeFilterChanged(object? sender, SelectionChangedEventArgs e) => ReloadFromFilter();

    private void ReloadFromFilter()
    {
        if (_fillingFilters || _datesReady == false)
        {
            return;
        }

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
            var types = await ListingFilters.DocumentTypesAsync();
            _fillingFilters = true;
            CustomerBox.ItemsSource = customers;
            TypeBox.ItemsSource = types;
            ListingFilters.SelectEveryone(CustomerBox);
            TypeBox.SelectedIndex = 0;
            _appliedCustomer = null;
        }
        catch (Exception exception)
        {
            ActionNotice.Text = exception.Message;
        }
        finally
        {
            _fillingFilters = false;
        }
    }

    private void ResetFilterBoxes()
    {
        _fillingFilters = true;
        ListingFilters.SelectEveryone(CustomerBox);
        if (TypeBox.Items.Count > 0)
        {
            TypeBox.SelectedIndex = 0;
        }

        _appliedCustomer = null;
        _fillingFilters = false;
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
        ResetFilterBoxes();
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
        _ = LoadAsync();
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

    private void OnLastPageClick(object? sender, RoutedEventArgs e) => GoToPage(_totalPages);

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

    private async void OnEmailClick(object? sender, RoutedEventArgs e)
    {
        var rows = ChosenRows();
        if (rows.Count == 0)
        {
            Toast.Warning(this, "Selecione um ou mais documentos.");
            return;
        }

        var documents = AppComposition.Services?.GetService<IPosDocumentService>();
        if (documents is null)
        {
            return;
        }

        var fiscalNumbers = rows.Select(row => row.TaxId).Where(value => string.IsNullOrWhiteSpace(value) == false).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var fiscalNumber = fiscalNumbers.Count == 1 ? fiscalNumbers[0] : null;
        EmailNotice.Text = string.Empty;
        PosEmailDraft draft;
        try
        {
            draft = await documents.PrepareDocumentEmailAsync(rows.Select(row => row.Number).ToList(), fiscalNumber);
        }
        catch (Exception exception)
        {
            draft = new PosEmailDraft
            {
                Subject = "Documentos",
                Body = string.Join(";\n", rows.Select(row => row.Number))
            };
            EmailNotice.Text = exception.Message;
        }

        EmailSubject.Text = draft.Subject;
        EmailTo.Text = draft.To;
        EmailCc.Text = string.Empty;
        EmailBcc.Text = string.Empty;
        EmailBody.Text = draft.Body;
        EmailOverlay.IsVisible = true;
        TouchFields.Attach(EmailOverlay);
    }

    private async void OnEmailConfirmClick(object? sender, RoutedEventArgs e)
    {
        if (_sendingEmail)
        {
            return;
        }

        var subject = EmailSubject.Text?.Trim() ?? string.Empty;
        var notice = string.IsNullOrWhiteSpace(subject) ? "Indique o assunto." : null;
        notice ??= InvalidEmails(EmailTo.Text, required: true);
        notice ??= InvalidEmails(EmailCc.Text, required: false);
        notice ??= InvalidEmails(EmailBcc.Text, required: false);
        if (notice is not null)
        {
            EmailNotice.Text = notice;
            return;
        }

        var documents = AppComposition.Services?.GetService<IPosDocumentService>();
        if (documents is null)
        {
            return;
        }

        _sendingEmail = true;
        try
        {
            EmailNotice.Text = string.Empty;
            var error = await documents.SendDocumentsByEmailAsync(
                SelectedIds(),
                EmailTo.Text!.Trim(),
                subject,
                EmailBody.Text ?? string.Empty,
                BlankToNull(EmailCc.Text),
                BlankToNull(EmailBcc.Text));
            if (error is not null)
            {
                EmailNotice.Text = error;
                Toast.Error(this, error);
                return;
            }

            EmailOverlay.IsVisible = false;
            Toast.Success(this, "O seu email foi enviado com sucesso!");
        }
        finally
        {
            _sendingEmail = false;
        }
    }

    private void OnEmailCancelClick(object? sender, RoutedEventArgs e) => EmailOverlay.IsVisible = false;

    private static string? BlankToNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? InvalidEmails(string? value, bool required)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return required ? "Indique o email." : null;
        }

        var emails = value.Split([',', ';'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (emails.Length == 0)
        {
            return required ? "Indique o email." : null;
        }

        foreach (var email in emails)
        {
            if (System.Net.Mail.MailAddress.TryCreate(email, out _) == false)
            {
                return "Email inválido.";
            }
        }

        return null;
    }

    private void OpenPrompt(string kind, string title, string label)
    {
        _promptKind = kind;
        PromptTitle.Text = title;
        PromptLabel.Text = label;
        PromptValue.Text = string.Empty;
        var reprint = kind == "reprint";
        var printer = kind == "print";
        PromptLabel.IsVisible = reprint == false && string.IsNullOrWhiteSpace(label) == false;
        PromptValue.IsVisible = reprint == false && printer == false;
        PrinterBox.IsVisible = printer;
        ReprintPanel.IsVisible = reprint;
        PromptFrame.Classes.Set("bo_reprint_prompt", reprint);
        PromptOverlay.IsVisible = true;
    }

    private async Task OpenReprintPromptAsync(PosDocumentRow row)
    {
        _pendingPrintIds.Clear();
        _pendingPrintIds.Add(row.Id);
        OpenPrompt("reprint", string.Format(PreferenceLabels.Text("window_title_dialog_document_finance_print", "Doc.Nº: {0}"), row.Number), string.Empty);
        await BuildReprintOptionsAsync(row.Id);
    }

    private async Task BuildReprintOptionsAsync(Guid documentId)
    {
        ReprintCopyChecks.Children.Clear();
        var documents = AppComposition.Services?.GetService<IPosDocumentService>();
        var options = documents is null ? null : await documents.GetPrintDialogOptionsAsync(documentId);
        var copyCount = options?.PrintCopies ?? 1;
        var requestMotive = options?.PrintRequestMotive ?? true;
        var titles = new[]
        {
            PreferenceLabels.Text("global_print_copy_title1", "Original"),
            PreferenceLabels.Text("global_print_copy_title2", "Duplicado"),
            PreferenceLabels.Text("global_print_copy_title3", "Triplicado"),
            PreferenceLabels.Text("global_print_copy_title4", "Quadriplicado")
        };

        for (var index = 0; index < titles.Length; index++)
        {
            var enabled = index < copyCount;
            var box = new CheckBox
            {
                Content = titles[index],
                IsChecked = enabled,
                IsEnabled = enabled,
                Classes = { "bo_entity_input" }
            };
            if (index == 0)
            {
                // GTK: Original stays checked when Segunda via is off.
                box.IsCheckedChanged += (_, _) =>
                {
                    if (ReprintSecondCopy.IsChecked != true && box.IsChecked != true)
                    {
                        box.IsChecked = true;
                    }
                };
            }

            ReprintCopyChecks.Children.Add(box);
        }

        ReprintCopiesLabel.Text = PreferenceLabels.Text("global_print_copies", "Cópias");
        ReprintSecondCopy.Content = PreferenceLabels.Text("global_second_copy", "Segunda via");
        ReprintSecondCopy.IsVisible = requestMotive;
        ReprintSecondCopy.IsChecked = true;
        ReprintMotiveLabel.Text = PreferenceLabels.Text("global_reprint_original_motive", "Motivo da re-impressão do original");
        ReprintMotive.Text = string.Empty;
        ReprintSecondCopy.IsCheckedChanged -= OnReprintSecondCopyChanged;
        ReprintSecondCopy.IsCheckedChanged += OnReprintSecondCopyChanged;
        UpdateReprintMotiveState();
    }

    private void OnReprintSecondCopyChanged(object? sender, RoutedEventArgs e) => UpdateReprintMotiveState();

    private void UpdateReprintMotiveState()
    {
        var secondCopy = ReprintSecondCopy.IsVisible == false || ReprintSecondCopy.IsChecked == true;
        ReprintMotiveLabel.IsEnabled = secondCopy == false;
        ReprintMotive.IsEnabled = secondCopy == false;
        if (secondCopy)
        {
            ReprintMotive.Text = string.Empty;
        }
    }

    private async void OnPromptConfirmClick(object? sender, RoutedEventArgs e)
    {
        var kind = _promptKind;
        var value = PromptValue.Text?.Trim() ?? string.Empty;
        var documents = AppComposition.Services?.GetService<IPosDocumentService>();
        if (documents is null || kind is null)
        {
            PromptOverlay.IsVisible = false;
            return;
        }

        if (kind == "reprint")
        {
            var documentId = _pendingPrintIds.FirstOrDefault();
            if (documentId == Guid.Empty)
            {
                ActionNotice.Text = "Selecione um documento.";
                PromptOverlay.IsVisible = false;
                return;
            }

            var copies = ReprintCopyChecks.Children.OfType<CheckBox>().Count(box => box.IsChecked == true && box.IsEnabled);
            if (copies < 1)
            {
                ActionNotice.Text = "Selecione pelo menos uma cópia.";
                return;
            }

            var secondCopy = ReprintSecondCopy.IsVisible == false || ReprintSecondCopy.IsChecked == true;
            var reason = ReprintMotive.Text?.Trim() ?? string.Empty;
            if (secondCopy == false && string.IsNullOrWhiteSpace(reason))
            {
                ActionNotice.Text = "Indique o motivo da reimpressão do original.";
                return;
            }

            PromptOverlay.IsVisible = false;
            var (_, reprintError) = await FrontOfficePrinting.TryReprintInvoiceAsync(documentId, copies, reason, secondCopy);
            ActionNotice.Text = reprintError ?? $"{copies} cópia(s) enviada(s) para a impressora térmica.";
            return;
        }

        PromptOverlay.IsVisible = false;

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

        if (kind != "cancel")
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            ActionNotice.Text = "Indique o motivo da anulação.";
            return;
        }

        if (FocusedRow() is not PosDocumentRow row)
        {
            return;
        }

        ActionNotice.Text = await documents.CancelDocumentAsync(row.Id, value) ?? "Documento anulado.";
        await ReloadAsync();
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

        if (rows.Count == 1 && await NeedsReprintDialogAsync(rows[0].Id))
        {
            await OpenReprintPromptAsync(rows[0]);
            return;
        }

        await PrintRowsAsync(rows, null);
    }

    private async void OnPrintAsClick(object? sender, RoutedEventArgs e)
    {
        var rows = ChosenRows();
        if (rows.Count == 0)
        {
            ActionNotice.Text = "Selecione um ou mais documentos.";
            return;
        }

        if (rows.Count == 1 && await NeedsReprintDialogAsync(rows[0].Id))
        {
            await OpenReprintPromptAsync(rows[0]);
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
        string? printError = null;
        var useThermal = string.IsNullOrWhiteSpace(printer) || await IsTerminalThermalPrinterAsync(printer);
        foreach (var row in rows)
        {
            if (useThermal)
            {
                var (handled, error) = await PrintThermalDocumentAsync(row.Id);
                if (handled)
                {
                    if (error is null)
                    {
                        sent++;
                        thermal++;
                    }
                    else
                    {
                        printError ??= error;
                    }

                    continue;
                }
            }

            var path = await CreatePdfAsync(row);
            if (path is null)
            {
                printError ??= "Não foi possível gerar o PDF.";
                continue;
            }

            try
            {
                await Task.Run(() => PdfWindowsPrint.Print(path, printer));
                sent++;
            }
            catch (Exception exception)
            {
                printError ??= exception.Message;
            }
        }

        var destination = useThermal && thermal == sent
            ? "a impressora térmica"
            : string.IsNullOrWhiteSpace(printer) ? "a impressora predefinida" : printer;
        ActionNotice.Text = sent == rows.Count
            ? $"{sent} documento(s) enviado(s) para {destination}."
            : $"Enviados {sent} de {rows.Count} documentos."
              + (printError is null ? string.Empty : $" Erro ao imprimir: {printError}");
    }

    private static async Task<(bool Handled, string? Error)> PrintThermalDocumentAsync(Guid documentId)
    {
        if (await NeedsReprintDialogAsync(documentId))
        {
            // Already printed: caller must open the reprint dialog (copies + motive).
            return (true, "Documento já impresso. Use Imprimir e indique o motivo da segunda via.");
        }

        return await FrontOfficePrinting.TryPrintInvoiceAsync(documentId);
    }

    private static async Task<bool> NeedsReprintDialogAsync(Guid documentId)
    {
        var source = AppComposition.Services?.GetService<IThermalPrintSource>();
        return source is not null && await source.WasPrintedAsync(documentId);
    }

    private static async Task<bool> IsTerminalThermalPrinterAsync(string? printerName)
    {
        if (string.IsNullOrWhiteSpace(printerName))
        {
            return false;
        }

        var source = AppComposition.Services?.GetService<IThermalPrintSource>();
        var settings = source is null ? null : await source.GetTerminalPrinterAsync();
        if (settings is null)
        {
            return false;
        }

        return string.Equals(settings.Designation, printerName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(settings.NetworkName, printerName, StringComparison.OrdinalIgnoreCase);
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

    private async void OnGridDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Visual visual && visual.FindAncestorOfType<Button>() is not null)
        {
            return;
        }

        if (DocumentGrid.SelectedItem is not CheckedDocument item)
        {
            return;
        }

        if (item.CanEdit)
        {
            await EditDraftAsync(item.Source);
            return;
        }

        await OpenPdfAsync(item.Source);
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
            await office.ShowPdfAsync(path, row.Number, row.Id);
        }
    }

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
        var services = AppComposition.Services;
        if (services is null)
        {
            return;
        }

        var generation = ++_loadGeneration;
        var start = StartDate.SelectedDate?.Date ?? new DateTime(DateTime.Today.Year, 1, 1);
        var end = EndDate.SelectedDate?.Date ?? DateTime.Today;
        if (end < start)
        {
            end = start;
            EndDate.SelectedDate = end;
        }

        var search = SearchBox.Text?.Trim();
        try
        {
            var page = await services.GetRequiredService<IPosDocumentService>().ListByPeriodAsync(
                start,
                end,
                _page,
                PageSize(),
                search,
                customerId: ListingFilters.CustomerId(CustomerBox),
                documentType: ListingFilters.TypeCode(TypeBox));
            if (generation != _loadGeneration)
            {
                return;
            }

            _rows.Clear();
            _rows.AddRange(page.Items);
            _page = Math.Max(1, page.Page);
            _totalPages = Math.Max(1, page.TotalPages);
            _loadedSearch = search ?? string.Empty;
            _picked.RemoveWhere(id => _rows.Exists(row => row.Id == id) == false);
            EmptyLabel.Text = "Não existem documentos neste período.";
        }
        catch (Exception exception)
        {
            if (generation != _loadGeneration)
            {
                return;
            }

            _rows.Clear();
            _picked.Clear();
            _totalPages = 1;
            EmptyLabel.Text = exception.Message;
        }

        ActionNotice.Text = string.Empty;
        ShowPage();
    }

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

    private void ShowPage()
    {
        DocumentGrid.ItemsSource = _rows
            .Select(row => new CheckedDocument(this, row, _picked.Contains(row.Id)))
            .ToList();
        PageLabel.Text = $"{_page} / {_totalPages}";
        EmptyLabel.IsVisible = _rows.Count == 0;
        if (string.IsNullOrEmpty(EmptyLabel.Text) || EmptyLabel.Text == "Não existem documentos neste período.")
        {
            EmptyLabel.Text = "Não existem documentos neste período.";
        }

        RefreshSelectionLabel();
        RefreshPickAll();
    }

    private List<PosDocumentRow> Filtered() => _rows;

    private async Task ExportAsync(bool excel)
    {
        var columns = ChoosableColumns().Where(column => column.IsVisible).ToList();
        var headers = columns.Select(column => column.Header?.ToString() ?? string.Empty).ToList();
        var source = await AllMatchingAsync();
        var rows = source.Select(row => (IReadOnlyList<string>)headers.Select(header => DocumentCell(row, header)).ToList()).ToList();
        await ListingFileExport.SaveAsync(TopLevel.GetTopLevel(this), "Documentos", excel, headers, rows);
    }

    private async Task<List<PosDocumentRow>> AllMatchingAsync()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return _rows.ToList();
        }

        var start = StartDate.SelectedDate?.Date ?? new DateTime(DateTime.Today.Year, 1, 1);
        var end = EndDate.SelectedDate?.Date ?? DateTime.Today;
        var search = SearchBox.Text?.Trim();
        var documents = services.GetRequiredService<IPosDocumentService>();
        var all = new List<PosDocumentRow>();
        for (var page = 1; page <= 40; page++)
        {
            var result = await documents.ListByPeriodAsync(
                start,
                end,
                page,
                50,
                search,
                customerId: ListingFilters.CustomerId(CustomerBox),
                documentType: ListingFilters.TypeCode(TypeBox));
            all.AddRange(result.Items);
            if (result.Items.Count == 0 || page >= result.TotalPages)
            {
                break;
            }
        }

        return all;
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
