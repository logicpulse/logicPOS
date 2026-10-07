using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Entities.Dtos;

public record CreateArticleFamilyDto(string Designation,
                                     Guid? CommissionGroupId,
                                     Guid? DiscountGroupId,
                                     Button? Button,
                                     string? Notes,
                                     bool? IsDeleted,
                                     bool Imported = false);