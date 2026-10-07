using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Finance.Documents.Documents.Create;
using LogicPOS.Domain.Results;
using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Domain.Entities.Utilities;

internal class DocumentPreviewCreator : EntityCreator<Document>
{
    private readonly CreateDocumentPreviewDto _dto;
    private readonly DocumentReferences _references;

    public DocumentPreviewCreator(CreateDocumentPreviewDto dto,
                                  DocumentReferences references) : base(new())
    {
        _dto = dto;
        _references = references;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        await AssignCurrencyAsync();
        _entity.Discount = _dto.Discount;
        _entity.Notes = _dto.Notes;
        _entity.ShipToAddress = _dto.ShipToAddress ?? new ShipAddress();
        _entity.ShipFromAddress = _dto.ShipFromAddress ?? new ShipAddress();
        _entity.MovementStartTime = DateTime.Now;
        _entity.MovementEndTime = DateTime.Now;
        _entity.TotalDelivery = _dto.TotalDelivery ?? 0;
        _entity.Customer = CreateCustomer();
        _entity.Number = "Preview";
        _entity.Hash = "Preview";
        _entity.Type = string.IsNullOrWhiteSpace(_dto.Type) ? "OR" : _dto.Type;
    }
    
    private async Task AssignCurrencyAsync()
    {
        if (_dto.CurrencyId == null)
        {
            _entity.CurrencyId = await _references.CurrencyRepository.GetCompanyCurrencyIdAsync();
            _entity.Currency = await _references.CurrencyRepository.GetByIdAsync(_entity.CurrencyId);
            _entity.ExchangeRate = 1;
        }
        else
        {
            _entity.CurrencyId = _dto.CurrencyId.Value;
            _entity.Currency = await _references.CurrencyRepository.GetByIdAsync(_entity.CurrencyId);
            _entity.ExchangeRate = _dto.ExchangeRate ?? _entity.Currency!.ExchangeRate;
        }
    }

    protected override Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }

    protected override Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success());
    }
    
    private DocumentCustomer CreateCustomer()
    {
        DocumentCustomer customer = new();
        customer.Name = "Cliente";
        customer.FiscalNumber = "Número Fiscal";
        customer.Address = "Endereço";
        customer.City = "Cidade";
        customer.Country = "País";
        customer.Phone = "Telefone";
        customer.Email = "Email";
        customer.Locality = "Localidade";
        customer.ZipCode = "Código Postal";
        return customer;
    }

    public override async Task<Result<Document>> CreateAsync(CancellationToken ct = default)
    {
        var createResult = await base.CreateAsync(ct);
        
        if (createResult.IsFailure)
        {
            return createResult;
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

        document.Details ??= new List<DocumentDetail>();
        document.CalculateTotals();
        
        return document;
    }
}