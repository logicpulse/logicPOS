using System.Globalization;
using LogicPOS.Application.Features.Common;
using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Domain.Enums;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Services.DemoData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Billing figures for the back-office dashboard (same aggregations as API reports/dashboard-billing).
/// </summary>
public sealed class DashboardBillingService : IDashboardBillingService
{
    private readonly IServiceScopeFactory _scopes;

    public DashboardBillingService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<DashboardFilterOptions> LoadFilterOptionsAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        var places = await database.Places.AsNoTracking()
            .Where(place => !place.IsDeleted)
            .OrderBy(place => place.Designation)
            .Select(place => new DashboardFilterOption(place.Id, place.Designation))
            .ToListAsync(cancellationToken);

        var terminals = await database.Terminals.AsNoTracking()
            .Where(terminal => !terminal.IsDeleted)
            .OrderBy(terminal => terminal.Designation)
            .Select(terminal => new DashboardFilterOption(terminal.Id, terminal.Designation))
            .ToListAsync(cancellationToken);

        var users = await database.Users.AsNoTracking()
            .Where(user => !user.IsDeleted)
            .OrderBy(user => user.Name)
            .Select(user => new DashboardFilterOption(user.Id, user.Name))
            .ToListAsync(cancellationToken);

        return new DashboardFilterOptions(places, terminals, users);
    }

    public async Task<DashboardBillingOverview> GetOverviewAsync(
        DashboardBillingQuery query,
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var year = query.Year is > 1900 and < 3000 ? query.Year : today.Year;
        var dailyDays = NormalizeDailyDays(query.DailyDays);
        var periodKey = NormalizePeriod(query.Period);
        var (periodStart, periodEnd, prevStart, prevEnd) = ResolvePeriod(periodKey, today);

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        // Local Avalonia / shared Persistence: fill demo sales before reading KPIs (safe: Demo licence + flag).
        try
        {
            var demo = scope.ServiceProvider.GetService<IDemoSalesDataService>();
            if (demo is not null)
                await demo.EnsureAsync(force: false, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<DashboardBillingService>>();
            logger?.LogDebug(ex, "Demo sales ensure skipped before dashboard overview.");
        }

        var terminalIdsForPlace = await ResolvePlaceTerminalIdsAsync(database, query.PlaceId, cancellationToken);
        var docs = ApplyDocumentFilters(
            database.Documents.AsNoTracking().ApplyActiveSalesInvoicesFilter(),
            query.TerminalId,
            query.UserId,
            terminalIdsForPlace);

        var details = ApplyDetailFilters(
            database.DocumentDetails.AsNoTracking().ApplyActiveSalesInvoicesFilter(),
            query.TerminalId,
            query.UserId,
            terminalIdsForPlace);

        var payments = ApplyPaymentFilters(
            database.DocumentPaymentMethods.AsNoTracking()
                .Where(payment => payment.Document != null
                                  && payment.Document.Status != "A"
                                  && payment.Document.IsDraft == false
                                  && payment.Document.Series!.DocumentType!.SaftDocumentType == SaftDocumentType.SalesInvoices),
            query.TerminalId,
            query.UserId,
            terminalIdsForPlace);

        var yesterday = today.AddDays(-1);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var prevMonthStart = monthStart.AddMonths(-1);
        var prevMonthEnd = monthStart.AddDays(-1);
        var yearStart = new DateTime(today.Year, 1, 1);
        var prevYearStart = new DateTime(today.Year - 1, 1, 1);
        var prevYearEnd = new DateTime(today.Year - 1, 12, 31);

        var todayTotals = await SumDocumentTotalsAsync(docs, today, today, cancellationToken);
        var yesterdayTotals = await SumDocumentTotalsAsync(docs, yesterday, yesterday, cancellationToken);
        var monthTotals = await SumDocumentTotalsAsync(docs, monthStart, today, cancellationToken);
        var prevMonthTotals = await SumDocumentTotalsAsync(docs, prevMonthStart, prevMonthEnd, cancellationToken);
        var yearTotals = await SumDocumentTotalsAsync(docs, yearStart, today, cancellationToken);
        var prevYearTotals = await SumDocumentTotalsAsync(docs, prevYearStart, prevYearEnd, cancellationToken);
        var periodTotals = await SumDocumentTotalsAsync(docs, periodStart, periodEnd, cancellationToken);
        var prevPeriodTotals = await SumDocumentTotalsAsync(docs, prevStart, prevEnd, cancellationToken);

        var documentCount = await CountDocumentsAsync(docs, periodStart, periodEnd, cancellationToken);
        var averageTicket = documentCount > 0 ? periodTotals.Final / documentCount : 0m;
        var averageTicketNet = documentCount > 0 ? periodTotals.Net / documentCount : 0m;

        var firstYear = await database.Documents.AsNoTracking()
            .OrderBy(document => document.CreatedAt)
            .Select(document => document.CreatedAt.Year)
            .FirstOrDefaultAsync(cancellationToken);
        if (firstYear == 0)
        {
            firstYear = today.Year;
        }

        var years = Enumerable.Range(firstYear, today.Year - firstYear + 1).OrderDescending().ToList();

        var daily = await BuildDailySeriesAsync(docs, today.AddDays(-(dailyDays - 1)), today, cancellationToken);
        var dailyPrevious = await BuildDailySeriesAsync(
            docs,
            today.AddDays(-(dailyDays * 2 - 1)),
            today.AddDays(-dailyDays),
            cancellationToken);

        var monthlyRows = await docs
            .Where(document => document.CreatedAt.Year == year)
            .GroupBy(document => document.CreatedAt.Month)
            .Select(group => new DashboardMonthPoint
            {
                Month = group.Key,
                Total = group.Sum(document => document.TotalFinal * (document.Type == "NC" ? -1 : 1)),
                NetTotal = group.Sum(document => document.TotalNet * (document.Type == "NC" ? -1 : 1))
            })
            .ToListAsync(cancellationToken);

        var monthly = Enumerable.Range(1, 12)
            .Select(month =>
            {
                var row = monthlyRows.FirstOrDefault(item => item.Month == month);
                return new DashboardMonthPoint
                {
                    Month = month,
                    Total = (row?.Total ?? 0m).Round(),
                    NetTotal = (row?.NetTotal ?? 0m).Round()
                };
            })
            .ToList();

        var topArticles = await details
            .ApplyDateFilter(periodStart, periodEnd)
            .GroupBy(detail => new
            {
                detail.ArticleId,
                Designation = detail.Article!.Designation,
                Code = detail.Article.Code
            })
            .Select(group => new DashboardTopArticlePoint
            {
                ArticleId = group.Key.ArticleId,
                Designation = group.Key.Designation,
                Code = group.Key.Code,
                Quantity = group.Sum(item => item.Quantity * (item.Document!.Type == "NC" ? -1 : 1)),
                Total = group.Sum(item => item.TotalFinal * (item.Document!.Type == "NC" ? -1 : 1)),
                NetTotal = group.Sum(item => item.TotalNet * (item.Document!.Type == "NC" ? -1 : 1))
            })
            .OrderByDescending(item => item.Total)
            .Take(10)
            .ToListAsync(cancellationToken);

        var paymentRows = await payments
            .ApplyDateFilter(periodStart, periodEnd)
            .GroupBy(payment => payment.PaymentMethod!.Designation)
            .Select(group => new
            {
                Name = group.Key,
                Total = group.Sum(payment => payment.Amount * (payment.Document!.Type == "NC" ? -1 : 1)),
                NetTotal = group.Sum(payment => payment.Amount * (payment.Document!.Type == "NC" ? -1 : 1))
            })
            .OrderByDescending(item => item.Total)
            .ToListAsync(cancellationToken);

        var paymentSum = paymentRows.Sum(item => item.Total);
        var paymentMethods = paymentRows
            .Select(item => new DashboardNamedAmountPoint
            {
                Name = string.IsNullOrWhiteSpace(item.Name) ? "—" : item.Name,
                Total = item.Total.Round(),
                NetTotal = item.NetTotal.Round(),
                Percent = paymentSum == 0
                    ? 0
                    : Math.Round(item.Total / paymentSum * 100m, 1, MidpointRounding.AwayFromZero)
            })
            .ToList();

        var hourlyDay = periodEnd;
        var hourlyRaw = await docs
            .ApplyDateFilter(hourlyDay, hourlyDay)
            .GroupBy(document => document.CreatedAt.Hour)
            .Select(group => new
            {
                Hour = group.Key,
                Total = group.Sum(document => document.TotalFinal * (document.Type == "NC" ? -1 : 1)),
                NetTotal = group.Sum(document => document.TotalNet * (document.Type == "NC" ? -1 : 1))
            })
            .ToListAsync(cancellationToken);

        var hourlyByHour = hourlyRaw.ToDictionary(item => item.Hour);
        List<DashboardHourPoint> hourly;
        if (hourlyRaw.Count == 0 || hourlyRaw.All(item => item.Total == 0 && item.NetTotal == 0))
        {
            hourly = Enumerable.Range(8, 13)
                .Select(hour => new DashboardHourPoint
                {
                    Hour = hour,
                    Label = $"{hour:00}h",
                    Total = 0m,
                    NetTotal = 0m
                })
                .ToList();
        }
        else
        {
            var withData = hourlyRaw.Where(item => item.Total != 0 || item.NetTotal != 0).ToList();
            var minHour = Math.Min(8, withData.Min(item => item.Hour));
            var maxHour = Math.Max(20, withData.Max(item => item.Hour));
            hourly = Enumerable.Range(minHour, maxHour - minHour + 1)
                .Select(hour =>
                {
                    hourlyByHour.TryGetValue(hour, out var row);
                    return new DashboardHourPoint
                    {
                        Hour = hour,
                        Label = $"{hour:00}h",
                        Total = (row?.Total ?? 0m).Round(),
                        NetTotal = (row?.NetTotal ?? 0m).Round()
                    };
                })
                .ToList();
        }

        var familyRows = await details
            .ApplyDateFilter(periodStart, periodEnd)
            .GroupBy(detail => detail.Article!.Subfamily!.Family!.Designation)
            .Select(group => new
            {
                Name = group.Key,
                Total = group.Sum(item => item.TotalFinal * (item.Document!.Type == "NC" ? -1 : 1)),
                NetTotal = group.Sum(item => item.TotalNet * (item.Document!.Type == "NC" ? -1 : 1))
            })
            .OrderByDescending(item => item.Total)
            .Take(8)
            .ToListAsync(cancellationToken);

        var familySum = familyRows.Sum(item => item.Total);
        var families = familyRows
            .Select(item => new DashboardNamedAmountPoint
            {
                Name = string.IsNullOrWhiteSpace(item.Name) ? "—" : item.Name,
                Total = item.Total.Round(),
                NetTotal = item.NetTotal.Round(),
                Percent = familySum == 0
                    ? 0
                    : Math.Round(item.Total / familySum * 100m, 1, MidpointRounding.AwayFromZero)
            })
            .ToList();

        return new DashboardBillingOverview
        {
            Period = periodKey,
            PeriodStart = DateOnly.FromDateTime(periodStart),
            PeriodEnd = DateOnly.FromDateTime(periodEnd),
            Year = year,
            DailyDays = dailyDays,
            Years = years,
            Kpis = new DashboardBillingKpis
            {
                TodayTotal = todayTotals.Final.Round(),
                TodayNetTotal = todayTotals.Net.Round(),
                YesterdayTotal = yesterdayTotals.Final.Round(),
                YesterdayNetTotal = yesterdayTotals.Net.Round(),
                MonthTotal = monthTotals.Final.Round(),
                MonthNetTotal = monthTotals.Net.Round(),
                PreviousMonthTotal = prevMonthTotals.Final.Round(),
                PreviousMonthNetTotal = prevMonthTotals.Net.Round(),
                YearTotal = yearTotals.Final.Round(),
                YearNetTotal = yearTotals.Net.Round(),
                PreviousYearTotal = prevYearTotals.Final.Round(),
                PreviousYearNetTotal = prevYearTotals.Net.Round(),
                DocumentCount = documentCount,
                AverageTicket = averageTicket.Round(),
                AverageTicketNet = averageTicketNet.Round(),
                PeriodTotal = periodTotals.Final.Round(),
                PeriodNetTotal = periodTotals.Net.Round(),
                PreviousPeriodTotal = prevPeriodTotals.Final.Round(),
                PreviousPeriodNetTotal = prevPeriodTotals.Net.Round()
            },
            Daily = daily,
            DailyPrevious = dailyPrevious,
            Monthly = monthly,
            TopArticles = topArticles,
            PaymentMethods = paymentMethods,
            Hourly = hourly,
            Families = families
        };
    }

    private static async Task<List<Guid>?> ResolvePlaceTerminalIdsAsync(
        LogicPOSDbContext database,
        Guid? placeId,
        CancellationToken cancellationToken)
    {
        if (placeId is null)
        {
            return null;
        }

        return await database.Terminals.AsNoTracking()
            .Where(terminal => terminal.PlaceId == placeId && !terminal.IsDeleted)
            .Select(terminal => terminal.Id)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<LogicPOS.Domain.Entities.Document> ApplyDocumentFilters(
        IQueryable<LogicPOS.Domain.Entities.Document> query,
        Guid? terminalId,
        Guid? userId,
        List<Guid>? placeTerminalIds)
    {
        query = query.ApplyTerminalFilter(terminalId);

        if (userId is not null)
        {
            query = query.Where(document => document.CreatedBy == userId);
        }

        if (placeTerminalIds is not null)
        {
            query = query.Where(document => placeTerminalIds.Contains(document.CreatedWhere));
        }

        return query;
    }

    private static IQueryable<LogicPOS.Domain.Entities.DocumentDetail> ApplyDetailFilters(
        IQueryable<LogicPOS.Domain.Entities.DocumentDetail> query,
        Guid? terminalId,
        Guid? userId,
        List<Guid>? placeTerminalIds)
    {
        query = query.ApplyTerminalFilter(terminalId);

        if (userId is not null)
        {
            query = query.Where(detail => detail.Document!.CreatedBy == userId);
        }

        if (placeTerminalIds is not null)
        {
            query = query.Where(detail => placeTerminalIds.Contains(detail.Document!.CreatedWhere));
        }

        return query;
    }

    private static IQueryable<LogicPOS.Domain.Entities.DocumentPaymentMethod> ApplyPaymentFilters(
        IQueryable<LogicPOS.Domain.Entities.DocumentPaymentMethod> query,
        Guid? terminalId,
        Guid? userId,
        List<Guid>? placeTerminalIds)
    {
        if (terminalId is not null)
        {
            query = query.Where(payment => payment.Document!.CreatedWhere == terminalId);
        }

        if (userId is not null)
        {
            query = query.Where(payment => payment.Document!.CreatedBy == userId);
        }

        if (placeTerminalIds is not null)
        {
            query = query.Where(payment => placeTerminalIds.Contains(payment.Document!.CreatedWhere));
        }

        return query;
    }

    private static async Task<(decimal Final, decimal Net)> SumDocumentTotalsAsync(
        IQueryable<LogicPOS.Domain.Entities.Document> documents,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken)
    {
        var totals = await documents
            .ApplyDateFilter(start, end)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Final = group.Sum(document => document.TotalFinal * (document.Type == "NC" ? -1 : 1)),
                Net = group.Sum(document => document.TotalNet * (document.Type == "NC" ? -1 : 1))
            })
            .FirstOrDefaultAsync(cancellationToken);

        return totals is null ? (0m, 0m) : (totals.Final, totals.Net);
    }

    private static Task<int> CountDocumentsAsync(
        IQueryable<LogicPOS.Domain.Entities.Document> documents,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken) =>
        documents.ApplyDateFilter(start, end).CountAsync(cancellationToken);

    private static async Task<List<DashboardAmountPoint>> BuildDailySeriesAsync(
        IQueryable<LogicPOS.Domain.Entities.Document> documents,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken)
    {
        var rows = await documents
            .ApplyDateFilter(start, end)
            .GroupBy(document => document.CreatedAt.Date)
            .Select(group => new
            {
                Day = group.Key,
                Total = group.Sum(document => document.TotalFinal * (document.Type == "NC" ? -1 : 1)),
                NetTotal = group.Sum(document => document.TotalNet * (document.Type == "NC" ? -1 : 1))
            })
            .ToListAsync(cancellationToken);

        var byDay = rows.ToDictionary(item => DateOnly.FromDateTime(item.Day));
        var points = new List<DashboardAmountPoint>();
        for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
        {
            var key = DateOnly.FromDateTime(day);
            byDay.TryGetValue(key, out var row);
            points.Add(new DashboardAmountPoint
            {
                Day = key,
                Label = day.ToString("dd/MM", CultureInfo.GetCultureInfo("pt-PT")),
                Total = (row?.Total ?? 0m).Round(),
                NetTotal = (row?.NetTotal ?? 0m).Round()
            });
        }

        return points;
    }

    private static string NormalizePeriod(string? period) =>
        period?.Trim().ToLowerInvariant() switch
        {
            "last7" or "7" or "7d" or "7dias" => "last7",
            "last30" or "30" or "30d" or "30dias" => "last30",
            "month" or "thismonth" or "mes" => "month",
            "year" or "thisyear" or "ano" => "year",
            _ => "today"
        };

    private static int NormalizeDailyDays(int days) =>
        days switch
        {
            15 => 15,
            30 => 30,
            _ => 7
        };

    private static (DateTime Start, DateTime End, DateTime PrevStart, DateTime PrevEnd) ResolvePeriod(
        string period,
        DateTime today)
    {
        switch (period)
        {
            case "last7":
                return (today.AddDays(-6), today, today.AddDays(-13), today.AddDays(-7));
            case "last30":
                return (today.AddDays(-29), today, today.AddDays(-59), today.AddDays(-30));
            case "month":
            {
                var start = new DateTime(today.Year, today.Month, 1);
                var prevStart = start.AddMonths(-1);
                var prevEnd = start.AddDays(-1);
                return (start, today, prevStart, prevEnd);
            }
            case "year":
            {
                var start = new DateTime(today.Year, 1, 1);
                var prevStart = new DateTime(today.Year - 1, 1, 1);
                var prevEnd = new DateTime(today.Year - 1, 12, 31);
                return (start, today, prevStart, prevEnd);
            }
            default:
                var yesterday = today.AddDays(-1);
                return (today, today, yesterday, yesterday);
        }
    }
}

