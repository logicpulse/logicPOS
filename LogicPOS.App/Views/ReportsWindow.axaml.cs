using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Platform;
using Avalonia.Threading;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class ReportsWindow : UserControl
{
    private const string IconRoot = "avares://LogicPOS.App/Assets/Images/BackOffice/";

    private static readonly LookupOption AllOption = new() { Id = Guid.Empty, Label = "Todos" };

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
        EnglishUi.Apply(this);
        Period.ShowCaption = false;
        var month = DefaultPeriods.Month(DateTime.Today);
        Period.SetRange(month.Start, month.End);
        Period.PeriodChanged += (_, _) => RefreshQuickPeriodState();
        RefreshQuickPeriodState();
        TouchFields.Attach(Filters);
        ApplyFilterVisibility(null);
        ListingNotice.BindToast(
            Notice,
            this,
            skipProgress: true,
            keepInline: text => _selected is not null
                && string.Equals(text, _selected.Name, StringComparison.Ordinal));
        ListingNotice.BindToast(FiltersNotice, this, skipProgress: true);
        AttachedToVisualTree += (_, _) =>
        {
            RelayoutForViewport();
            LayoutUpdated += OnReportsLayoutUpdated;
        };
        DetachedFromVisualTree += (_, _) => LayoutUpdated -= OnReportsLayoutUpdated;
    }

    public async Task ShowReportAsync(string key)
    {
        await ShowAsync();
        var report = _catalog.FirstOrDefault(item => string.Equals(item.Key, key, StringComparison.Ordinal));
        if (report is null)
        {
            return;
        }

        _group = report.Group;
        BuildTabs(_catalog.Select(item => item.Group).Distinct().ToList());
        ShowGroup(_group);
        var button = _rows.FirstOrDefault(row => ReferenceEquals(row.Tag, report));
        if (button is not null)
        {
            await ActivateReportAsync(report, button);
        }
    }

    public async Task ShowAsync()
    {
        try
        {
            RelayoutForViewport();
            Notice.Text = string.Empty;
            FiltersNotice.Text = string.Empty;
            FiltersOverlay.IsVisible = false;
            EmptyDataOverlay.IsVisible = false;
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
        catch (Exception exception)
        {
            App.WriteCrash("ReportsWindow.ShowAsync", exception);
            FiltersOverlay.IsVisible = false;
            EmptyDataOverlay.IsVisible = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }
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
                Classes = { "bo_entity_tab", "bo_report_tab" },
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
        SubfamilyPanel.IsVisible = report?.ShowSubfamily == true || report?.ShowArticle == true;
        SerialPanel.IsVisible = report?.ShowSerial == true;
        DocumentNumberPanel.IsVisible = report?.ShowDocumentNumber == true;
        PackFilterRow(DatesRow, PeriodPanel, DocumentTypePanel, TerminalPanel);
        PackFilterRow(FieldsRow, CustomerPanel, VatPanel, WarehousePanel, ArticlePanel);
        PackFilterRow(ExtraRow, FamilyPanel, SubfamilyPanel, SerialPanel, DocumentNumberPanel);
    }

    /// <summary>
    /// Visible fields pack from the left. These rows stay StackPanels: a Grid whose
    /// child column is past ColumnDefinitions throws in MeasureCellsGroup every frame
    /// and leaves the reports dialog white and blocked.
    /// </summary>
    private void PackFilterRow(StackPanel row, params Control[] fields)
    {
        var any = false;
        foreach (var field in fields)
        {
            field.HorizontalAlignment = HorizontalAlignment.Left;
            field.Width = ReferenceEquals(field, PeriodPanel) ? double.NaN : 260;
            field.Margin = new Avalonia.Thickness(0, 0, field.IsVisible ? 12 : 0, 0);
            if (field.IsVisible)
            {
                any = true;
            }
        }

        row.IsVisible = any;
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
                     CustomerBox, ArticleBox, DocumentTypeBox, TerminalBox, VatBox, WarehouseBox, FamilyBox
                 })
        {
            box.ItemTemplate = template;
        }

        CustomerBox.ItemsSource = WithAllOption(await listing.LookupAsync("Clientes"));
        _reportArticles = await listing.LookupAsync("Artigos");
        DocumentTypeBox.ItemsSource = WithAllOption(await listing.LookupAsync("Tipo de documento"));
        TerminalBox.ItemsSource = WithAllOption(await listing.LookupAsync("Terminais"));
        VatBox.ItemsSource = WithAllOption(await listing.LookupAsync("Taxas de imposto"));
        WarehouseBox.ItemsSource = WithAllOption(await listing.LookupAsync("Armazém"));
        FamilyBox.ItemsSource = WithAllOption(await listing.LookupAsync("Famílias"));
        _reportGroups = await listing.LookupAsync("Subfamílias");
        ResetFilterDefaults();
        _lookupsLoaded = true;
    }

    private static IReadOnlyList<LookupOption> WithAllOption(IReadOnlyList<LookupOption> items)
    {
        var list = new List<LookupOption>(items.Count + 1) { AllOption };
        list.AddRange(items);
        return list;
    }

    private IReadOnlyList<LookupOption> _reportArticles = [];
    private IReadOnlyList<LookupOption> _reportGroups = [];

    private void ResetFilterDefaults()
    {
        var month = DefaultPeriods.Month(DateTime.Today);
        Period.SetRange(month.Start, month.End);
        ArticleBox.ItemsSource = WithAllOption(_reportArticles);
        ReportClassification.Content = new ArticleSubfamilyPicker(_reportGroups, "", false, selected =>
        {
            ArticleBox.ItemsSource = WithAllOption(_reportArticles.Where(item => selected is null || item.SubfamilyId == selected.Id).ToList());
            ArticleBox.SelectedIndex = 0;
        });
        foreach (var box in new[]
                 {
                     CustomerBox, ArticleBox, DocumentTypeBox, TerminalBox, VatBox, WarehouseBox, FamilyBox
                 })
        {
            if (box.ItemCount > 0)
            {
                box.SelectedIndex = 0;
            }
            else
            {
                box.SelectedItem = null;
            }
        }

        SerialBox.Text = string.Empty;
        DocumentNumberBox.Text = string.Empty;
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
        ResetFilterDefaults();
        FiltersOverlay.IsVisible = true;
        RelayoutForViewport();
    }

    private void OnFiltersCancelClick(object? sender, RoutedEventArgs e)
    {
        FiltersOverlay.IsVisible = false;
        EmptyDataOverlay.IsVisible = false;
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

        var missing = ReportFilterSelection.Missing(
            _selected.ShowCustomer,
            CustomerBox.SelectedItem is not null,
            _selected.CustomerIsSupplier,
            _selected.ShowArticle,
            ArticleBox.SelectedItem is not null);
        if (missing is not null)
        {
            FiltersNotice.Text = missing;
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
            SubfamilyId = SubfamilyPanel.IsVisible ? (ReportClassification.Content as ArticleSubfamilyPicker)?.Selected?.Id : null,
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

        SetActiveNotice(string.Empty);
        Busy.Show();
        try
        {
            var result = await service.GenerateAsync(_selected.Key, filters);
            if (string.Equals(result.Error, LocalReportService.NoRecordsError, StringComparison.Ordinal))
            {
                ShowNoRecordsAlert();
                return;
            }

            if (result.Succeeded == false || string.IsNullOrWhiteSpace(result.Message))
            {
                SetActiveNotice(result.Error ?? "Não há dados para este relatório no intervalo indicado.");
                return;
            }

            FiltersOverlay.IsVisible = false;
            if (PreviewRequested is not null)
            {
                await PreviewRequested.Invoke(result.Message, _selected.Name);
            }
        }
        finally
        {
            Busy.Hide();
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
        EmptyDataOverlay.IsVisible = false;
        Busy.Hide();
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private void ShowNoRecordsAlert()
    {
        SetActiveNotice(string.Empty);
        EmptyDataTitle.Text = PreferenceLabels.Text("global_information", "Informação");
        EmptyDataMessage.Text = PreferenceLabels.Text(
            "dialog_message_report_filter_no_records_with_criteria",
            "Sem registos. Verifique os filtros.");
        EmptyDataOverlay.IsVisible = true;
        // Focus Ok so Escape activates IsCancel and closes only this overlay.
        Dispatcher.UIThread.Post(() => EmptyDataOkButton.Focus(), DispatcherPriority.Input);
    }

    private void OnEmptyDataOkClick(object? sender, RoutedEventArgs e)
        => EmptyDataOverlay.IsVisible = false;

    private void OnQuickDayClick(object? sender, RoutedEventArgs e)
        => ApplyPreset(DateTime.Today, DateTime.Today);

    private void OnQuickMonthClick(object? sender, RoutedEventArgs e)
    {
        var month = DefaultPeriods.Month(DateTime.Today);
        ApplyPreset(month.Start, month.End.Date);
    }

    private void OnQuickYearClick(object? sender, RoutedEventArgs e)
    {
        var year = DefaultPeriods.Year(DateTime.Today);
        ApplyPreset(year.Start, year.End.Date);
    }

    private void ApplyPreset(DateTime startDay, DateTime endDay)
        => Period.SetRange(startDay.Date, endDay.Date.AddDays(1).AddSeconds(-1), raise: true);

    private void RefreshQuickPeriodState()
    {
        var today = DateTime.Today;
        var day = IsPreset(today, today);
        var monthRange = DefaultPeriods.Month(today);
        var yearRange = DefaultPeriods.Year(today);
        var month = IsPreset(monthRange.Start, monthRange.End.Date);
        var year = IsPreset(yearRange.Start, yearRange.End.Date);
        SetQuickOn(QuickDayButton, day);
        SetQuickOn(QuickMonthButton, month && day == false);
        SetQuickOn(QuickYearButton, year && month == false);
    }

    private bool IsPreset(DateTime startDay, DateTime endDay)
    {
        var end = endDay.Date.AddDays(1).AddSeconds(-1);
        return Period.Start == startDay.Date && Period.End == end;
    }

    private static void SetQuickOn(Button button, bool on)
    {
        if (on)
        {
            button.Classes.Add("bo_reports_quick_on");
        }
        else
        {
            button.Classes.Remove("bo_reports_quick_on");
        }
    }
}
