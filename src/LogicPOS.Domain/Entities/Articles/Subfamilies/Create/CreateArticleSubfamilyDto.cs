using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Entities.Dtos;

public record CreateArticleSubfamilyDto(string Designation,
                                        Guid FamilyId,
                                        Guid? CommissionGroupId,
                                        Guid? DiscountGroupId,
                                        Guid? VatOnTableId,
                                        Guid? VatDirectSellingId,
                                        Button? Button,
                                        string? Notes,
                                        bool? IsDeleted,
                                        bool Imported = false);