public sealed record DashboardBillingQuery(
    string Period,
    int Year,
    int DailyDays,
    Guid? PlaceId = null,
    Guid? TerminalId = null,
    Guid? UserId = null);

public sealed class DashboardBillingOverview
{
    public string Period { get; init; } = "today";

    public DateOnly PeriodStart { get; init; }

    public DateOnly PeriodEnd { get; init; }

    public int Year { get; init; }

    public int DailyDays { get; init; }

    public IReadOnlyList<int> Years { get; init; } = [];

    public DashboardBillingKpis Kpis { get; init; } = new();

    public IReadOnlyList<DashboardAmountPoint> Daily { get; init; } = [];

    public IReadOnlyList<DashboardAmountPoint> DailyPrevious { get; init; } = [];

    public IReadOnlyList<DashboardMonthPoint> Monthly { get; init; } = [];

    public IReadOnlyList<DashboardTopArticlePoint> TopArticles { get; init; } = [];

    public IReadOnlyList<DashboardNamedAmountPoint> PaymentMethods { get; init; } = [];

    public IReadOnlyList<DashboardHourPoint> Hourly { get; init; } = [];

    public IReadOnlyList<DashboardNamedAmountPoint> Families { get; init; } = [];
}

public sealed class DashboardBillingKpis
{
    public decimal TodayTotal { get; init; }

