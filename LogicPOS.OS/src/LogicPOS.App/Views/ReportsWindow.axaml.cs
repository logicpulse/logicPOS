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
        CustomerPanel.IsVisible = report.NeedsCustomer;
        ArticlePanel.IsVisible = report.NeedsArticle;
        Notice.Text = report.Name;
    }

    private async Task LoadLookupsAsync()
    {
        var listing = AppComposition.Services?.GetService<IBackOfficeListingService>();
        if (listing is null)
        {
            return;
        }

        var template = new FuncDataTemplate<LookupOption>((item, _) => new TextBlock { Text = item?.Label ?? string.Empty });
        CustomerBox.ItemTemplate = template;
        ArticleBox.ItemTemplate = template;
        CustomerBox.ItemsSource = await listing.LookupAsync("Clientes");
        ArticleBox.ItemsSource = await listing.LookupAsync("Artigos");
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

        var start = StartDate.SelectedDate?.Date ?? DateTime.Today;
        var end = EndDate.SelectedDate?.Date ?? DateTime.Today;
        Guid? customer = CustomerBox.SelectedItem is LookupOption customerOption ? customerOption.Id : null;
        Guid? article = ArticleBox.SelectedItem is LookupOption articleOption ? articleOption.Id : null;
        Notice.Text = "A gerar...";
        var result = await service.GenerateAsync(_selected.Key, start, end, customer, article);
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

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Closed?.Invoke(this, EventArgs.Empty);
}
