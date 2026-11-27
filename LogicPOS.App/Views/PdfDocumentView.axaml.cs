using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using LogicPOS.App;
using LogicPOS.App.Hardware;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;

namespace LogicPOS.App.Views;

public partial class PdfDocumentView : UserControl
{
    private enum ZoomMode
    {
        Page,
        Width,
        Manual
    }

    private readonly List<PdfPage> _pages = new();
    private byte[]? _pdfBytes;
    private int _pageCount;
    private int _renderGeneration;
    private TaskCompletionSource<bool>? _closed;
    private string? _pdfPath;
    private string? _documentTitle;
    private string? _preferredPrinter;
    private Guid? _documentId;
    private bool _barcodeMode;
    private string? _barcodeTemplate;
    private IReadOnlyList<BarcodeLabelLine> _barcodeLabels = [];
    private bool _suppressTemplateChange;
    private int _pageIndex;
    private double _zoom = 1;
    private ZoomMode _zoomMode = ZoomMode.Page;
    private UiViewport _viewport;
    private IInputElement? _reprintReturnFocus;
    private bool _lastInputWasKeyboard;
    private NavigationMethod _reprintFocusMethod;

    public PdfDocumentView()
    {
        InitializeComponent();
        ListingNotice.BindToast(StatusText, this, skipProgress: true);
        PageScroll.SizeChanged += (_, _) => ApplyFit();
        AttachedToVisualTree += (_, _) =>
        {
            RelayoutForViewport();
            LayoutUpdated += OnPdfLayoutUpdated;
        };
        DetachedFromVisualTree += (_, _) => LayoutUpdated -= OnPdfLayoutUpdated;
        ReprintOverlay.PropertyChanged += (_, e) =>
        {
            if (e.Property == IsVisibleProperty)
            {
                OnReprintOverlayVisibilityChanged(ReprintOverlay.IsVisible);
            }
        };
        // A barcode-scanner Enter suffix must not toggle a copy checkbox; Space still toggles it.
        ReprintOverlay.AddHandler(KeyDownEvent, (_, e) =>
        {
            if (e.Key == Key.Enter && e.Source is CheckBox)
            {
                e.Handled = true;
            }
        }, RoutingStrategies.Tunnel);
        // Last input kind decides whether the overlay shows a focus ring (keyboard) or not (mouse/touch).
        AddHandler(KeyDownEvent, (_, _) => _lastInputWasKeyboard = true, RoutingStrategies.Tunnel, handledEventsToo: true);
        AddHandler(PointerPressedEvent, (_, _) => _lastInputWasKeyboard = false, RoutingStrategies.Tunnel, handledEventsToo: true);
    }

    public Task ShowAsync(
        string path,
        string? title,
        Guid? documentId = null,
        string? preferredPrinter = null,
        BarcodeLabelPrintResult? barcode = null)
    {
        RelayoutForViewport();
        _pdfPath = path;
        _documentId = documentId;
        _preferredPrinter = string.IsNullOrWhiteSpace(preferredPrinter) ? null : preferredPrinter.Trim();
        _barcodeMode = barcode is not null && barcode.Labels.Count > 0;
        _barcodeLabels = _barcodeMode ? barcode!.Labels : [];
        _barcodeTemplate = _barcodeMode
            ? BarcodeLabelPdf.NormalizeModel(barcode!.PreferredTemplate ?? barcode.Labels[0].PrintModel)
            : null;
        _documentTitle = string.IsNullOrWhiteSpace(title) ? "Documento" : title;
        TitleText.Text = _documentTitle;
        Busy.Show();
        PageLabel.Text = string.Empty;
        ZoomLabel.Text = string.Empty;
        PreviousButton.IsEnabled = false;
        NextButton.IsEnabled = false;
        UpdateFitButtons();
        LoadTemplateChoices();
        LoadPrinterChoices(ResolvePreferredPrinter());
        _zoomMode = ZoomMode.Page;
        _pageIndex = 0;
        _pdfBytes = null;
        _pageCount = 0;
        _renderGeneration++;
        ClearPages();
        ReprintOverlay.IsVisible = false;
        _closed = new TaskCompletionSource<bool>();
        _ = OpenAsync(path);
        return _closed.Task;
    }

