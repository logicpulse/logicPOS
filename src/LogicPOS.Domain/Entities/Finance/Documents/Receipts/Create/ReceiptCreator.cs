using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;
using LogicPOS.Domain.Services;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities.Utilities;

internal class ReceiptCreator : EntityCreator<Receipt>
{
    private readonly IReceiptRepository _repository;
    private readonly CreateReceiptDto _dto;
    private readonly ReceiptReferences _references;
    private readonly IDocumentHasher _documentHasher;
    private DocumentSeries? _series;
    private readonly FiscalCountry _country;
    private readonly Guid? _terminalId;
    private bool SystemCountryIsPortugal => _country.IsPortugal;

    public ReceiptCreator(CreateReceiptDto dto,
        IReceiptRepository repository,
        IDocumentHasher documentHasher,
        ReceiptReferences references,
        FiscalCountry country,
        Guid? terminalId) : base(new())
    {
        _dto = dto;
        _references = references;
        _repository = repository;
        _documentHasher = documentHasher;
        _country = country;
        _terminalId = terminalId;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.PaymentMethodId = _dto.PaymentMethodId;
        _entity.Mechanism = await _references.PaymentMethodRepository.GetAcronymAsync(_dto.PaymentMethodId, ct);
        _entity.RefNo = GenerateNumber();
        _entity.DocumentType = await _repository.GetDocumentType(ct);
        _entity.Amount = _dto.Amount;
        _entity.TaxPayable = _dto.TaxPayable;
        _entity.CurrencyCode = await _references.CurrencyRepository.GetAcronymAsync(_dto.CurrencyId, ct);
        _entity.CurrencyAmount = _dto.CurrencyAmount;
        _entity.ExchangeRate = _dto.ExchangeRate;
        _entity.SourceId = _entity.CreatedBy.ToString();
        _entity.Notes = _dto.Notes;
    }

    private string GenerateNumber()
    {
        string number = $"{_series!.Acronym}/{_series.NextNumber}";
        _series.NextNumber++;
        return number;
    }

    private string LastReceiptNumber => $"{_series!.Acronym}/{(_series.NextNumber - 1)}";

    protected override Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        _series =
            await _references.SeriesRepository.GetActiveSeriesForReceiptCreationAsync(_terminalId, ct);

        if (_series is null)
        {
            return Result.InvalidOperation("Nenhuma série activa para emissão de recibos foi encontrada.");
        }

        if (!await _references.PaymentMethodExistsAsync(_dto.PaymentMethodId, ct))
        {
            return Result.NotFound(nameof(PaymentMethod), _dto.PaymentMethodId.ToString());
        }

        if (!await _references.CurrencyExistsAsync(_dto.CurrencyId, ct))
        {
            return Result.NotFound(nameof(Currency), _dto.CurrencyId.ToString());
        }

        return Result.Success();
    }
}