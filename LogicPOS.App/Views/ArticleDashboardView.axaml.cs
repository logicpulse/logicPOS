using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class ArticleDashboardView : UserControl
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("pt-PT");
    private static readonly string[] StockColors = ["#8CC63F", "#6B9E2E", "#4D6610"];

    private bool _suppressYear;
    private int _year = DateTime.Today.Year;
    private int _generation;

    public event EventHandler? OpenStockRequested;

    public ArticleDashboardView()
    {
        InitializeComponent();
        FillYears(_year);
    }

    public async Task<bool> EnsureModuleAsync()
    {
        var stock = AppComposition.Services?.GetService<IStockManagementService>();
        if (stock is null)
        {
            return false;
        }

        var access = await stock.GetModuleAccessAsync();
        return access.HasModule;
    }

    public Task ReloadAsync() => LoadAsync(_year);

    private void OnOpenStockClick(object? sender, RoutedEventArgs e) => OpenStockRequested?.Invoke(this, EventArgs.Empty);

    private async void OnYearChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressYear || YearBox.SelectedItem is not int year || year == _year)
        {
            return;
        }

        _year = year;
        await LoadAsync(year);
    }

    private async Task LoadAsync(int year)
    {
        var generation = ++_generation;
        var stock = AppComposition.Services?.GetService<IStockManagementService>();
        if (stock is null)
        {
            StockStatus.Text = "Não foi possível ler os stocks.";
            StockStatus.IsVisible = true;
            return;
        }

        try
        {
            StockStatus.Text = "A carregar stocks...";
            StockStatus.IsVisible = true;
            var snapshot = await stock.LoadDashboardAsync(year);
            if (generation != _generation)
            {
                return;
            }

            ApplyStock(snapshot);
            StockStatus.IsVisible = false;
        }
        catch
        {
            if (generation != _generation)
            {
                return;
            }

            StockStatus.Text = "Não foi possível ler os stocks.";
            StockStatus.IsVisible = true;
        }
    }

    private void ApplyStock(StockDashboardSnapshot snapshot)
    {
        _year = snapshot.Year;
        FillYears(snapshot.Year);
        BelowMinimumValue.Text = snapshot.BelowMinimum.ToString("N0", Culture);
        UniqueStockValue.Text = snapshot.UniqueInStock.ToString("N0", Culture);
        UniqueSoldValue.Text = snapshot.UniqueSoldInYear.ToString("N0", Culture);
        UniqueSoldHint.Text = $"Séries vendidas em {snapshot.Year}";
        StockValue.Text = snapshot.StockValue.ToString("C", Culture);
        StockHint.Text = snapshot.Limited
            ? "Resumo limitado aos registos mais recentes."
            : "Artigos únicos, armazéns e vendas do ano selecionado.";
        UniqueHint.Text = $"Em stock, compostos e vendidos em {snapshot.Year}";
        TopSoldHint.Text = $"Saídas em {snapshot.Year}";
        WarehouseChart.SetPoints(ChartPoints(snapshot.Warehouses));
        TopSoldChart.SetPoints(ChartPoints(snapshot.TopSold));
        var plainInStock = Math.Max(0, snapshot.UniqueInStock - snapshot.UniqueComposedInStock);
        var slices = new[]
        {
            new DashboardChartPoint { Label = "Em stock", Value = plainInStock, Caption = plainInStock.ToString("N0", Culture) },
            new DashboardChartPoint { Label = "Compostos", Value = snapshot.UniqueComposedInStock, Caption = snapshot.UniqueComposedInStock.ToString("N0", Culture) },
            new DashboardChartPoint { Label = "Vendidos", Value = snapshot.UniqueSoldInYear, Caption = snapshot.UniqueSoldInYear.ToString("N0", Culture) }
        };
        UniqueChart.SetPoints(slices);
        FillLegend(slices);
    }

    private void FillYears(int selected)
    {
        var start = DateTime.Today.Year - 4;
        var years = Enumerable.Range(start, 5).Reverse().ToList();
        if (years.Contains(selected) == false)
        {
            years.Insert(0, selected);
        }

        _suppressYear = true;
        YearBox.ItemsSource = years;
        YearBox.SelectedItem = selected;
        _suppressYear = false;
    }

    private static List<DashboardChartPoint> ChartPoints(IReadOnlyList<StockDashboardPoint> source)
        => source.Select(point => new DashboardChartPoint { Label = point.Label, Value = point.Value }).ToList();

    private void FillLegend(IReadOnlyList<DashboardChartPoint> points)
    {
        UniqueLegend.Children.Clear();
        for (var index = 0; index < points.Count; index++)
        {
            UniqueLegend.Children.Add(new Ellipse
            {
                Classes = { "dash_legend_swatch" },
                Fill = new SolidColorBrush(Color.Parse(StockColors[index % StockColors.Length]))
            });
            UniqueLegend.Children.Add(new TextBlock
            {
                Classes = { "dash_legend_text" },
                Text = points[index].Label
            });
        }
    }
}