    private void LoadTemplateChoices()
    {
        _suppressTemplateChange = true;
        try
        {
            if (_barcodeMode == false)
            {
                TemplateBox.IsVisible = false;
                TemplateBox.ItemsSource = null;
                TemplateBox.SelectedItem = null;
                return;
            }

            var options = BarcodeLabelPdf.Models.Select(item => item.Label).ToList();
            TemplateBox.ItemsSource = options;
            TemplateBox.IsVisible = true;
            TemplateBox.SelectedItem = BarcodeLabelPdf.LabelOf(_barcodeTemplate);
        }
        finally
        {
            _suppressTemplateChange = false;
        }
    }

    private string? ResolvePreferredPrinter()
    {
        if (_barcodeMode)
        {
            return PdfPrinterMemory.PrinterForTemplate(_barcodeTemplate)
                ?? _preferredPrinter
                ?? PdfPrinterMemory.LastA4Printer();
        }

        return PdfPrinterMemory.LastA4Printer() ?? _preferredPrinter;
    }

    private async Task<string?> ResolvePreferredPrinterAsync()
    {
        if (_barcodeMode)
        {
            return ResolvePreferredPrinter();
        }

        var thermalDocument = PosDocumentRules.UsesThermalPrinter(_documentTitle);
        return await FrontOfficePrinting.DefaultWindowsPrinterAsync(thermalDocument) ?? _preferredPrinter;
    }

    private async Task OpenAsync(string path)
    {
        try
        {
            LoadPrinterChoices(await ResolvePreferredPrinterAsync());
        }
        catch
        {
            LoadPrinterChoices(ResolvePreferredPrinter());
        }

        await LoadAsync(path);
    }

    private void LoadPrinterChoices(string? preferred)
    {
        IReadOnlyList<string> printers;
        try
        {
            printers = WindowsPrinters.Installed();
        }
        catch
        {
            printers = [];
        }

        var items = printers.ToList();
        if (string.IsNullOrWhiteSpace(preferred) == false
            && items.Any(item => string.Equals(item, preferred, StringComparison.OrdinalIgnoreCase)) == false)
        {
            items.Insert(0, preferred);
        }

        PrinterBox.ItemsSource = items;
        PrinterBox.IsVisible = items.Count > 0;
        if (items.Count == 0)
        {
            PrinterBox.SelectedItem = null;
            return;
        }

        var match = string.IsNullOrWhiteSpace(preferred)
            ? null
            : items.FirstOrDefault(item => string.Equals(item, preferred, StringComparison.OrdinalIgnoreCase));
        PrinterBox.SelectedItem = match ?? items[0];
    }

    private string? SelectedPrinter()
    {
        if (PrinterBox.SelectedItem is string selected && string.IsNullOrWhiteSpace(selected) == false)
        {
            return selected.Trim();
        }

        return _preferredPrinter;
    }

    private string? SelectedTemplate()
    {
        if (TemplateBox.SelectedItem is string label)
        {
            return BarcodeLabelPdf.NormalizeModel(label);
        }

        return _barcodeTemplate;
    }

