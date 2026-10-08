using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using LogicPOS.App.Hardware;
using LogicPOS.Core;
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
    private TaskCompletionSource<bool>? _closed;
    private string? _pdfPath;
    private string? _documentTitle;
    private Guid? _documentId;
    private int _pageIndex;
    private double _zoom = 1;
    private ZoomMode _zoomMode = ZoomMode.Page;

    public PdfDocumentView()
    {
        InitializeComponent();
        PageScroll.SizeChanged += (_, _) => ApplyFit();
    }

    public Task ShowAsync(string path, string? title, Guid? documentId = null)
    {
        _pdfPath = path;
        _documentId = documentId;
        _documentTitle = string.IsNullOrWhiteSpace(title) ? "Documento" : title;
        TitleText.Text = _documentTitle;
        SetStatus("A carregar...");
        PageLabel.Text = string.Empty;
        ZoomLabel.Text = string.Empty;
        PreviousButton.IsEnabled = false;
        NextButton.IsEnabled = false;
        UpdateFitButtons();
        _zoomMode = ZoomMode.Page;
        _pageIndex = 0;
        ClearPages();
        _closed = new TaskCompletionSource<bool>();
        _ = LoadAsync(path);
        return _closed.Task;
    }

    private async Task LoadAsync(string path)
    {
        try
        {
            var bytes = await File.ReadAllBytesAsync(path);
            var pngPages = await Task.Run(() => RenderPages(bytes));
            ClearPages();
            foreach (var png in pngPages)
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

            SetStatus(string.Empty);
            ShowPage(0);
            ApplyFit();
        }
        catch (Exception)
        {
            SetStatus("Não foi possível mostrar o documento.");
        }
    }

    private static List<byte[]> RenderPages(byte[] pdf)
    {
        var pages = new List<byte[]>();
        var options = new PDFtoImage.RenderOptions { Dpi = 130 };
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

    private void OnPreviousClick(object? sender, RoutedEventArgs e)
    {
        if (_pageIndex > 0)
        {
            ShowPage(_pageIndex - 1);
        }
    }

    private void OnNextClick(object? sender, RoutedEventArgs e)
    {
        if (_pageIndex < _pages.Count - 1)
        {
            ShowPage(_pageIndex + 1);
        }
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
        _zoom = Math.Min(3, _zoom + 0.1);
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

        try
        {
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

                var (_, printError) = await FrontOfficePrinting.TryPrintInvoiceAsync(documentId);
                if (printError is null)
                {
                    SetStatus(string.Empty);
                    return;
                }

                if (printError.Contains("não configurada", StringComparison.OrdinalIgnoreCase) == false
                    && printError.Contains("não encontrada", StringComparison.OrdinalIgnoreCase) == false)
                {
                    SetStatus(printError);
                    return;
                }
            }

            await Task.Run(() => PdfWindowsPrint.Print(_pdfPath));
            SetStatus(string.Empty);
        }
        catch (Exception exception)
        {
            SetStatus(string.IsNullOrWhiteSpace(exception.Message)
                ? "Não foi possível imprimir o documento."
                : exception.Message);
        }
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
        var (_, reprintError) = await FrontOfficePrinting.TryReprintInvoiceAsync(documentId, copies, reason, secondCopy);
        SetStatus(reprintError ?? string.Empty);
    }

    private void OnReprintCancelClick(object? sender, RoutedEventArgs e)
    {
        ReprintOverlay.IsVisible = false;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        _closed?.TrySetResult(true);
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
        _zoom = _zoomMode == ZoomMode.Width
            ? Math.Clamp(fitWidth, 0.2, 3)
            : Math.Clamp(Math.Min(fitWidth, fitHeight), 0.2, 3);
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

    private void SetStatus(string text)
    {
        StatusText.Text = text;
        StatusText.IsVisible = string.IsNullOrEmpty(text) == false;
    }

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
