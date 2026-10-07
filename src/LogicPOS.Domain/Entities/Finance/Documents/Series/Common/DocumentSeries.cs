using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("fin_documentfinanceseries")]
public class DocumentSeries : Entity.WithCode.AndOrder.AndDesignation
{
    public int NextNumber { get; set; }
    public int NumberRangeBegin { get; set; }
    public int NumberRangeEnd { get; set; }
    public string Acronym { get; set; } = null!;

    public DocumentType? DocumentType { get; set; }
    public Guid DocumentTypeId { get; set; }

    public FiscalYear? FiscalYear { get; set; }
    public Guid FiscalYearId { get; set; }
    public string? ATDocCodeValidationSeries { get; set; }
    public Guid? TerminalId { get; set; }
    public Terminal? Terminal { get; set; }

    public async Task<Result> UpdateAsync(UpdateDocumentSeriesDto dto,
        IDocumentSeriesRepository repository,
        CancellationToken cancellationToken = default)
    {
        var seriesUpdater = new DocumentSeriesUpdater(this, repository, dto);
        return await seriesUpdater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<DocumentSeries>> CreateAsync(CreateDocumentSeriesDto dto,
        IDocumentSeriesRepository repository,
        DocumentSeriesReferences references,
        CancellationToken cancellationToken = default)
    {
        var seriesCreator = new DocumentSeriesCreator(dto, repository, references);
        return await seriesCreator.CreateAsync(cancellationToken);
    }
}