using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class DocumentSeriesUpdater : EntityUpdater<DocumentSeries>
{
    private readonly UpdateDocumentSeriesDto _dto;

    public DocumentSeriesUpdater(DocumentSeries entity,
                                 IDocumentSeriesRepository repository,
                                 UpdateDocumentSeriesDto dto) : base(entity,repository)
    {
        _dto = dto;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForDesignationConflictAsync(_dto.Designation, ct);
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override void SetNewData()
    {
        _entity.Designation = _dto.Designation;
        _entity.ATDocCodeValidationSeries = _dto.AtValidationCode;
    }
}