using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class CustomerUpdater : EntityUpdater<Customer>
{
    private readonly UpdateCustomerDto _dto;
    private readonly CustomerReferences _dependencies;
    private readonly ICustomerRepository _customerRepository;

    public CustomerUpdater(Customer entity,
        UpdateCustomerDto dto,
        ICustomerRepository repository,
        CustomerReferences dependencies) :
        base(entity, repository)
    {
        _dto = dto;
        _dependencies = dependencies;
        _customerRepository = repository;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (_dto.FiscalNumber != _entity.FiscalNumber &&
            await (_repository as ICustomerRepository)!.FiscalNumberExistsAsync(_dto.FiscalNumber, ct))
        {
            return Result.Conflict($"Número Fiscal '{_dto.FiscalNumber}' já existe.");
        }

        var codeResult = await CheckForCodeConflictAsync(_dto.Code, ct);
        if (codeResult.IsFailure)
        {
            return codeResult;
        }

        var nameResult = await CheckForNameConflictAsync(_dto.Name, ct);
        if (nameResult.IsFailure)
        {
            return nameResult;
        }

        if (!string.IsNullOrWhiteSpace(_dto.CardNumber) && _dto.CardNumber != _entity.CardNumber &&
            await _customerRepository.CardNumberExistsAsync(_dto.CardNumber, ct))
        {
            return Result.Conflict("Número de cartão já existe.");
        }

        return Result.Success();
    }



    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (_dto.TypeId != _entity.CustomerTypeId &&
            !await _dependencies.CustomerTypeRepository.ExistsAsync(_dto.TypeId, ct))
        {
            return Result.NotFound<CustomerType>(_dto.TypeId.ToString()!);
        }

        if (_dto.DiscountGroupId.HasValue &&
            _dto.DiscountGroupId != _entity.DiscountGroupId &&
            !await _dependencies.DiscountGroupRepository.ExistsAsync(_dto.DiscountGroupId.Value, ct)
           )
        {
            return Result.NotFound<DiscountGroup>(_dto.DiscountGroupId.ToString()!);
        }

        if (_dto.PriceTypeId != _entity.PriceTypeId &&
            !await _dependencies.PriceTypeRepository.ExistsAsync(_dto.PriceTypeId, ct))
        {
            return Result.NotFound<PriceType>(_dto.PriceTypeId.ToString()!);
        }

        if (_dto.CountryId != _entity.CountryId &&
            !await _dependencies.CountryRepository.ExistsAsync(_dto.CountryId, ct))
        {
            return Result.NotFound<Country>(_dto.CountryId.ToString()!);
        }

        return Result.Success();
    }

    protected override void SetNewData()
    {
        _entity.Order = _dto.Order;
        _entity.Code = _dto.Code;
        _entity.Name = _dto.Name;
        _entity.CustomerTypeId = _dto.TypeId;
        _entity.DiscountGroupId = _dto.DiscountGroupId;
        _entity.PriceTypeId = _dto.PriceTypeId;
        _entity.CountryId = _dto.CountryId;
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
        _entity.DiscountType = _dto.DiscountType;
        _entity.Discount = _dto.Discount;
        _entity.CardMode = _dto.CardMode;
        _entity.Supplier = _dto.Supplier;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted;
    }
}