using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class DashboardView : UserControl
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("pt-PT");
    private static readonly string[] PaymentColors = ChartBrushes.Hex;

    private DashboardBillingOverview? _overview;
    private bool _includeVat = true;
    private bool _suppressFilters;
    private string _period = "today";
    private int _year = DateTime.Today.Year;
    private int _dailyDays = 7;
    private Guid? _placeId;
    private Guid? _terminalId;
    private Guid? _userId;
    private int _loadGeneration;
    private CancellationTokenSource? _loadCts;
    private bool _filtersLoaded;

    public DashboardView()
    {
        InitializeComponent();
        ListingNotice.BindToast(StatusText, this);
        var nameBinding = new Avalonia.Data.Binding(nameof(FilterItem.Name));
        PlaceBox.DisplayMemberBinding = nameBinding;
        TerminalBox.DisplayMemberBinding = nameBinding;
        UserBox.DisplayMemberBinding = nameBinding;
    }

    public Task ReloadAsync() => LoadAsync();

    private void OnVatOnClick(object? sender, RoutedEventArgs e)
    {
        if (_includeVat)
        {
            return;
        }

        _includeVat = true;
        VatOn.Classes.Add("active");
        VatOff.Classes.Remove("active");
        ApplyOverview();
    }

    private void OnVatOffClick(object? sender, RoutedEventArgs e)
    {
        if (_includeVat == false)
        {
            return;
        }

        _includeVat = false;
        VatOff.Classes.Add("active");
        VatOn.Classes.Remove("active");
        ApplyOverview();
    }

    private async void OnPeriodClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string period } || string.Equals(_period, period, StringComparison.Ordinal))
        {
            return;
        }

        _period = period;
        SetActiveChip(period);
        await LoadAsync();
    }

    private async void OnDailyDaysClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string tag } || !int.TryParse(tag, out var days) || days == _dailyDays)
        {
            return;
        }

        _dailyDays = days;
        SetActiveDailyChip(days);
        await LoadAsync();
    }

    private async void OnYearChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressFilters || YearBox.SelectedItem is not int year || year == _year)
        {
            return;
        }

        _year = year;
        await LoadAsync();
    }

    private async void OnFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressFilters)
        {
            return;
        }

        _placeId = SelectedFilterId(PlaceBox);
        _terminalId = SelectedFilterId(TerminalBox);
        _userId = SelectedFilterId(UserBox);
        await LoadAsync();
    }

    private void OnDocumentsStubClick(object? sender, PointerPressedEventArgs e)
    {
        this.FindAncestorOfType<BackOfficeWindow>()?.OpenPage("Documentos");
    }

    private void OnArticlesStubClick(object? sender, RoutedEventArgs e)
    {
        this.FindAncestorOfType<BackOfficeWindow>()?.OpenReport(DashboardReportLinks.TopArticles);
    }

    private async Task LoadAsync()
    {
        var generation = ++_loadGeneration;
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();
        var cancellationToken = _loadCts.Token;

        var service = AppComposition.Services?.GetService<IDashboardBillingService>();
        if (service is null)
        {
            await ShowStatusOnUiAsync("Não foi possível ler a faturação.");
            return;
        }

        await ShowStatusOnUiAsync("A carregar faturação...", loading: true);
        try
        {
            if (_filtersLoaded == false)
            {
                var options = await Task.Run(
                        async () => await service.LoadFilterOptionsAsync(cancellationToken),
                        cancellationToken)
                    .ConfigureAwait(false);

                if (generation != _loadGeneration || cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (generation != _loadGeneration)
                    {
                        return;
                    }

                    FillFilterBoxes(options);
                    _filtersLoaded = true;
                });
            }

            var query = new DashboardBillingQuery(_period, _year, _dailyDays, _placeId, _terminalId, _userId);
            var overview = await Task.Run(
                    async () => await service.GetOverviewAsync(query, cancellationToken),
                    cancellationToken)
                .ConfigureAwait(false);

            if (generation != _loadGeneration || cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (generation != _loadGeneration)
                {
                    return;
                }

                _overview = overview;
                _year = overview.Year;
                _dailyDays = overview.DailyDays;
                FillYears(overview.Years, overview.Year);
                SetActiveDailyChip(_dailyDays);
                StatusText.IsVisible = false;
                Busy.Hide();
                ApplyOverview();
            });
        }
        catch (OperationCanceledException)
        {
            // Pedido mais recente substituiu este carregamento.
        }
        catch (Exception exception)
        {
            if (generation != _loadGeneration)
            {
                return;
            }

            App.WriteCrash("DashboardView.LoadAsync", exception);
            await ShowStatusOnUiAsync("Não foi possível ler a faturação.");
        }
    }

    private async Task ShowStatusOnUiAsync(string message, bool loading = false)
    {
        await Dispatcher.UIThread.InvokeAsync(() => ShowStatus(message, loading));
    }

    private void FillFilterBoxes(DashboardFilterOptions options)
    {
        _suppressFilters = true;
        PlaceBox.ItemsSource = WithAll(options.Places);
        TerminalBox.ItemsSource = WithAll(options.Terminals);
        UserBox.ItemsSource = WithAll(options.Users);
        PlaceBox.SelectedIndex = 0;
        TerminalBox.SelectedIndex = 0;
        UserBox.SelectedIndex = 0;
        _suppressFilters = false;
    }

    private static List<FilterItem> WithAll(IReadOnlyList<DashboardFilterOption> source)
    {
        var items = new List<FilterItem> { new(null, "Todos") };
        items.AddRange(source.Select(item => new FilterItem(item.Id, item.Name)));
        return items;
    }

    private void FillYears(IReadOnlyList<int> years, int selected)
    {
        _suppressFilters = true;
        var list = years.Count > 0
            ? years.ToList()
            : [DateTime.Today.Year, DateTime.Today.Year - 1, DateTime.Today.Year - 2];
        YearBox.ItemsSource = list;
        YearBox.SelectedItem = list.Contains(selected) ? selected : list[0];
        _suppressFilters = false;
    }

    private void ApplyOverview()
    {
        if (_overview is null)
        {
            return;
        }

        var kpis = _overview.Kpis;
        TodayValue.Text = Money(Pick(kpis.TodayTotal, kpis.TodayNetTotal));
        ApplyDeltaTrend(TodayDelta, TodayVs, TodayArrow, Pick(kpis.TodayTotal, kpis.TodayNetTotal), Pick(kpis.YesterdayTotal, kpis.YesterdayNetTotal), "vs ontem");
        MonthValue.Text = Money(Pick(kpis.MonthTotal, kpis.MonthNetTotal));
        ApplyDeltaTrend(MonthDelta, MonthVs, MonthArrow, Pick(kpis.MonthTotal, kpis.MonthNetTotal), Pick(kpis.PreviousMonthTotal, kpis.PreviousMonthNetTotal), "vs mês ant.");
        YearValue.Text = Money(Pick(kpis.YearTotal, kpis.YearNetTotal));
        ApplyDeltaTrend(YearDelta, YearVs, YearArrow, Pick(kpis.YearTotal, kpis.YearNetTotal), Pick(kpis.PreviousYearTotal, kpis.PreviousYearNetTotal), "vs ano ant.");
        DocumentsValue.Text = kpis.DocumentCount.ToString("N0", Culture);
        TicketValue.Text = Money(Pick(kpis.AverageTicket, kpis.AverageTicketNet));
        EvolutionValue.Text = FormatEvolution(Pick(kpis.PeriodTotal, kpis.PeriodNetTotal), Pick(kpis.PreviousPeriodTotal, kpis.PreviousPeriodNetTotal));
        ApplyEvolutionTrend(EvolutionVs, EvolutionArrow, Pick(kpis.PeriodTotal, kpis.PeriodNetTotal), Pick(kpis.PreviousPeriodTotal, kpis.PreviousPeriodNetTotal));

        DailyHint.Text = _dailyDays switch
        {
            15 => "Últimos 15 dias",
            30 => "Últimos 30 dias",
            _ => "Últimos 7 dias"
        };
        AnnualHint.Text = $"Evolução mensal em {_overview.Year}";
        QuarterHint.Text = $"Distribuição por trimestre em {_overview.Year}";
        HourlyHint.Text = _period == "today" ? "Ao longo de hoje" : "Último dia do período";

        var dailyPoints = BuildDailyPoints(_overview.Daily);
        var hasDaily = dailyPoints.Any(point => point.Value != 0);
        DailyEmpty.IsVisible = !hasDaily;
        DailyChart.IsVisible = hasDaily;
        // Match web Syncfusion fills: daily=Verde, annual=Laranja, hourly=Roxo.
        DailyChart.SetPoints(dailyPoints, showTrend: true, ChartBrushes.Verde);

        var monthlyPoints = BuildMonthlyPoints(_overview.Monthly);
        var yearTotal = _overview.Monthly.Sum(month => Pick(month.Total, month.NetTotal));
        AnnualTotal.Text = Money(yearTotal);
        var hasMonthly = monthlyPoints.Any(point => point.Value != 0);
        AnnualEmpty.IsVisible = !hasMonthly;
        AnnualChart.IsVisible = hasMonthly;
        AnnualChart.SetPoints(monthlyPoints, columnBrush: ChartBrushes.Laranja);

        var quarters = BuildQuarterPoints(_overview.Monthly);
        var hasQuarter = quarters.Any(point => point.Value != 0);
        QuarterEmpty.IsVisible = !hasQuarter;
        QuarterBars.IsVisible = hasQuarter;
        FillHorizontalBars(QuarterBars, quarters, stacked: false);

        var articles = _overview.TopArticles;
        var hasArticles = articles.Count > 0;
        ArticlesEmpty.IsVisible = !hasArticles;
        TopArticlesList.IsVisible = hasArticles;
        FillTopArticles(articles);

        var payments = _overview.PaymentMethods
            .Select(payment => new DashboardChartPoint
            {
                Label = payment.Name,
                Value = (double)Pick(payment.Total, payment.NetTotal),
                Caption = Money(Pick(payment.Total, payment.NetTotal))
            })
            .ToList();
        var hasPayments = payments.Any(point => point.Value != 0);
        PaymentEmpty.IsVisible = !hasPayments;
        PaymentChart.IsVisible = hasPayments;
        PaymentLegend.IsVisible = hasPayments;
        PaymentTotal.Text = Money((decimal)payments.Sum(point => point.Value));
        PaymentChart.SetPoints(payments);
        FillPaymentLegend(payments);

        var hourlyPoints = _overview.Hourly
            .Select(hour => new DashboardChartPoint
            {
                Label = hour.Label,
                Value = (double)Pick(hour.Total, hour.NetTotal)
            })
            .ToList();
        var hasHourly = hourlyPoints.Any(point => point.Value != 0);
        HourlyEmpty.IsVisible = !hasHourly;
        HourlyChart.IsVisible = hasHourly;
        HourlyChart.SetPoints(hourlyPoints, columnBrush: ChartBrushes.Roxo);

        var families = _overview.Families
            .Select(family => new BarItem(family.Name, (double)Pick(family.Total, family.NetTotal)))
            .ToList();
        var hasFamilies = families.Any(item => item.Value != 0);
        FamilyEmpty.IsVisible = !hasFamilies;
        FamilyBars.IsVisible = hasFamilies;
        FillHorizontalBars(FamilyBars, families, stacked: true);
    }

    private List<DashboardChartPoint> BuildDailyPoints(IReadOnlyList<DashboardAmountPoint> daily)
    {
        var points = daily
            .Select(point => new DashboardChartPoint
            {
                Label = point.Label,
                Value = (double)Pick(point.Total, point.NetTotal),
                Trend = (double)Pick(point.Total, point.NetTotal)
            })
            .ToList();

        ApplyDailyTrend(points);
        return points;
    }

    private static void ApplyDailyTrend(IList<DashboardChartPoint> points)
    {
        for (var index = 0; index < points.Count; index++)
        {
            var from = Math.Max(0, index - 1);
            var to = Math.Min(points.Count - 1, index + 1);
            var sum = 0d;
            var count = 0;
            for (var cursor = from; cursor <= to; cursor++)
            {
                sum += points[cursor].Value;
                count++;
            }

            points[index] = new DashboardChartPoint
            {
                Label = points[index].Label,
                Value = points[index].Value,
                Caption = points[index].Caption,
                Trend = Math.Round(sum / count, 2, MidpointRounding.AwayFromZero)
            };
        }
    }

    private List<DashboardChartPoint> BuildMonthlyPoints(IReadOnlyList<DashboardMonthPoint> months)
    {
        var names = Culture.DateTimeFormat.AbbreviatedMonthNames;
        return Enumerable.Range(1, 12)
            .Select(month =>
            {
                var row = months.FirstOrDefault(item => item.Month == month);
                var label = names[month - 1];
                if (string.IsNullOrWhiteSpace(label) == false)
                {
                    label = label.TrimEnd('.');
                }

                return new DashboardChartPoint
                {
                    Label = label,
                    Value = (double)Pick(row?.Total ?? 0m, row?.NetTotal ?? 0m)
                };
            })
            .ToList();
    }

    private List<BarItem> BuildQuarterPoints(IReadOnlyList<DashboardMonthPoint> months)
    {
        return Enumerable.Range(0, 4)
            .Select(index =>
            {
                var slice = months.Where(month => month.Month >= (index * 3) + 1 && month.Month <= (index + 1) * 3);
                var value = slice.Sum(month => (double)Pick(month.Total, month.NetTotal));
                return new BarItem($"T{index + 1}", Math.Round(value, 2, MidpointRounding.AwayFromZero));
            })
            .ToList();
    }

    private void FillHorizontalBars(ItemsControl host, IReadOnlyList<BarItem> items, bool stacked)
    {
        var panel = new StackPanel { Spacing = stacked ? 10 : 8 };
        var max = Math.Max(0.01d, items.Count == 0 ? 1d : items.Max(item => item.Value));
        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            panel.Children.Add(stacked
                ? BuildStackedBar(item, index, max)
                : BuildInlineBar(item, index, max));
        }

        host.ItemsSource = new[] { panel };
    }

    private Control BuildInlineBar(BarItem item, int index, double max)
    {
        var row = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("36,*,Auto"),
            Margin = new Thickness(0, 0, 0, 2)
        };
        row.Children.Add(BarLabel(item.Label));
        var bar = BarTrack(item, index, max);
        Grid.SetColumn(bar, 1);
        row.Children.Add(bar);
        var value = BarValue(item.Value);
        Grid.SetColumn(value, 2);
        row.Children.Add(value);
        return row;
    }

    private Control BuildStackedBar(BarItem item, int index, double max)
    {
        var row = new StackPanel { Spacing = 4 };
        var header = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto") };
        header.Children.Add(BarLabel(item.Label));
        var value = BarValue(item.Value);
        Grid.SetColumn(value, 1);
        header.Children.Add(value);
        row.Children.Add(header);
        row.Children.Add(BarTrack(item, index, max));
        return row;
    }

    private static TextBlock BarLabel(string label)
    {
        var text = new TextBlock
        {
            Text = label,
            Classes = { "dash_bar_label" },
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            TextTrimming = TextTrimming.CharacterEllipsis
        };
        ToolTip.SetTip(text, label);
        return text;
    }

    private TextBlock BarValue(double amount)
        => new()
        {
            Text = Money((decimal)amount),
            Classes = { "dash_bar_value" },
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(12, 0, 0, 0)
        };

    private static ProgressBar BarTrack(BarItem item, int index, double max)
        => new()
        {
            Classes = { "dash_bar_progress" },
            Minimum = 0,
            Maximum = max,
            Value = Math.Max(0, item.Value),
            Height = 10,
            MinHeight = 10,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = ChartBrushes.SeriesAt(index)
        };

    private void FillTopArticles(IReadOnlyList<DashboardTopArticlePoint> articles)
    {
        var panel = new StackPanel { Spacing = 6 };
        for (var index = 0; index < articles.Count; index++)
        {
            var article = articles[index];
            var amount = Pick(article.Total, article.NetTotal);
            var row = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("28,*,Auto"),
                MinHeight = 34
            };
            row.Children.Add(new TextBlock
            {
                Text = (index + 1).ToString(Culture),
                Classes = { "dash_article_rank" },
                VerticalAlignment = VerticalAlignment.Center
            });
            var meta = new StackPanel { Spacing = 1 };
            meta.Children.Add(new TextBlock
            {
                Text = string.IsNullOrWhiteSpace(article.Designation) ? "—" : article.Designation,
                Classes = { "dash_article_name" }
            });
            meta.Children.Add(new TextBlock
            {
                Text = Money(amount),
                Classes = { "dash_article_amount" }
            });
            Grid.SetColumn(meta, 1);
            row.Children.Add(meta);
            var qty = new TextBlock
            {
                Text = $"{article.Quantity.ToString("0.##", Culture)} un.",
                Classes = { "dash_article_qty" },
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(qty, 2);
            row.Children.Add(qty);
            panel.Children.Add(row);
        }

        TopArticlesList.ItemsSource = new[] { panel };
    }

    private void FillPaymentLegend(IReadOnlyList<DashboardChartPoint> payments)
    {
        PaymentLegend.Children.Clear();
        for (var index = 0; index < payments.Count; index++)
        {
            PaymentLegend.Children.Add(new Ellipse
            {
                Classes = { "dash_legend_swatch" },
                Fill = new SolidColorBrush(Color.Parse(PaymentColors[index % PaymentColors.Length]))
            });
            PaymentLegend.Children.Add(new TextBlock
            {
                Classes = { "dash_legend_text" },
                Text = payments[index].Label
            });
        }
    }

    private void SetActiveChip(string period)
    {
        foreach (var button in new[] { PeriodToday, Period7, Period30, PeriodMonth, PeriodYear })
        {
            if (button.Tag is string tag && string.Equals(tag, period, StringComparison.Ordinal))
            {
                button.Classes.Add("active");
            }
            else
            {
                button.Classes.Remove("active");
            }
        }
    }

    private void SetActiveDailyChip(int days)
    {
        foreach (var button in new[] { Daily7, Daily15, Daily30 })
        {
            if (button.Tag is string tag && int.TryParse(tag, out var value) && value == days)
            {
                button.Classes.Add("active");
            }
            else
            {
                button.Classes.Remove("active");
            }
        }
    }

    private void ShowStatus(string message, bool loading)
    {
        if (loading)
        {
            StatusText.Text = string.Empty;
            Busy.Show(message);
            return;
        }

        Busy.Hide();
        StatusText.Text = message;
        StatusText.IsVisible = !string.IsNullOrWhiteSpace(message);
    }

    private decimal Pick(decimal withVat, decimal withoutVat) => _includeVat ? withVat : withoutVat;

    private static Guid? SelectedFilterId(ComboBox box) =>
        box.SelectedItem is FilterItem { Id: Guid id } ? id : null;

    private static string Money(decimal value) => MoneyFormat.Format(value);

    private static void ApplyDeltaTrend(TextBlock delta, TextBlock vs, TextBlock arrow, decimal current, decimal previous, string vsLabel)
    {
        ClearTrendTone(delta);
        ClearTrendTone(arrow);
        var pct = PercentChange(current, previous);
        if (pct is null)
        {
            delta.Text = "—";
            vs.Text = string.Empty;
            arrow.Text = string.Empty;
            return;
        }

        var up = pct >= 0;
        var sign = up ? "+" : string.Empty;
        delta.Text = $"{sign}{pct.Value.ToString("0", Culture)}%";
        vs.Text = $" {vsLabel}";
        arrow.Text = up ? "▲" : "▼";
        delta.Classes.Add(up ? "up" : "down");
        arrow.Classes.Add(up ? "up" : "down");
    }

    private static void ApplyEvolutionTrend(TextBlock vs, TextBlock arrow, decimal current, decimal previous)
    {
        ClearTrendTone(arrow);
        vs.Text = "Vs período anterior";
        var pct = PercentChange(current, previous);
        if (pct is null)
        {
            arrow.Text = string.Empty;
            return;
        }

        var up = pct >= 0;
        arrow.Text = up ? "▲" : "▼";
        arrow.Classes.Add(up ? "up" : "down");
    }

    private static void ClearTrendTone(TextBlock target)
    {
        target.Classes.Remove("up");
        target.Classes.Remove("down");
    }

    private static string FormatEvolution(decimal current, decimal previous)
    {
        var pct = PercentChange(current, previous);
        if (pct is null)
        {
            return "—";
        }

        var sign = pct >= 0 ? "+" : string.Empty;
        return $"{sign}{pct.Value.ToString("0", Culture)}%";
    }

    private static decimal? PercentChange(decimal current, decimal previous)
    {
        if (previous == 0m)
        {
            return current == 0m ? null : 100m;
        }

        return Math.Round((current - previous) / Math.Abs(previous) * 100m, 0, MidpointRounding.AwayFromZero);
    }

    private sealed record FilterItem(Guid? Id, string Name);

    private sealed record BarItem(string Label, double Value);
}
