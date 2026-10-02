using LogicPOS.Core.Fiscal;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public sealed class LocalFiscalYearWizard : IFiscalYearWizard
{
    private readonly IServiceScopeFactory _scopes;

    public LocalFiscalYearWizard(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<FiscalYearDraft> LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var system = scope.ServiceProvider.GetRequiredService<LogicPOS.Application.Features.System.ISystemInformationService>();
        var year = DateTime.Now.Year;
        var count = await database.FiscalYears.IgnoreQueryFilters().CountAsync(item => item.Year == year, cancellationToken);
        var active = await database.FiscalYears.AsNoTracking().FirstOrDefaultAsync(item => item.IsDeleted == false, cancellationToken);
        var terminals = await database.Terminals.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Designation)
            .Select(item => new FiscalYearChoice { Id = item.Id, Label = item.Designation })
            .ToListAsync(cancellationToken);
        var types = await database.DocumentTypes.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .OrderBy(item => item.Order)
            .Select(item => new FiscalYearChoice { Id = item.Id, Label = item.Designation, Acronym = item.Acronym })
            .ToListAsync(cancellationToken);

        return new FiscalYearDraft
        {
            Year = year,
            Designation = $"Ano {year} {count + 1}",
            Acronym = $"{year}A{count + 1}",
            Portugal = system.IsPortugal(),
            ActiveYearId = active?.Id,
            ActiveYearDesignation = active?.Designation,
            Terminals = terminals,
            DocumentTypes = FiscalYearSeriesPlan.DocumentTypes(types)
        };
    }

    public async Task<FiscalYearSetupResult> CreateAsync(FiscalYearSetupRequest request, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var created = new List<Guid>();
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            if (request.CloseCurrent)
            {
                var active = await database.FiscalYears.FirstOrDefaultAsync(item => item.IsDeleted == false, cancellationToken);
                if (active is not null)
                {
                    var series = await database.DocumentSeries.Where(item => item.FiscalYearId == active.Id && item.IsDeleted == false).ToListAsync(cancellationToken);
                    foreach (var item in series)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.Now;
                    }

                    active.IsDeleted = true;
                    active.DeletedAt = DateTime.Now;
                }
            }

            var yearNumbers = await database.FiscalYears.IgnoreQueryFilters().Select(item => new { item.Order, item.Code }).ToListAsync(cancellationToken);
            var yearNext = Next(yearNumbers.Select(item => (item.Order, item.Code)));
            var fiscalYear = new FiscalYear
            {
                Order = yearNext.Order,
                Code = yearNext.Code,
                Designation = request.Designation.Trim(),
                Acronym = request.Acronym.Trim(),
                Year = request.Year,
                SeriesForEachTerminal = request.SeriesForEachTerminal,
                Notes = request.Notes,
                IsDeleted = false
            };
            database.FiscalYears.Add(fiscalYear);

            if (request.CreateSeries)
            {
                var seriesNumbers = await database.DocumentSeries.IgnoreQueryFilters().Select(item => new { item.Order, item.Code }).ToListAsync(cancellationToken);
                var next = Next(seriesNumbers.Select(item => (item.Order, item.Code)));
                var types = await database.DocumentTypes.AsNoTracking()
                    .Where(item => item.IsDeleted == false)
                    .OrderBy(item => item.Order)
                    .Select(item => new FiscalYearChoice { Id = item.Id, Label = item.Designation, Acronym = item.Acronym })
                    .ToListAsync(cancellationToken);
                var selectedTypes = request.DocumentTypeIds.ToHashSet();
                types = FiscalYearSeriesPlan.DocumentTypes(types).Where(type => selectedTypes.Contains(type.Id)).ToList();
                var terminals = request.SeriesForEachTerminal ? request.TerminalIds.ToList() : [Guid.Empty];
                var terminalIndex = 1;
                foreach (var terminalId in terminals)
                {
                    foreach (var type in types)
                    {
                        var suffix = request.SeriesForEachTerminal ? terminalIndex.ToString() : string.Empty;
                        var acronym = $"{type.Acronym} {request.Acronym.Trim()}{suffix}";
                        var series = new DocumentSeries
                        {
                            Order = next.Order,
                            Code = next.Code,
                            Designation = $"{type.Label} {acronym}",
                            FiscalYearId = fiscalYear.Id,
                            DocumentTypeId = type.Id,
                            NextNumber = 1,
                            NumberRangeBegin = 1,
                            NumberRangeEnd = int.MaxValue,
                            Acronym = acronym,
                            TerminalId = request.SeriesForEachTerminal ? terminalId : null,
                            IsDeleted = false
                        };
                        database.DocumentSeries.Add(series);
                        created.Add(series.Id);
                        next = (next.Order + 1, (long.Parse(next.Code) + 1).ToString());
                    }

                    terminalIndex++;
                }
            }

            await database.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return FiscalYearSetupResult.Fail(exception.GetBaseException().Message);
        }

        var notes = new List<string> { "Ano fiscal criado." };
        if (request.CreateSeries)
        {
            notes.Add($"{created.Count} séries criadas.");
        }

        if (request.CreateSeries && request.CommunicateWithAt)
        {
            var module = scope.ServiceProvider.GetService<IFiscalModule>() ?? new NullFiscalModule();
            if (module.IsAvailable == false)
            {
                notes.Add("A certificação fiscal não está instalada neste posto. As séries ficaram por comunicar.");
            }
            else
            {
                foreach (var id in created)
                {
                    var error = await module.RegisterSeriesAsync(id, cancellationToken);
                    if (string.IsNullOrWhiteSpace(error) == false)
                    {
                        notes.Add(error);
                    }
                }
            }
        }

        return FiscalYearSetupResult.Done(string.Join(" ", notes));
    }

    public Task<FiscalYearSetupResult> TestAtAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(FiscalYearSetupResult.Fail("O teste em linha à AT não está disponível neste posto."));
    }

    private static (uint Order, string Code) Next(IEnumerable<(uint Order, string Code)> rows)
    {
        uint order = 0;
        long code = 0;
        foreach (var row in rows)
        {
            if (row.Order > order)
            {
                order = row.Order;
            }

            if (long.TryParse(row.Code, out var parsed) && parsed > code)
            {
                code = parsed;
            }
        }

        return (order + 1, (code + 1).ToString());
    }
}
