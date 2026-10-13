using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class ReportsWindow : UserControl
{
    private readonly List<ReportDefinition> _catalog = new();
    private readonly List<Button> _rows = new();
    private ReportDefinition? _selected;
    private string _group = string.Empty;

    public event Func<string, string?, Task>? PreviewRequested;

    public event EventHandler? Closed;

    public ReportsWindow()
    {
        InitializeComponent();
        StartDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        EndDate.SelectedDate = DateTime.Today;
        TouchFields.Attach(Filters);
        ApplyFilterVisibility(null);
    }

    public async Task ShowAsync()
    {
        Notice.Text = string.Empty;
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
                BuildTabs(groups);
                ShowGroup(tab);
                ApplyFilterVisibility(null);
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
            var button = new Button
            {
                Classes = { "bo_report_row" },
                Content = report.Name,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left
            };
            if (ReferenceEquals(_selected, current))
            {
                button.Classes.Add("bo_report_row_on");
            }

            button.Click += (_, _) => Select(current, button);
            _rows.Add(button);
            Groups.Children.Add(button);
        }
    }

    private void Select(ReportDefinition report, Button button)
    {
        _selected = report;
        foreach (var row in _rows)
        {
            row.Classes.Remove("bo_report_row_on");
        }

        button.Classes.Add("bo_report_row_on");
        ApplyFilterVisibility(report);
        Notice.Text = report.Name;
    }

    private void ApplyFilterVisibility(ReportDefinition? report)
    {
        var showDates = report?.ShowDates == true;
        StartDatePanel.IsVisible = showDates;
        EndDatePanel.IsVisible = showDates;
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
    }

    private async void OnGenerateClick(object? sender, RoutedEventArgs e)
    {
        if (_selected is null)
        {
            Notice.Text = "Escolha um relatório.";
            return;
        }

        var service = AppComposition.Services?.GetService<IReportService>();
        if (service is null)
        {
            Notice.Text = "Os relatórios ainda não estão disponíveis.";
            return;
        }

        var documentType = SelectedOption(DocumentTypeBox);
        var filters = new ReportFilterRequest
        {
            Start = StartDate.SelectedDate?.Date ?? DateTime.Today,
            End = EndDate.SelectedDate?.Date ?? DateTime.Today,
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

        Notice.Text = "A gerar...";
        var result = await service.GenerateAsync(_selected.Key, filters);
        if (result.Succeeded == false || string.IsNullOrWhiteSpace(result.Message))
        {
            Notice.Text = result.Error ?? "Não há dados para este relatório no intervalo indicado.";
            return;
        }

        Notice.Text = string.Empty;
        if (PreviewRequested is not null)
        {
            await PreviewRequested.Invoke(result.Message, _selected.Name);
        }
    }

    private static Guid? SelectedId(ComboBox box)
        => box.SelectedItem is LookupOption option && option.Id != Guid.Empty ? option.Id : null;

    private static LookupOption? SelectedOption(ComboBox box)
        => box.SelectedItem as LookupOption;

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Closed?.Invoke(this, EventArgs.Empty);
}
