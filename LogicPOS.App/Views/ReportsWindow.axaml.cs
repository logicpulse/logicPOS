using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Platform;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class ReportsWindow : UserControl
{
    private const string IconRoot = "avares://LogicPOS.App/Assets/Images/BackOffice/";

    private readonly List<ReportDefinition> _catalog = new();
    private readonly List<Button> _rows = new();
    private ReportDefinition? _selected;
    private string _group = string.Empty;
    private UiViewport _viewport;
    private bool _lookupsLoaded;

    public event Func<string, string?, Task>? PreviewRequested;

    public event EventHandler? Closed;

    public ReportsWindow()
    {
        InitializeComponent();
        Period.SetRange(
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
            DateTime.Today.AddDays(1).AddSeconds(-1));
        TouchFields.Attach(Filters);
        ApplyFilterVisibility(null);
        AttachedToVisualTree += (_, _) =>
        {
            RelayoutForViewport();
            LayoutUpdated += OnReportsLayoutUpdated;
        };
        DetachedFromVisualTree += (_, _) => LayoutUpdated -= OnReportsLayoutUpdated;
    }

    public async Task ShowAsync()
    {
        RelayoutForViewport();
        Notice.Text = string.Empty;
        FiltersNotice.Text = string.Empty;
        FiltersOverlay.IsVisible = false;
        _selected = null;
        _catalog.Clear();
        var service = AppComposition.Services?.GetService<IReportService>();
        _catalog.AddRange(service?.Catalog ?? ReportCatalog.All);
        var groups = _catalog.Select(item => item.Group).Distinct().ToList();
        _group = groups.FirstOrDefault() ?? string.Empty;
        BuildTabs(groups);
        ShowGroup(_group);
        ApplyFilterVisibility(null);
        await LoadLookupsAsync();
    }

    private void OnReportsLayoutUpdated(object? sender, EventArgs e) => RelayoutForViewport();

    private void RelayoutForViewport()
    {
        if (PortraitChrome.TryUpdate(this, ref _viewport, out var portrait) == false && ReportsFrame.Bounds.Width > 0)
        {
            if (FiltersOverlay.IsVisible)
            {
                PortraitChrome.FitToContent(FiltersFrame, this, 980, minWidth: 520);
            }

            return;
        }

        var bottomReserve = portrait ? PortraitChrome.FloatingToolbarReserve : 0;
        PortraitChrome.FitOverlay(ReportsFrame, this, 1080, 720, bottomReserve: bottomReserve);
        if (FiltersOverlay.IsVisible)
        {
            PortraitChrome.FitToContent(FiltersFrame, this, 980, minWidth: 520, bottomReserve: bottomReserve);
        }
    }

    private void BuildTabs(IReadOnlyList<string> groups)
    {
        TabBar.Children.Clear();
        foreach (var name in groups)
        {
            var tab = name;
            var button = new Button
            {
                Classes = { "bo_entity_tab" },
                Content = tab
            };
            if (tab == _group)
            {
                button.Classes.Add("bo_entity_tab_on");
            }

            button.Click += (_, _) =>
            {
                _group = tab;
                _selected = null;
                Notice.Text = string.Empty;
                BuildTabs(groups);
                ShowGroup(tab);
            };
            TabBar.Children.Add(button);
        }
    }

    private void ShowGroup(string group)
    {
        Groups.Children.Clear();
        _rows.Clear();
        foreach (var report in _catalog.Where(item => item.Group == group))
        {
            var current = report;
            var selected = ReferenceEquals(_selected, current);
            var button = new Button
            {
                Classes = { "bo_report_row" },
                Tag = current,
                Content = CreateReportRow(current, selected),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            if (selected)
            {
                button.Classes.Add("bo_report_row_on");
            }

            button.Click += async (_, _) => await ActivateReportAsync(current, button);
            _rows.Add(button);
            Groups.Children.Add(button);
        }
    }

    private static Control CreateReportRow(ReportDefinition report, bool selected)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Classes = { "bo_menu_row" }
        };

        var glyph = new Border { Classes = { "bo_menu_glyph", "bo_menu_glyph_svg" } };
        glyph.Child = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
        {
            Classes = { "bo_menu_glyph_icon" },
            Path = $"{IconRoot}{ResolveReportIcon(report.Icon, selected)}"
        };
        panel.Children.Add(glyph);
        panel.Children.Add(new TextBlock
        {
            Classes = { "bo_menu_label" },
            Text = report.Name,
            // Unselected gray rows must stay dark even if a parent Foreground is light.
            Foreground = selected
                ? new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#FFFFFF"))
                : new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#323537")),
            VerticalAlignment = VerticalAlignment.Center
        });
        return panel;
    }

    /// <summary>
    /// Light gray rows use dark (_b) SVGs; selected lime row uses white SVGs.
    /// </summary>
    private static string ResolveReportIcon(string? icon, bool selected)
    {
        var relative = string.IsNullOrWhiteSpace(icon)
            ? "Menu/menu_relatorios_relatorios.svg"
            : icon!;
        var candidate = selected ? ToLightIcon(relative) : ToDarkIcon(relative);
        return AssetLoader.Exists(new Uri($"{IconRoot}{candidate}")) ? candidate : relative;
    }

    private static string ToDarkIcon(string relative)
    {
        if (relative.EndsWith("_b.svg", StringComparison.OrdinalIgnoreCase))
        {
            return relative;
        }

        return relative.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
            ? string.Concat(relative.AsSpan(0, relative.Length - 4), "_b.svg")
            : relative;
    }

    private static string ToLightIcon(string relative)
    {
        if (relative.EndsWith("_b.svg", StringComparison.OrdinalIgnoreCase))
        {
            return string.Concat(relative.AsSpan(0, relative.Length - 6), ".svg");
        }

        return relative;
    }

    private void Select(ReportDefinition report, Button button)
    {
        _selected = report;
        foreach (var row in _rows)
        {
            row.Classes.Remove("bo_report_row_on");
            ApplyRowIcon(row, selected: false);
        }

        button.Classes.Add("bo_report_row_on");
        ApplyRowIcon(button, selected: true);
        Notice.Text = report.Name;
    }

    private static void ApplyRowIcon(Button button, bool selected)
    {
        if (button.Tag is not ReportDefinition report || button.Content is not Panel panel)
        {
            return;
        }

        var labelBrush = new Avalonia.Media.SolidColorBrush(
            Avalonia.Media.Color.Parse(selected ? "#FFFFFF" : "#323537"));

        foreach (var child in panel.Children)
        {
            if (child is Border { Child: Avalonia.Svg.Skia.Svg svg })
            {
                svg.Path = $"{IconRoot}{ResolveReportIcon(report.Icon, selected)}";
            }
            else if (child is TextBlock label && label.Classes.Contains("bo_menu_label"))
            {
                label.Foreground = labelBrush;
            }
        }
    }

    private async Task ActivateReportAsync(ReportDefinition report, Button button)
    {
        Select(report, button);
        if (report.HasFilters == false)
        {
            await GenerateSelectedAsync(BuildEmptyFilters());
            return;
        }

        OpenFiltersDialog(report);
    }

    private void ApplyFilterVisibility(ReportDefinition? report)
    {
        var showDates = report?.ShowDates == true;
        PeriodPanel.IsVisible = showDates;
        DocumentTypePanel.IsVisible = report?.ShowDocumentType == true;
        TerminalPanel.IsVisible = report?.ShowTerminal == true;
        CustomerPanel.IsVisible = report?.ShowCustomer == true;
        CustomerCaption.Text = report?.CustomerIsSupplier == true ? "Fornecedor" : "Cliente";
        VatPanel.IsVisible = report?.ShowVat == true;
        WarehousePanel.IsVisible = report?.ShowWarehouse == true;
        ArticlePanel.IsVisible = report?.ShowArticle == true;
        FamilyPanel.IsVisible = report?.ShowFamily == true;
        SubfamilyPanel.IsVisible = report?.ShowSubfamily == true;
        SerialPanel.IsVisible = report?.ShowSerial == true;
        DocumentNumberPanel.IsVisible = report?.ShowDocumentNumber == true;
    }

    private async Task LoadLookupsAsync()
    {
        if (_lookupsLoaded)
        {
            return;
        }

        var listing = AppComposition.Services?.GetService<IBackOfficeListingService>();
        if (listing is null)
        {
            return;
        }

        var template = new FuncDataTemplate<LookupOption>((item, _) => new TextBlock { Text = item?.Label ?? string.Empty });
        foreach (var box in new[]
                 {
                     CustomerBox, ArticleBox, DocumentTypeBox, TerminalBox, VatBox, WarehouseBox, FamilyBox, SubfamilyBox
                 })
        {
            box.ItemTemplate = template;
        }

        CustomerBox.ItemsSource = await listing.LookupAsync("Clientes");
        ArticleBox.ItemsSource = await listing.LookupAsync("Artigos");
        DocumentTypeBox.ItemsSource = await listing.LookupAsync("Tipo de documento");
        TerminalBox.ItemsSource = await listing.LookupAsync("Terminais");
        VatBox.ItemsSource = await listing.LookupAsync("Taxas de imposto");
        WarehouseBox.ItemsSource = await listing.LookupAsync("Armazém");
        FamilyBox.ItemsSource = await listing.LookupAsync("Famílias");
        SubfamilyBox.ItemsSource = await listing.LookupAsync("Subfamílias");
        _lookupsLoaded = true;
    }

    private async void OnGenerateClick(object? sender, RoutedEventArgs e)
    {
        if (_selected is null)
        {
            Notice.Text = "Escolha um relatório.";
            return;
        }

        var button = _rows.FirstOrDefault(row => ReferenceEquals(row.Tag, _selected));
        if (button is not null)
        {
            await ActivateReportAsync(_selected, button);
            return;
        }

        if (_selected.HasFilters == false)
        {
            await GenerateSelectedAsync(BuildEmptyFilters());
            return;
        }

        OpenFiltersDialog(_selected);
    }

    private void OpenFiltersDialog(ReportDefinition report)
    {
        FiltersNotice.Text = string.Empty;
        FiltersTitle.Text = "Filtros";
        FiltersReportName.Text = report.Name;
        ApplyFilterVisibility(report);
        FiltersOverlay.IsVisible = true;
        RelayoutForViewport();
    }

    private void OnFiltersCancelClick(object? sender, RoutedEventArgs e)
    {
        FiltersOverlay.IsVisible = false;
        FiltersNotice.Text = string.Empty;
        ApplyFilterVisibility(null);
    }

    private async void OnFiltersGenerateClick(object? sender, RoutedEventArgs e)
    {
        if (_selected is null)
        {
            FiltersNotice.Text = "Escolha um relatório.";
            return;
        }

        await GenerateSelectedAsync(BuildFiltersFromUi());
    }

    private static ReportFilterRequest BuildEmptyFilters()
        => new()
        {
            Start = DateTime.Today,
            End = DateTime.Today
        };

    private ReportFilterRequest BuildFiltersFromUi()
    {
        var documentType = SelectedOption(DocumentTypeBox);
        return new ReportFilterRequest
        {
            Start = Period.StartDate,
            End = Period.EndDate,
            CustomerId = SelectedId(CustomerBox),
            ArticleId = SelectedId(ArticleBox),
            TerminalId = SelectedId(TerminalBox),
            DocumentTypeId = documentType?.Id,
            DocumentTypeAcronym = string.IsNullOrWhiteSpace(documentType?.Meta) ? null : documentType.Meta,
            VatRateId = SelectedId(VatBox),
            WarehouseId = SelectedId(WarehouseBox),
            FamilyId = SelectedId(FamilyBox),
            SubfamilyId = SelectedId(SubfamilyBox),
            SerialNumber = SerialBox.Text?.Trim() ?? string.Empty,
            DocumentNumber = DocumentNumberBox.Text?.Trim() ?? string.Empty
        };
    }

    private async Task GenerateSelectedAsync(ReportFilterRequest filters)
    {
        if (_selected is null)
        {
            return;
        }

        var service = AppComposition.Services?.GetService<IReportService>();
        if (service is null)
        {
            SetActiveNotice("Os relatórios ainda não estão disponíveis.");
            return;
        }

        SetActiveNotice("A gerar...");
        var result = await service.GenerateAsync(_selected.Key, filters);
        if (result.Succeeded == false || string.IsNullOrWhiteSpace(result.Message))
        {
            SetActiveNotice(result.Error ?? "Não há dados para este relatório no intervalo indicado.");
            return;
        }

        SetActiveNotice(string.Empty);
        FiltersOverlay.IsVisible = false;
        if (PreviewRequested is not null)
        {
            await PreviewRequested.Invoke(result.Message, _selected.Name);
        }
    }

    private void SetActiveNotice(string text)
    {
        if (FiltersOverlay.IsVisible)
        {
            FiltersNotice.Text = text;
        }
        else
        {
            Notice.Text = text;
        }
    }

    private static Guid? SelectedId(ComboBox box)
        => box.SelectedItem is LookupOption option && option.Id != Guid.Empty ? option.Id : null;

    private static LookupOption? SelectedOption(ComboBox box)
        => box.SelectedItem as LookupOption;

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        FiltersOverlay.IsVisible = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }
}
