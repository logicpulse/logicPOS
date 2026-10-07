using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Finance.Customers.Customers.Common;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("erp_customer")]
public class Customer : Entity.WithCode.AndOrder.AndName
{
    #region Relations

    public CustomerType? CustomerType { get; set; }
    public Guid CustomerTypeId { get; set; }

    public DiscountGroup? DiscountGroup { get; set; }
    public Guid? DiscountGroupId { get; set; }

    public PriceType? PriceType { get; set; }
    public Guid PriceTypeId { get; set; }

    public Country? Country { get; set; }
    public Guid CountryId { get; set; }

    #endregion

    #region Properties

    public string? Address { get; set; }
    public string? Locality { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? MobilePhone { get; set; }
    public string? Email { get; set; }
    public string? WebSite { get; set; }
    public string FiscalNumber { get; set; } = null!;
    public string? DiscountType { get; set; }

    public string? CardNumber { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal Discount { get; set; }
    [Column(TypeName = "decimal(18,6)")] public decimal CardCredit { get; set; }
    public CardMode CardMode { get; set; }

    public bool Supplier { get; set; }

    #endregion

    public bool HasSufficientCardBalance(decimal amount)
        => CardMode == CardMode.Credit || CardCredit >= amount;

    public Result RechargeCard(decimal amount)
    {
        if (IsDeleted)
        {
            return Result.InvalidOperation("Cliente eliminado.");
        }

        if (string.IsNullOrWhiteSpace(CardNumber))
        {
            return Result.InvalidOperation("Cliente sem número de cartão.");
        }

        CardCredit += amount;
        return Result.Success();
    }

    public async Task<Result> UpdateAsync(UpdateCustomerDto dto,
        ICustomerRepository repository,
        CustomerReferences dependencies,
        CancellationToken cancellationToken = default)
    {
        var updater = new CustomerUpdater(this, dto, repository, dependencies);
        return await updater.UpdateAsync(cancellationToken);
    }


    public static Task<Result<Customer>> CreateAsync(CreateCustomerDto dto,
        ICustomerRepository repository,
        CustomerReferences dependencies,
        CancellationToken cancellationToken = default)
    {
        var creator = new CustomerCreator(dto, repository, dependencies);
        return creator.CreateAsync(cancellationToken);
    }
}