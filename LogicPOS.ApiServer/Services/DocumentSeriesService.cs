using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class DocumentSeriesService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DocumentTypeService _documentTypeService;
    private readonly FiscalYearService _fiscalYearService;

    public DocumentSeriesService(ApplicationDbContext dbContext, DocumentTypeService documentTypeService, FiscalYearService fiscalYearService)
    {
        _dbContext = dbContext;
        _documentTypeService = documentTypeService;
        _fiscalYearService = fiscalYearService;
    }

    public async Task<IReadOnlyList<DocumentSeriesResponse>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var series = await _dbContext.ApiDocumentSeries.AsNoTracking().Where(item => !item.IsDeleted).ToListAsync(cancellationToken);
        if (series.Count == 0)
        {
            return [];
        }

        var typeIds = series.Select(item => item.DocumentTypeId).Distinct().ToList();
        var types = await _dbContext.ApiDocumentTypes.AsNoTracking().Where(item => typeIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        var fiscalYearIds = series.Select(item => item.FiscalYearId).Distinct().ToList();
        var fiscalYears = await _dbContext.ApiFiscalYears.AsNoTracking().Where(item => fiscalYearIds.Contains(item.Id)).ToDictionaryAsync(item => item.Id, cancellationToken);

        return series.Select(item => Map(item, types.GetValueOrDefault(item.DocumentTypeId), fiscalYears.GetValueOrDefault(item.FiscalYearId))).ToList();
    }

    public async Task<HasActiveDocumentSeriesResponse> HasActiveAsync(string? documentTypeAcronym, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(documentTypeAcronym))
        {
            return new HasActiveDocumentSeriesResponse { HasActiveSeries = false, SeriesForEachTerminal = false };
        }

        var documentType = await _documentTypeService.GetByAcronymAsync(documentTypeAcronym, cancellationToken);
        if (documentType is null)
        {
            return new HasActiveDocumentSeriesResponse { HasActiveSeries = false, SeriesForEachTerminal = false };
        }

        var activeSeries = await _dbContext.ApiDocumentSeries.AsNoTracking()
            .Where(item => !item.IsDeleted && item.DocumentTypeId == documentType.Id)
            .ToListAsync(cancellationToken);

        if (activeSeries.Count == 0)
        {
            return new HasActiveDocumentSeriesResponse { HasActiveSeries = false, SeriesForEachTerminal = false };
        }

        var fiscalYearIds = activeSeries.Select(item => item.FiscalYearId).Distinct().ToList();
        var seriesForEachTerminal = await _dbContext.ApiFiscalYears.AsNoTracking()
            .Where(item => fiscalYearIds.Contains(item.Id))
            .AnyAsync(item => item.SeriesForEachTerminal, cancellationToken);

        return new HasActiveDocumentSeriesResponse { HasActiveSeries = true, SeriesForEachTerminal = seriesForEachTerminal };
    }

    public async Task<ApiDocumentSeries?> GetActiveForTypeAsync(Guid documentTypeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiDocumentSeries.AsNoTracking()
            .Where(item => !item.IsDeleted && item.DocumentTypeId == documentTypeId)
            .OrderByDescending(item => item.CreatedUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(bool Success, string? Field, string? Error, AddEntityIdResponse? Response)> CreateAsync(CreateDocumentSeriesRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _documentTypeService.ExistsAsync(request.DocumentTypeId, cancellationToken))
        {
            return (false, "documentTypeId", "DocumentTypeId não existe.", null);
        }

        if (!await _fiscalYearService.ExistsAsync(request.FiscalYearId, cancellationToken))
        {
            return (false, "fiscalYearId", "FiscalYearId não existe ou está fechado.", null);
        }

        var series = new ApiDocumentSeries
        {
            Id = Guid.NewGuid(),
            Code = $"{request.Acronym ?? "S"}{DateTime.UtcNow:yyyy}",
            Designation = request.Designation.Trim(),
            NextNumber = request.NextNumber,
            NumberRangeBegin = request.NumberRangeBegin,
            NumberRangeEnd = request.NumberRangeEnd,
            Acronym = request.Acronym,
            DocumentTypeId = request.DocumentTypeId,
            FiscalYearId = request.FiscalYearId,
            AtValidationCode = request.AtValidationCode,
            TerminalId = request.Terminals?.Count == 1 ? request.Terminals[0] : null,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiDocumentSeries.Add(series);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, null, new AddEntityIdResponse { Id = series.Id });
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateDocumentSeriesRequest request, CancellationToken cancellationToken = default)
    {
        var series = await _dbContext.ApiDocumentSeries.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (series is null)
        {
            return false;
        }

        series.Designation = request.Designation.Trim();
        series.AtValidationCode = request.AtValidationCode;
        series.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>Atomically claims the next number in the series, formatted "&lt;Acronym&gt; &lt;Number&gt;/&lt;Year&gt;".</summary>
    public async Task<(bool Success, string? Error, string? Number)> IssueNumberAsync(Guid seriesId, CancellationToken cancellationToken = default)
    {
        var series = await _dbContext.ApiDocumentSeries.SingleOrDefaultAsync(item => item.Id == seriesId, cancellationToken);
        if (series is null)
        {
            return (false, "DocumentSeries não existe.", null);
        }

        if (series.NextNumber > series.NumberRangeEnd)
        {
            return (false, "A série esgotou o intervalo de numeração.", null);
        }

        var fiscalYear = await _dbContext.ApiFiscalYears.AsNoTracking().SingleOrDefaultAsync(item => item.Id == series.FiscalYearId, cancellationToken);
        var number = series.NextNumber;
        series.NextNumber++;
        series.UpdatedUtc = DateTime.UtcNow;

        var formatted = $"{series.Acronym ?? series.Code} {number}/{fiscalYear?.Year ?? DateTime.UtcNow.Year}";
        return (true, null, formatted);
    }

    internal static DocumentSeriesResponse Map(ApiDocumentSeries item, ApiDocumentType? type, ApiFiscalYear? fiscalYear)
    {
        return new DocumentSeriesResponse
        {
            Id = item.Id,
            Notes = item.Notes,
            CreatedAt = item.CreatedUtc,
            UpdatedAt = item.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = item.IsDeleted,
            Code = item.Code,
            Designation = item.Designation,
            NextNumber = item.NextNumber,
            NumberRangeBegin = item.NumberRangeBegin,
            NumberRangeEnd = item.NumberRangeEnd,
            Acronym = item.Acronym,
            DocumentType = type is null ? null : DocumentTypeService.Map(type),
            FiscalYear = fiscalYear is null ? null : FiscalYearService.Map(fiscalYear),
            ATDocCodeValidationSeries = item.AtValidationCode,
            TerminalId = item.TerminalId,
            Terminal = null
        };
    }
}
