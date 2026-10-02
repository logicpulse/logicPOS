using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class DashboardView : UserControl
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("pt-PT");
    private static readonly string[] QuarterColors = ["#8CC63F", "#6B9E2E", "#4D6610", "#A8D96A"];

    private DashboardSnapshot? _snapshot;
    private bool _includeVat = true;
    private bool _suppressYear;
    private int _year = DateTime.Today.Year;

    public DashboardView()
    {
        InitializeComponent();
    }

    public Task ReloadAsync() => LoadAsync(_year);

    private void OnVatOnClick(object? sender, RoutedEventArgs e)
    {
        if (_includeVat)
        {
            return;
        }

        _includeVat = true;
        VatOn.Classes.Add("active");
        VatOff.Classes.Remove("active");
        ApplySnapshot();
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
        ApplySnapshot();
    }

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
        var service = AppComposition.Services?.GetService<IDashboardBillingService>();
        if (service is null)
        {
            ShowStatus("Não foi possível ler a faturação.");
            return;
        }

        ShowStatus("A carregar faturação...");
        try
        {
            _snapshot = await service.LoadAsync(year);
            _year = _snapshot.Year;
            FillYears(_snapshot.Years, _snapshot.Year);
            StatusText.IsVisible = false;
            ApplySnapshot();
        }
        catch
        {
            ShowStatus("Não foi possível ler a faturação.");
        }
    }

    private void FillYears(IReadOnlyList<int> years, int selected)
    {
        _suppressYear = true;
        YearBox.ItemsSource = years;
        YearBox.SelectedItem = selected;
        _suppressYear = false;
    }

    private void ApplySnapshot()
    {
        if (_snapshot is null)
        {
            return;
        }

        var today = DateTime.Today;
        TodayValue.Text = Money(_includeVat ? _snapshot.TodayFinal : _snapshot.TodayNet);
        TodayHint.Text = Capitalize(today.ToString("dddd, dd MMMM", Culture));
        MonthValue.Text = Money(_includeVat ? _snapshot.MonthFinal : _snapshot.MonthNet);
        MonthHint.Text = $"Acumulado em {today.ToString("MMMM yyyy", Culture)}";
        YearValue.Text = Money(_includeVat ? _snapshot.CurrentYearFinal : _snapshot.CurrentYearNet);
        YearHint.Text = $"Acumulado em {today.Year}";
        BestLabel.Text = $"Melhor mês ({_snapshot.Year})";
        BestValue.Text = Money(_includeVat ? _snapshot.BestMonthFinal : _snapshot.BestMonthNet);
        BestHint.Text = _includeVat ? _snapshot.BestMonthLabel : _snapshot.BestMonthNetLabel;
        AnnualHint.Text = $"Evolução mensal em {_snapshot.Year}";
        AnnualTotal.Text = Money(_includeVat ? _snapshot.SelectedYearFinal : _snapshot.SelectedYearNet);
        QuarterHint.Text = $"Distribuição por trimestre em {_snapshot.Year}";

        DailyChart.SetPoints(Points(_snapshot.Days));
        AnnualChart.SetPoints(Points(_snapshot.Months));
        QuarterChart.SetPoints(Points(_snapshot.Quarters));
        FillQuarterLegend(_snapshot.Quarters);
    }

    private List<DashboardChartPoint> Points(IReadOnlyList<DashboardPoint> source) =>
        source.Select(point => new DashboardChartPoint
        {
            Label = point.Label,
            Value = _includeVat ? point.Final : point.Net
        }).ToList();

    private void FillQuarterLegend(IReadOnlyList<DashboardPoint> quarters)
    {
        QuarterLegend.Children.Clear();
        for (var index = 0; index < quarters.Count; index++)
        {
            QuarterLegend.Children.Add(new Ellipse
            {
                Classes = { "dash_legend_swatch" },
                Fill = new SolidColorBrush(Color.Parse(QuarterColors[index % QuarterColors.Length]))
            });
            QuarterLegend.Children.Add(new TextBlock
            {
                Classes = { "dash_legend_text" },
                Text = quarters[index].Label
            });
        }
    }

    private void ShowStatus(string message)
    {
        StatusText.Text = message;
        StatusText.IsVisible = true;
    }

    private static string Money(decimal value) => value.ToString("C", Culture);

    private static string Capitalize(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        return char.ToUpper(text[0], Culture) + text[1..];
    }
}