    private async void OnTemplateChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressTemplateChange || _barcodeMode == false || _barcodeLabels.Count == 0)
        {
            return;
        }

        var template = SelectedTemplate();
        if (string.IsNullOrWhiteSpace(template)
            || string.Equals(template, _barcodeTemplate, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _barcodeTemplate = template;
        LoadPrinterChoices(ResolvePreferredPrinter());
        try
        {
            SetStatus("A regenerar etiqueta...");
            var path = await Task.Run(() => BarcodeLabelPdf.Write(_barcodeLabels, template));
            _pdfPath = path;
            ClearPages();
            await LoadAsync(path);
        }
        catch (Exception exception)
        {
            SetStatus(exception.Message);
        }
    }

    private void OnPdfLayoutUpdated(object? sender, EventArgs e) => RelayoutForViewport();

    private void RelayoutForViewport()
    {
        if (PortraitChrome.TryUpdate(this, ref _viewport, out var portrait) == false && PdfFrame.Bounds.Width > 0)
        {
            return;
        }

        var bottomReserve = portrait ? PortraitChrome.FloatingToolbarReserve : 0;
        PortraitChrome.FitOverlay(PdfFrame, this, 1200, 860, bottomReserve: bottomReserve);
    }

    private async Task LoadAsync(string path)
    {
        try
        {
            var bytes = await File.ReadAllBytesAsync(path);
            _pdfBytes = bytes;
            var dpi = _barcodeMode ? 220 : 130;
            _pageCount = await Task.Run(() => PDFtoImage.Conversion.GetPageCount(bytes));
            if (_pageCount <= 0)
            {
                Busy.Hide();
                SetStatus("Não foi possível mostrar o documento.");
                return;
            }

            // Stock reports can run to well over a hundred pages. Rasterizing every
            // page up front exhausts memory and the preview never appears.
            if (_pageCount <= 8)
            {
                var pngPages = await Task.Run(() => RenderPages(bytes, dpi));
                ClearPages();
                foreach (var png in pngPages)
                {
                    AddPage(png);
                }

                Busy.Hide();
                SetStatus(string.Empty);
                ShowPage(0);
                ApplyFit();
                return;
            }

            await ShowSinglePageAsync(0);
        }
        catch (Exception)
        {
            Busy.Hide();
            SetStatus("Não foi possível mostrar o documento.");
        }
    }

    private static List<byte[]> RenderPages(byte[] pdf, int dpi = 130)
    {
        var pages = new List<byte[]>();
        var options = new PDFtoImage.RenderOptions { Dpi = Math.Clamp(dpi, 96, 300) };
        foreach (var image in PDFtoImage.Conversion.ToImages(pdf, password: null, options: options))
        {
            using (image)
            using (var data = image.Encode(SKEncodedImageFormat.Png, 90))
            {
                pages.Add(data.ToArray());
            }
        }

        return pages;
    }

    private async void OnPreviousClick(object? sender, RoutedEventArgs e)
    {
        if (_pageIndex <= 0)
        {
            return;
        }

        if (_pdfBytes is not null && _pageCount > 8)
        {
            await ShowSinglePageAsync(_pageIndex - 1);
            return;
        }

        ShowPage(_pageIndex - 1);
    }

    private async void OnNextClick(object? sender, RoutedEventArgs e)
    {
        var last = (_pdfBytes is not null && _pageCount > 8 ? _pageCount : _pages.Count) - 1;
        if (_pageIndex >= last)
        {
            return;
        }

        if (_pdfBytes is not null && _pageCount > 8)
        {
            await ShowSinglePageAsync(_pageIndex + 1);
            return;
        }

        ShowPage(_pageIndex + 1);
    }

    private void OnZoomOutClick(object? sender, RoutedEventArgs e)
    {
        _zoomMode = ZoomMode.Manual;
        _zoom = Math.Max(0.2, _zoom - 0.1);
        UpdateFitButtons();
        ApplyZoom();
    }

    private void OnZoomInClick(object? sender, RoutedEventArgs e)
    {
        _zoomMode = ZoomMode.Manual;
        _zoom = Math.Min(_barcodeMode ? 10 : 3, _zoom + 0.1);
        UpdateFitButtons();
        ApplyZoom();
    }

    private void OnFitPageClick(object? sender, RoutedEventArgs e)
    {
        _zoomMode = ZoomMode.Page;
        ApplyFit();
    }

    private void OnFitWidthClick(object? sender, RoutedEventArgs e)
    {
        _zoomMode = ZoomMode.Width;
        ApplyFit();
    }

    private async void OnExportClick(object? sender, RoutedEventArgs e)
    {
        if (_pdfPath is null || TopLevel.GetTopLevel(this) is not { } top)
        {
            return;
        }

        var file = await top.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Salvar Documento",
            SuggestedFileName = SuggestedFileName(),
            DefaultExtension = "pdf",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("PDF") { Patterns = new[] { "*.pdf" } }
            }
        });
        if (file is null)
        {
            return;
        }

        try
        {
            await using var destination = await file.OpenWriteAsync();
            await using var source = File.OpenRead(_pdfPath);
            await source.CopyToAsync(destination);
            SetStatus(string.Empty);
        }
        catch (Exception)
        {
            SetStatus("Erro ao salvar documento");
        }
    }

    private async void OnPrintClick(object? sender, RoutedEventArgs e)
    {
        if (_pdfPath is null)
        {
            return;
        }

        var printer = SelectedPrinter();
        var thermalDocument = PosDocumentRules.UsesThermalPrinter(_documentTitle);
        var selectedIsThermal = await FrontOfficePrinting.NamesTerminalThermalPrinterAsync(printer);
        var backOfficeOnly = await IsBackOfficeOnlyAsync();
        if (_documentId is Guid documentId && documentId != Guid.Empty)
        {
            var source = AppComposition.Services?.GetService<IThermalPrintSource>();
            if (source is not null && await source.WasPrintedAsync(documentId))
            {
                ReprintTitle.Text = string.Format(
                    PreferenceLabels.Text("window_title_dialog_document_finance_print", "Doc.Nº: {0}"),
                    string.IsNullOrWhiteSpace(_documentTitle) ? "—" : _documentTitle);
                await BuildReprintOptionsAsync(documentId);
                ReprintOverlay.IsVisible = true;
                return;
            }

            if (backOfficeOnly == false && thermalDocument && (string.IsNullOrWhiteSpace(printer) || selectedIsThermal))
            {
                Busy.Show("A imprimir...");
                try
                {
                    var (handled, printError) = await FrontOfficePrinting.TryPrintInvoiceAsync(documentId);
                    if (handled && printError is null)
                    {
                        SetStatus(string.Empty);
                        PrintUi.Success(this, "Documento enviado para a impressora.");
                        return;
                    }

                    if (FrontOfficePrinting.YieldsToPdfPrint(handled, printError) == false)
                    {
                        SetStatus(string.Empty);
                        PrintUi.Error(this, printError!);
                        return;
                    }
                }
                finally
                {
                    Busy.Hide();
                }
            }
        }

        float? labelWidthMm = null;
        float? labelHeightMm = null;
        if (_barcodeMode)
        {
            var size = BarcodeLabelPdf.SizeMm(SelectedTemplate() ?? _barcodeTemplate);
            labelWidthMm = size.WidthMm;
            labelHeightMm = size.HeightMm;
        }

        await PrintUi.RunAsync(this, Busy, async () =>
        {
            await Task.Run(() =>
            {
                if (PdfWindowsPrint.PrintOrOpen(_pdfPath, printer, labelWidthMm, labelHeightMm) == false)
                {
                    throw new InvalidOperationException("Não foi possível enviar o documento para a impressora.");
                }
            });
            if (_barcodeMode)
            {
                PdfPrinterMemory.RememberTemplate(SelectedTemplate(), printer);
            }
            else if (thermalDocument == false || selectedIsThermal == false)
            {
                PdfPrinterMemory.RememberA4(printer);
            }

            if (_barcodeMode == false && _documentId is Guid documentId && documentId != Guid.Empty)
            {
                var printSource = AppComposition.Services?.GetService<IThermalPrintSource>();
                var wasPrinted = printSource is not null && await printSource.WasPrintedAsync(documentId);
                await FrontOfficePrinting.TryRegisterA4PrintAsync(documentId, secondPrint: wasPrinted);
            }

            SetStatus(string.Empty);
        });
    }

    private static async Task<bool> IsBackOfficeOnlyAsync()
    {
        var mode = AppComposition.Services?.GetService<IOperationMode>();
        return mode is not null && await mode.IsBackOfficeOnlyAsync();
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

    /// <summary>
    /// Keeps keyboard focus inside the Segunda via overlay while it is shown (the viewer body is
    /// disabled; the title bar stays enabled so the IsCancel close button still handles Escape)
    /// and returns focus to the control that opened it when it closes.
    /// </summary>
    private void OnReprintOverlayVisibilityChanged(bool visible)
    {
        if (visible)
        {
            _reprintReturnFocus = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement();
            _reprintFocusMethod = _lastInputWasKeyboard ? NavigationMethod.Tab : NavigationMethod.Unspecified;
            ViewerBody.IsEnabled = false;
            // First input control, never Ok: Enter must not reprint by accident.
            Dispatcher.UIThread.Post(() => FirstReprintInput()?.Focus(_reprintFocusMethod), DispatcherPriority.Loaded);
            return;
        }

        ViewerBody.IsEnabled = true;
        if (_reprintReturnFocus is Control { IsEffectivelyVisible: true, IsEffectivelyEnabled: true } target)
        {
            target.Focus(_lastInputWasKeyboard ? NavigationMethod.Tab : NavigationMethod.Unspecified);
        }

        _reprintReturnFocus = null;
    }

    private Control? FirstReprintInput()
    {
        if (ReprintOverlay.IsVisible == false)
        {
            return null;
        }

        return ReprintCopyChecks.Children.OfType<Control>()
            .Append(ReprintSecondCopy)
            .Append(ReprintMotive)
            .FirstOrDefault(control => control.IsEffectivelyVisible && control.IsEffectivelyEnabled);
    }

    private void UpdateReprintMotiveState()
    {
        var secondCopy = ReprintSecondCopy.IsVisible == false || ReprintSecondCopy.IsChecked == true;
        var needMotive = secondCopy == false;
        ReprintMotiveLabel.IsEnabled = needMotive;
        ReprintMotiveLabel.IsVisible = needMotive;
        ReprintMotive.IsEnabled = needMotive;
        ReprintMotive.IsVisible = needMotive;
        if (needMotive == false)
        {
            ReprintMotive.Text = string.Empty;
        }

        if (ReprintMotive.Parent is DockPanel row && row.Classes.Contains("bo_touch_row"))
        {
            row.IsVisible = needMotive;
        }
    }

    private async void OnReprintConfirmClick(object? sender, RoutedEventArgs e)
    {
        if (_documentId is not Guid documentId || documentId == Guid.Empty)
        {
            ReprintOverlay.IsVisible = false;
            return;
        }

        var copies = ReprintCopyChecks.Children.OfType<CheckBox>().Count(box => box.IsChecked == true && box.IsEnabled);
        if (copies < 1)
        {
            SetStatus("Selecione pelo menos uma cópia.");
            return;
        }

        var secondCopy = ReprintSecondCopy.IsVisible == false || ReprintSecondCopy.IsChecked == true;
        var reason = ReprintMotive.Text?.Trim() ?? string.Empty;
        if (secondCopy == false && string.IsNullOrWhiteSpace(reason))
        {
            SetStatus("Indique o motivo da reimpressão do original.");
            return;
        }

        ReprintOverlay.IsVisible = false;
        Busy.Show("A imprimir...");
        try
        {
            var useA4 = await FrontOfficePrinting.ShouldUseA4PrintAsync(_documentTitle);
            string? reprintError;
            if (useA4)
            {
                var printer = SelectedPrinter();
                var (_, error, cancelled) = await FrontOfficePrinting.TryReprintA4Async(
                    documentId, copies, reason, secondCopy, printer);
                reprintError = error;
                if (cancelled)
                {
                    SetStatus(string.Empty);
                    return;
                }
            }
            else
            {
                (_, reprintError) = await FrontOfficePrinting.TryReprintInvoiceAsync(documentId, copies, reason, secondCopy);
            }

            SetStatus(string.Empty);
            if (reprintError is null)
            {
                PrintUi.Success(this, $"{copies} cópia(s) enviada(s) para a impressora.");
            }
            else
            {
                PrintUi.Error(this, reprintError);
            }
        }
        finally
        {
            Busy.Hide();
        }
    }

    private void OnReprintCancelClick(object? sender, RoutedEventArgs e)
    {
        ReprintOverlay.IsVisible = false;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        if (ReprintOverlay.IsVisible)
        {
            ReprintOverlay.IsVisible = false;
            return;
        }

        _renderGeneration++;
        Busy.Hide();
        _closed?.TrySetResult(true);
    }

    private async Task ShowSinglePageAsync(int index)
    {
        if (_pdfBytes is null || _pageCount <= 0)
        {
            return;
        }

        index = Math.Clamp(index, 0, _pageCount - 1);
        var generation = ++_renderGeneration;
        Busy.Show();
        PreviousButton.IsEnabled = false;
        NextButton.IsEnabled = false;
        try
        {
            var dpi = _barcodeMode ? 220 : 130;
            var png = await Task.Run(() => RenderPage(_pdfBytes, index, dpi));
            if (generation != _renderGeneration)
            {
                return;
            }

            ClearPages();
            AddPage(png);
            _pageIndex = index;
            PageLabel.Text = $"{_pageIndex + 1} / {_pageCount}";
            PreviousButton.IsEnabled = _pageIndex > 0;
            NextButton.IsEnabled = _pageIndex < _pageCount - 1;
            Busy.Hide();
            SetStatus(string.Empty);
            ApplyFit();
        }
        catch (Exception)
        {
            if (generation == _renderGeneration)
            {
                Busy.Hide();
                SetStatus("Não foi possível mostrar o documento.");
            }
        }
    }

    private void AddPage(byte[] png)
    {
        var bitmap = new Bitmap(new MemoryStream(png));
        var view = new Image
        {
            Source = bitmap,
            Classes = { "bo_pdf_page" }
        };
        var sheet = new Border
        {
            Classes = { "bo_pdf_sheet" },
            Child = view
        };
        _pages.Add(new PdfPage(bitmap, view));
        PagesHost.Children.Add(sheet);
    }

    private static byte[] RenderPage(byte[] pdf, int index, int dpi)
    {
        var options = new PDFtoImage.RenderOptions { Dpi = Math.Clamp(dpi, 96, 300) };
        using var image = PDFtoImage.Conversion.ToImage(pdf, index, password: null, options: options);
        using var data = image.Encode(SKEncodedImageFormat.Png, 90);
        return data.ToArray();
    }

    private void ShowPage(int index)
    {
        if (_pages.Count == 0)
        {
            PageLabel.Text = string.Empty;
            PreviousButton.IsEnabled = false;
            NextButton.IsEnabled = false;
            return;
        }

        _pageIndex = Math.Clamp(index, 0, _pages.Count - 1);
        PageLabel.Text = $"{_pageIndex + 1} / {_pages.Count}";
        PreviousButton.IsEnabled = _pageIndex > 0;
        NextButton.IsEnabled = _pageIndex < _pages.Count - 1;
        _pages[_pageIndex].View.BringIntoView();
    }

    private void ApplyFit()
    {
        if (_zoomMode == ZoomMode.Manual || _pages.Count == 0)
        {
            return;
        }

        var availableWidth = PageScroll.Bounds.Width - 48;
        var availableHeight = PageScroll.Bounds.Height - 24;
        if (availableWidth < 200 || availableHeight < 200)
        {
            return;
        }

        var page = _pages[0].Bitmap.Size;
        var fitWidth = availableWidth / page.Width;
        var fitHeight = availableHeight / page.Height;
        // Label pages are small (40x30 / 100x50 mm); allow higher zoom so they fill the preview.
        var maxZoom = _barcodeMode ? 10.0 : 3.0;
        _zoom = _zoomMode == ZoomMode.Width
            ? Math.Clamp(fitWidth, 0.2, maxZoom)
            : Math.Clamp(Math.Min(fitWidth, fitHeight), 0.2, maxZoom);
        UpdateFitButtons();
        ApplyZoom();
    }

    private void ApplyZoom()
    {
        foreach (var page in _pages)
        {
            page.View.Width = page.Bitmap.Size.Width * _zoom;
        }

        ZoomLabel.Text = $"{_zoom * 100:0}%";
    }

    private string SuggestedFileName()
    {
        var name = string.IsNullOrWhiteSpace(_documentTitle) ? "Documento" : _documentTitle;
        foreach (var character in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(character, '_');
        }

        return name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? name : name + ".pdf";
    }

    private void SetStatus(string text) => StatusText.Text = text;

    private void UpdateFitButtons()
    {
        SetActive(FitPageButton, _zoomMode == ZoomMode.Page);
        SetActive(FitWidthButton, _zoomMode == ZoomMode.Width);
    }

    private static void SetActive(Button button, bool active)
    {
        if (active)
        {
            if (button.Classes.Contains("bo_pdf_tool_on") == false)
            {
                button.Classes.Add("bo_pdf_tool_on");
            }
        }
        else
        {
            button.Classes.Remove("bo_pdf_tool_on");
        }
    }

    private void ClearPages()
    {
        PagesHost.Children.Clear();
        foreach (var page in _pages)
        {
            page.Bitmap.Dispose();
        }

        _pages.Clear();
    }

    private sealed class PdfPage
    {
        public PdfPage(Bitmap bitmap, Image view)
        {
            Bitmap = bitmap;
            View = view;
        }

        public Bitmap Bitmap { get; }

        public Image View { get; }
    }
}
