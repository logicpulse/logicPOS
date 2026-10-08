using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Documents;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;
using LogicPOS.Domain.Services;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities.Utilities;

internal class DocumentCreator : EntityCreator<Document>
{
    private readonly CreateDocumentDto _dto;
    private readonly IDocumentRepository _repository;
    private readonly IDocumentHasher _hasher;
    private readonly DocumentReferences _references;
    private readonly FiscalCountry _country;
    private DocumentSeries? _series = null;
    private readonly Guid? _terminalId;
    private bool SystemCountryIsPortugal => _country.IsPortugal;

    public DocumentCreator(CreateDocumentDto dto,
        IDocumentRepository repository,
        DocumentReferences references,
        IDocumentHasher hasher,
        FiscalCountry country,
        Guid? terminalId) : base(new())
    {
        _dto = dto;
        _repository = repository;
        _references = references;
        _hasher = hasher;
        _country = country;
        _terminalId = terminalId;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.IsDraft = _dto.IsDraft;
        AssignDependencies();
        await AssignCurrencyAsync();
        _entity.Number = GenerateNumber();
        _entity.Type = _dto.Type;
        _entity.Status = "N";
        _entity.StatusDate = $"{DateTime.Now:yyyy-MM-ddTHH:mm:ss}";
        _entity.Discount = _dto.Discount;
        _entity.Notes = _dto.Notes;
        _entity.ShipToAddress = _dto.ShipToAddress ?? new ShipAddress();
        _entity.ShipFromAddress = _dto.ShipFromAddress ?? new ShipAddress();
        _entity.MovementStartTime = DateTime.Now;
        _entity.MovementEndTime = DateTime.Now;
        _entity.TotalDelivery = _dto.TotalDelivery ?? 0;
    }

    private string GenerateNumber()
    {
        string number = $"{_series!.Acronym}/{_series.NextNumber}";
        if (_dto.IsDraft)
        {
            return $"[{number}]";
        }

        _series.NextNumber++;
        return number;
    }

    private async Task<Result> AssignCustomerAsync()
    {
        if (_dto.CustomerId == null)
        {
            if (await _references.CustomerRepository.FiscalNumberExistsAsync(_dto.Customer!.FiscalNumber) ||
                await _references.CustomerRepository.NameExistsAsync(_dto.Customer!.Name))
            {
                return Result.Conflict("Um cliente com o mesmo número fiscal ou nome já existe.");
            }

            _entity.CustomerId = await _references.CustomerRepository.CreateCustomerFromDocumentAsync(_dto.Customer!);
            _entity.Customer = _dto.Customer;

            return Result.Success();
        }

        if (!await _references.CustomerRepository.ExistsAsync(_dto.CustomerId.Value))
        {
            return Result.NotFound(nameof(Customer), _dto.CustomerId.ToString());
        }

        _entity.CustomerId = _dto.CustomerId.Value;

        if (_dto.Customer == null)
        {
            _entity.Customer = await _references.CustomerRepository.CreateDocumentCustomerAsync(_dto.CustomerId.Value);
            return Result.Success();
        }

        var (name, fiscalNumber) =
            await _references.CustomerRepository.GetCustomerNameAndFiscalNumberAsync(_dto.CustomerId.Value);

        if (name != _dto.Customer.Name || fiscalNumber != _dto.Customer.FiscalNumber)
        {
            return Result.Conflict("Nome ou número fiscal especificado não coincide com o cliente registado");
        }

        _entity.Customer = _dto.Customer;

        return Result.Success();
    }

    private async Task AssignHashAsync(CancellationToken ct = default)
    {
        if (_dto.IsDraft)
        {
            _entity.Hash = "?";
            return;
        }

        _entity.Hash = await _hasher.GenerateDocumentHashAsync(_entity.SeriesId,
            _entity.Number,
            _entity.Date,
            _entity.SystemEntryDate,
            _entity.TotalFinal, ct);
    }

    private void AssignDependencies()
    {
        _entity.PaymentMethods = _dto.PaymentMethods?.Select(x => new DocumentPaymentMethod
        {
            DocumentId = _entity.Id,
            PaymentMethodId = x.PaymentMethodId,
            Amount = x.Amount
        }).ToList();

        _entity.PaymentConditionId = _dto.PaymentConditionId;
        _entity.SeriesId = _series!.Id;
        _entity.ParentId = _dto.ParentId;
    }

