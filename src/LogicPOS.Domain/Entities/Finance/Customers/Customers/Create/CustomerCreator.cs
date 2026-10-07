using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class CustomerCreator : EntityCreator<Customer>
{
    private readonly CreateCustomerDto _dto;
    private readonly ICustomerRepository _repository;
    private readonly CustomerReferences _dependencies;

    public CustomerCreator(CreateCustomerDto dto,
                           ICustomerRepository repository,
                           CustomerReferences dependencies) : base(new())
    {
        _dto = dto;
        _repository = repository;
        _dependencies = dependencies;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = _dto.Imported ? 0 : await _repository.GetNextOrderAsync(ct);
        _entity.Code = _dto.Imported ? "?" : await _repository.GetNextCodeAsync(ct);
        _entity.CustomerTypeId = _dto.CustomerTypeId;
        _entity.DiscountGroupId = _dto.DiscountGroupId;
        _entity.PriceTypeId = _dto.PriceTypeId;
        _entity.CountryId = _dto.CountryId;
        _entity.Name = _dto.Name;
        _entity.Address = _dto.Address;
        _entity.Locality = _dto.Locality;
        _entity.ZipCode = _dto.ZipCode;
        _entity.City = _dto.City;
        _entity.BirthDate = _dto.BirthDate;
        _entity.Phone = _dto.Phone;
        _entity.Fax = _dto.Fax;
        _entity.MobilePhone = _dto.MobilePhone;
        _entity.Email = _dto.Email;
        _entity.WebSite = _dto.WebSite;
        _entity.FiscalNumber = _dto.FiscalNumber;
        _entity.CardNumber = _dto.CardNumber;
        _entity.CardCredit = 0;
        _entity.DiscountType = _dto.DiscountType;
        _entity.Discount = _dto.Discount;
        _entity.CardCredit = _dto.CardCredit;
        _entity.CardMode = _dto.CardMode;
        _entity.Supplier = _dto.Supplier;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (_dto.Imported)
        {
            return Result.Success();
        }
        
        if (await _repository.NameExistsAsync(_dto.Name, ct))
        {
            return Result.Conflict($"Nome '{_dto.Name}' já existe.");
        }

        if (await _repository.FiscalNumberExistsAsync(_dto.FiscalNumber, ct))
        {
            return Result.Conflict($"Número fiscal '{_dto.FiscalNumber}' já existe.");
        }

        return Result.Success();
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (!await _dependencies.CustomerTypeRepository.ExistsAsync(_dto.CustomerTypeId, ct))
        {
            return Result.NotFound(nameof(CustomerType),_dto.CustomerTypeId.ToString());
        }

        if (_dto.DiscountGroupId.HasValue &&
            !await _dependencies.DiscountGroupRepository.ExistsAsync(_dto.DiscountGroupId.Value, ct))
        {
            return Result.NotFound(nameof(DiscountGroup),_dto.DiscountGroupId.ToString());
        }

        if (!await _dependencies.PriceTypeRepository.ExistsAsync(_dto.PriceTypeId, ct))
        {
            return Result.NotFound(nameof(PriceType),_dto.PriceTypeId.ToString());
        }

        if (!await _dependencies.CountryRepository.ExistsAsync(_dto.CountryId, ct))
        {
            return Result.NotFound(nameof(Country),_dto.CountryId.ToString());
        }

        return Result.Success();
    }
}