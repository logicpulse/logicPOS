using System.Globalization;
using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Billing figures for the back-office dashboard, using the same sales-invoice filter as the web app.
/// </summary>
public sealed class DashboardBillingService : IDashboardBillingService
{
    private readonly IServiceScopeFactory _scopes;

    public DashboardBillingService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<DashboardSnapshot> LoadAsync(int year, CancellationToken cancellationToken = default)
    {
        var culture = CultureInfo.GetCultureInfo("pt-PT");
        var today = DateTime.Today;
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var sales = database.Documents.AsNoTracking().ApplyActiveSalesInvoicesFilter();

        var firstYear = await sales
            .OrderBy(document => document.CreatedAt)
            .Select(document => document.CreatedAt.Year)
            .FirstOrDefaultAsync(cancellationToken);
        if (firstYear == 0)
        {
            firstYear = today.Year;
        }

        var years = Enumerable.Range(firstYear, today.Year - firstYear + 1).OrderDescending().ToList();
        var monthlyRows = await sales
            .Where(document => document.CreatedAt.Year == year)
            .GroupBy(document => document.CreatedAt.Month)
            .Select(group => new
            {
                Month = group.Key,
                Net = group.Sum(document => document.TotalNet * (document.Type == "NC" ? -1 : 1)),
                Final = group.Sum(document => document.TotalFinal * (document.Type == "NC" ? -1 : 1))
            })
            .ToListAsync(cancellationToken);

        var byMonth = monthlyRows.ToDictionary(row => row.Month);
        var months = Enumerable.Range(1, 12)
            .Select(month =>
            {
                byMonth.TryGetValue(month, out var row);
                return new DashboardPoint
                {
                    Label = MonthLabel(culture, month),
                    Final = ToDouble(row?.Final ?? 0m),
                    Net = ToDouble(row?.Net ?? 0m)
                };
            })
            .ToList();

        var quarters = Enumerable.Range(0, 4)
            .Select(index =>
            {
                var slice = months.Skip(index * 3).Take(3).ToList();
                return new DashboardPoint
                {
                    Label = $"T{index + 1}",
                    Final = Math.Round(slice.Sum(point => point.Final), 2, MidpointRounding.AwayFromZero),
                    Net = Math.Round(slice.Sum(point => point.Net), 2, MidpointRounding.AwayFromZero)
                };
            })
            .ToList();

        var best = months
            .Select((point, index) => (Point: point, Month: index + 1))
            .OrderByDescending(item => item.Point.Final)
            .First();
        var bestNet = months.OrderByDescending(point => point.Net).First();

        var start = today.AddDays(-6);
        var end = today.AddDays(1);
        var dayRows = await sales
            .Where(document => document.CreatedAt >= start && document.CreatedAt < end)
            .Select(document => new { document.CreatedAt, document.TotalFinal, document.TotalNet, document.Type })
            .ToListAsync(cancellationToken);
        var days = Enumerable.Range(0, 7)
            .Select(offset =>
            {
                var day = start.AddDays(offset);
                var rows = dayRows.Where(row => row.CreatedAt.Date == day).ToList();
                var sign = (string? type) => type == "NC" ? -1m : 1m;
                return new DashboardPoint
                {
                    Label = day.ToString("dd/MM", culture),
                    Final = ToDouble(rows.Sum(row => row.TotalFinal * sign(row.Type))),
                    Net = ToDouble(rows.Sum(row => row.TotalNet * sign(row.Type)))
                };
            })
            .ToList();

        var currentYear = sales.Where(document => document.CreatedAt.Year == today.Year);
        var currentMonth = currentYear.Where(document => document.CreatedAt.Month == today.Month);
        var currentDay = currentMonth.Where(document => document.CreatedAt.Day == today.Day);

        return new DashboardSnapshot
        {
            Year = year,
            Years = years,
            TodayFinal = await SumAsync(currentDay, final: true, cancellationToken),
            TodayNet = await SumAsync(currentDay, final: false, cancellationToken),
            MonthFinal = await SumAsync(currentMonth, final: true, cancellationToken),
            MonthNet = await SumAsync(currentMonth, final: false, cancellationToken),
            CurrentYearFinal = await SumAsync(currentYear, final: true, cancellationToken),
            CurrentYearNet = await SumAsync(currentYear, final: false, cancellationToken),
            Days = days,
            Months = months,
            Quarters = quarters,
            SelectedYearFinal = (decimal)Math.Round(months.Sum(point => point.Final), 2, MidpointRounding.AwayFromZero),
            SelectedYearNet = (decimal)Math.Round(months.Sum(point => point.Net), 2, MidpointRounding.AwayFromZero),
            BestMonthFinal = best.Point.Final <= 0 ? 0m : (decimal)best.Point.Final,
            BestMonthNet = bestNet.Net <= 0 ? 0m : (decimal)bestNet.Net,
            BestMonthLabel = best.Point.Final <= 0 ? "—" : best.Point.Label,
            BestMonthNetLabel = bestNet.Net <= 0 ? "—" : bestNet.Label
        };
    }

    private static async Task<decimal> SumAsync(
        IQueryable<LogicPOS.Domain.Entities.Document> documents,
        bool final,
        CancellationToken cancellationToken)
    {
        var total = final
            ? await documents.SumAsync(document => document.TotalFinal * (document.Type == "NC" ? -1 : 1), cancellationToken)
            : await documents.SumAsync(document => document.TotalNet * (document.Type == "NC" ? -1 : 1), cancellationToken);
        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    private static double ToDouble(decimal value) => Math.Round((double)value, 2, MidpointRounding.AwayFromZero);

    private static string MonthLabel(CultureInfo culture, int month)
    {
        var label = culture.DateTimeFormat.AbbreviatedMonthNames[month - 1];
        return string.IsNullOrWhiteSpace(label) ? month.ToString(culture) : label.TrimEnd('.');
    }
}

public sealed class DashboardSnapshot
{
    public int Year { get; init; }

    public IReadOnlyList<int> Years { get; init; } = [];

    public decimal TodayFinal { get; init; }

    public decimal TodayNet { get; init; }

    public decimal MonthFinal { get; init; }

    public decimal MonthNet { get; init; }

    public decimal CurrentYearFinal { get; init; }

    public decimal CurrentYearNet { get; init; }

    public IReadOnlyList<DashboardPoint> Days { get; init; } = [];

    public IReadOnlyList<DashboardPoint> Months { get; init; } = [];

    public IReadOnlyList<DashboardPoint> Quarters { get; init; } = [];

    public decimal SelectedYearFinal { get; init; }

    public decimal SelectedYearNet { get; init; }

    public decimal BestMonthFinal { get; init; }

    public decimal BestMonthNet { get; init; }

    public string BestMonthLabel { get; init; } = "—";

    public string BestMonthNetLabel { get; init; } = "—";
}

public sealed class DashboardPoint
{
    public string Label { get; init; } = string.Empty;

    public double Final { get; init; }

    public double Net { get; init; }
}