    private async Task AssignCurrencyAsync()
    {
        if (_dto.CurrencyId == null)
        {
            _entity.CurrencyId = await _references.CurrencyRepository.GetCompanyCurrencyIdAsync();
            _entity.ExchangeRate = 1;
        }
        else
        {
            _entity.CurrencyId = _dto.CurrencyId.Value;
            _entity.ExchangeRate = _dto.ExchangeRate ?? 1;
        }
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (await _repository.NumberExistsAsync(_entity.Number, ct) && _dto.IsDraft == false)
        {
            return Result.Conflict("Número de documento já existe.");
        }

        var newCustomer = _dto.CustomerId == null && _dto.Customer != null;

        if (newCustomer &&
            await _references.CustomerRepository.FiscalNumberExistsAsync(_dto.Customer!.FiscalNumber, ct))
        {
            return Result.Conflict("Um cliente com o mesmo número fiscal já existe");
        }

        return Result.Success();
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        _series =
            await _references.SeriesRepository.GetActiveSeriesByDocumentTypeForDocumentCreationAsync(_dto.Type,_terminalId, ct);

        if (_series is null)
        {
            return Result.NotFound(nameof(DocumentSeries), _dto.Type);
        }

        if (_dto.PaymentMethods != null)
        {
            foreach (var paymentMethod in _dto.PaymentMethods)
            {
                if (!await _references.PaymentMethodExistsAsync(paymentMethod.PaymentMethodId, ct))
                {
                    return Result.NotFound(nameof(PaymentMethod), paymentMethod.PaymentMethodId.ToString());
                }
            }
        }

        if (_dto.PaymentConditionId.HasValue &&
            !await _references.PaymentConditionExistsAsync(_dto.PaymentConditionId.Value, ct))
        {
            return Result.NotFound(nameof(PaymentCondition), _dto.PaymentConditionId.ToString());
        }

        if (_dto.CurrencyId.HasValue && !await _references.CurrencyExistsAsync(_dto.CurrencyId.Value, ct))
        {
            return Result.NotFound(nameof(Currency), _dto.CurrencyId.ToString());
        }

        if (_dto.ParentId.HasValue && !await _references.DocumentExistsAsync(_dto.ParentId.Value, ct))
        {
            return Result.NotFound(nameof(Document), _dto.ParentId.ToString());
        }

        return Result.Success();
    }

    public override async Task<Result<Document>> CreateAsync(CancellationToken ct = default)
    {
        var createResult = await base.CreateAsync(ct);
        if (createResult.IsFailure)
        {
            return createResult;
        }

        var assignCustomerResult = await AssignCustomerAsync();
        if (assignCustomerResult.IsFailure)
        {
            return assignCustomerResult.ToGenericFailure<Document>();
        }

        var document = createResult.Value!;

        var addDetailsResult = await document.AddDetailsAsync(_dto.Details,
            new DocumentDetailReferences(_references.ArticleRepository,
                _references.VatRateRepository,
                _references.VatExemptionRepository),
            ct);
        if (addDetailsResult.IsFailure)
        {
            return addDetailsResult.ToGenericFailure<Document>();
        }

        document.CalculateTotals();
        if (document.PaymentMethods is not null && document.PaymentMethods.Count == 1)
        {
            document.PaymentMethods.First().Amount = document.TotalFinal;
        }

        await AssignHashAsync(ct);
        AssignAtcud();

        return document;
    }

    /// <summary>GTK ProcessFinanceDocument: ATCUD = ATDocCodeValidacaoSerie + "-" + sequential number.</summary>
    private void AssignAtcud()
    {
        if (_dto.IsDraft)
        {
            return;
        }

        var built = BuildAtcud(_series?.ATDocCodeValidationSeries, _entity.Number);
        if (string.IsNullOrWhiteSpace(built) == false)
        {
            _entity.ATCUD = built;
        }
    }

    private static string BuildAtcud(string? seriesValidationCode, string documentNumber)
    {
        var value = documentNumber.Trim().Trim('[', ']');
        var slash = value.LastIndexOf('/');
        var sequential = slash >= 0 && slash < value.Length - 1
            ? value[(slash + 1)..].Trim()
            : value;
        if (string.IsNullOrWhiteSpace(sequential))
        {
            return string.Empty;
        }

        return $"{seriesValidationCode?.Trim() ?? string.Empty}-{sequential}";
    }
}