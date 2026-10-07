using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateArticleFamilyDto(uint Order,
                                     string Code,
                                     string Designation,
                                     Guid? CommissionGroupId,
                                     Guid? DiscountGroupId,
                                     Guid? PrinterId,
                                     Guid? PrinterTemplateId,
                                     Button? Button,
                                     string? Notes,
                                     bool IsDeleted);