    public decimal TodayNetTotal { get; init; }

    public decimal YesterdayTotal { get; init; }

    public decimal YesterdayNetTotal { get; init; }

    public decimal MonthTotal { get; init; }

    public decimal MonthNetTotal { get; init; }

    public decimal PreviousMonthTotal { get; init; }

    public decimal PreviousMonthNetTotal { get; init; }

    public decimal YearTotal { get; init; }

    public decimal YearNetTotal { get; init; }

    public decimal PreviousYearTotal { get; init; }

    public decimal PreviousYearNetTotal { get; init; }

    public int DocumentCount { get; init; }

    public decimal AverageTicket { get; init; }

    public decimal AverageTicketNet { get; init; }

    public decimal PeriodTotal { get; init; }

    public decimal PeriodNetTotal { get; init; }

    public decimal PreviousPeriodTotal { get; init; }

    public decimal PreviousPeriodNetTotal { get; init; }
}

public sealed class DashboardAmountPoint
{
    public DateOnly Day { get; init; }

    public string Label { get; init; } = string.Empty;

    public decimal Total { get; init; }

    public decimal NetTotal { get; init; }
}

public sealed class DashboardMonthPoint
{
    public int Month { get; init; }

    public decimal Total { get; init; }

    public decimal NetTotal { get; init; }
}

public sealed class DashboardTopArticlePoint
{
    public Guid? ArticleId { get; init; }

    public string Designation { get; init; } = string.Empty;

    public string? Code { get; init; }

    public decimal Quantity { get; init; }

    public decimal Total { get; init; }

    public decimal NetTotal { get; init; }
}

public sealed class DashboardNamedAmountPoint
{
    public string Name { get; init; } = string.Empty;

    public decimal Total { get; init; }

    public decimal NetTotal { get; init; }

    public decimal Percent { get; init; }
}

public sealed class DashboardHourPoint
{
    public int Hour { get; init; }

    public string Label { get; init; } = string.Empty;

    public decimal Total { get; init; }

    public decimal NetTotal { get; init; }
}

public sealed record DashboardFilterOption(Guid Id, string Name);

public sealed record DashboardFilterOptions(
    IReadOnlyList<DashboardFilterOption> Places,
    IReadOnlyList<DashboardFilterOption> Terminals,
    IReadOnlyList<DashboardFilterOption> Users);

/// <summary>Legacy snapshot shape kept for callers that still bind to chart points only.</summary>
public sealed class DashboardPoint
{
    public string Label { get; init; } = string.Empty;

    public double Final { get; init; }

    public double Net { get; init; }
}
