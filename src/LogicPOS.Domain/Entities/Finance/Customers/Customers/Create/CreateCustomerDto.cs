using LogicPOS.Domain.Entities.Finance.Customers.Customers.Common;

namespace LogicPOS.Domain.Entities.Dtos;

public record CreateCustomerDto(Guid CustomerTypeId,
                                Guid? DiscountGroupId,
                                Guid PriceTypeId,
                                Guid CountryId,
                                string Name,
                                string? Address,
                                string? Locality,
                                string? ZipCode,
                                string? City,
                                DateTime? BirthDate,
                                string? Phone,
                                string? Fax,
                                string? MobilePhone,
                                string? Email,
                                string? WebSite,
                                string FiscalNumber,
                                string? CardNumber,
                                string? DiscountType,
                                decimal Discount,
                                decimal CardCredit,
                                CardMode CardMode,
                                bool Supplier,
                                string? Notes,
                                bool? IsDeleted,
                                bool Imported = false);