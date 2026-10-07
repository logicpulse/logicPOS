using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class DocumentSeriesCreator : EntityCreator<DocumentSeries>
{
    private readonly CreateDocumentSeriesDto _dto;
    private readonly IDocumentSeriesRepository _repository;
    private readonly DocumentSeriesReferences _references;

    public DocumentSeriesCreator(CreateDocumentSeriesDto dto,
                                 IDocumentSeriesRepository repository,
                                 DocumentSeriesReferences references) : base(new())
    {
        _dto = dto;
        _repository = repository;
        _references = references;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = _dto.Imported ? 0 : await _repository.GetNextOrderAsync(ct);
        _entity.Code = _dto.Imported ? "?" : await _repository.GetNextCodeAsync(ct);
        _entity.Acronym = _dto.Acronym;
        _entity.Designation = _dto.Designation;
        _entity.FiscalYearId = _dto.FiscalYearId;
        _entity.DocumentTypeId = _dto.DocumentTypeId;
        _entity.NextNumber = _dto.NextNumber;
        _entity.NumberRangeBegin = _dto.NumberRangeBegin;
        _entity.NumberRangeEnd = _dto.NumberRangeEnd;
        _entity.Notes = _dto.Notes;
        _entity.ATDocCodeValidationSeries =_dto.AtValidationCode;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (_dto.Imported)
        {
            return Result.Success();
        }
        
        return await CheckForDesignationConflictAsync(_repository,
                                                      _entity.Designation,
                                                      ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (!await _references.FiscalYearExistsAsync(_dto.FiscalYearId, ct))
        {
            return Result.NotFound(nameof(FiscalYear),_dto.FiscalYearId.ToString());
        }

        if (!await _references.DocumentTypeExistsAsync(_dto.DocumentTypeId, ct))
        {
            return Result.NotFound(nameof(DocumentType), _dto.DocumentTypeId.ToString());
        }

        return Result.Success();
    }
}