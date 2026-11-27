using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class ArticleDashboardView : UserControl
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("pt-PT");
    private static readonly string[] StockColors = ChartBrushes.Hex;

    private bool _suppressYear;
    private int _year = DateTime.Today.Year;
    private int _generation;

    public event EventHandler? OpenStockRequested;

    public ArticleDashboardView()
    {
        InitializeComponent();
        EnglishUi.Apply(this);
        ListingNotice.BindToast(StockStatus, this);
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
            return;
        }

        try
        {
            StockStatus.IsVisible = false;
            Busy.Show("A carregar stocks...");
            // Queries fora do UI thread — mesmo padrão anti-ANR do dashboard de faturação.
            var snapshot = await Task.Run(async () => await stock.LoadDashboardAsync(year)).ConfigureAwait(false);
            if (generation != _generation)
            {
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (generation != _generation)
                {
                    return;
                }

                ApplyStock(snapshot);
                Busy.Hide();
            });
        }
        catch
        {
            if (generation != _generation)
            {
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Busy.Hide();
                StockStatus.Text = "Não foi possível ler os stocks.";
            });
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
        StockValue.Text = MoneyFormat.Format(snapshot.StockValue);
        EntriesValue.Text = snapshot.StockEntriesInYear.ToString("0.##", Culture);
        EntriesHint.Text = $"Quantidade entrada em {snapshot.Year}";
        RotationValue.Text = snapshot.RotationLabel;
        StockHint.Text = snapshot.Limited
            ? "Resumo limitado aos registos mais recentes."
            : "Artigos únicos, armazéns e vendas do ano selecionado.";
        UniqueHint.Text = $"Em stock, compostos e vendidos em {snapshot.Year}";
        TopSoldHint.Text = $"Saídas em {snapshot.Year}";
        MonthlyHint.Text = $"Entradas e saídas em {snapshot.Year}";
        WarehouseTotalValue.Text = snapshot.WarehouseTotalQuantity.ToString("0.##", Culture);
        FamilyTotalValue.Text = MoneyFormat.Format(snapshot.FamilyTotalValue);

        var hasWarehouses = snapshot.Warehouses.Count > 0;
        WarehouseChart.IsVisible = hasWarehouses;
        WarehouseEmpty.IsVisible = !hasWarehouses;
        if (hasWarehouses)
        {
            WarehouseChart.SetPoints(ChartPoints(snapshot.Warehouses), columnBrush: ChartBrushes.Verde);
        }

        var hasTopSold = snapshot.TopSold.Count > 0;
        TopSoldChart.IsVisible = hasTopSold;
        TopSoldEmpty.IsVisible = !hasTopSold;
        if (hasTopSold)
        {
            TopSoldChart.SetPoints(ChartPoints(snapshot.TopSold), columnBrush: ChartBrushes.Laranja);
        }

        var hasFamilies = snapshot.FamilyValues.Count > 0;
        FamilyChart.IsVisible = hasFamilies;
        FamilyEmpty.IsVisible = !hasFamilies;
        if (hasFamilies)
        {
            FamilyChart.SetPoints(ChartPoints(snapshot.FamilyValues), columnBrush: ChartBrushes.Azul);
        }

        var monthlyPoints = snapshot.MonthlyMovements
            .Select(point => new DashboardGroupedChartPoint
            {
                Label = point.Label,
                Primary = point.InQuantity,
                Secondary = point.OutQuantity
            })
            .ToList();
        var hasMonthly = monthlyPoints.Any(point => point.Primary > 0 || point.Secondary > 0);
        MonthlyChart.IsVisible = hasMonthly;
        MonthlyEmpty.IsVisible = !hasMonthly;
        if (hasMonthly)
        {
            MonthlyChart.SetPoints(monthlyPoints, ChartBrushes.Verde, ChartBrushes.Laranja);
        }

        var plainInStock = Math.Max(0, snapshot.UniqueInStock - snapshot.UniqueComposedInStock);
        var slices = new[]
        {
            new DashboardChartPoint { Label = "Em stock", Value = plainInStock, Caption = plainInStock.ToString("N0", Culture) },
            new DashboardChartPoint { Label = "Compostos", Value = snapshot.UniqueComposedInStock, Caption = snapshot.UniqueComposedInStock.ToString("N0", Culture) },
            new DashboardChartPoint { Label = "Vendidos", Value = snapshot.UniqueSoldInYear, Caption = snapshot.UniqueSoldInYear.ToString("N0", Culture) }
        };
        var hasUnique = slices.Any(point => point.Value > 0);
        UniqueChart.IsVisible = hasUnique;
        UniqueLegend.IsVisible = hasUnique;
        UniqueEmpty.IsVisible = !hasUnique;
        if (hasUnique)
        {
            UniqueChart.SetPoints(slices);
            FillLegend(slices);
        }
        else
        {
            UniqueLegend.Children.Clear();
        }

        FillBelowMinimum(snapshot.BelowMinimumRows);
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

    private void FillBelowMinimum(IReadOnlyList<StockDashboardBelowMinimumRow> rows)
    {
        BelowMinimumRows.Children.Clear();
        var empty = rows.Count == 0;
        BelowMinimumEmpty.IsVisible = empty;
        if (empty)
        {
            return;
        }

        foreach (var row in rows)
        {
            var grid = new Grid
            {
                Classes = { "dash_stock_table_row" },
                ColumnDefinitions = new ColumnDefinitions("2*,*,*,*")
            };
            grid.Children.Add(Cell(row.Article, 0));
            grid.Children.Add(Cell(row.CurrentStock.ToString("0.##", Culture), 1, warn: true));
            grid.Children.Add(Cell(row.MinimumStock.ToString("0.##", Culture), 2));
            grid.Children.Add(Cell(row.Warehouse, 3));
            BelowMinimumRows.Children.Add(grid);
        }
    }

    private static TextBlock Cell(string text, int column, bool warn = false)
    {
        var block = new TextBlock
        {
            Classes = { warn ? "dash_stock_table_warn" : "dash_stock_table_td" },
            Text = text,
            TextTrimming = TextTrimming.CharacterEllipsis
        };
        Grid.SetColumn(block, column);
        return block;
    }
}
