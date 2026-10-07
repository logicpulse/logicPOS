using LogicPOS.Domain.Entities.Common;

namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateArticleSubfamilyDto(uint Order,
                                        string Code,
                                        string Designation,
                                        Guid FamilyId,
                                        Guid? CommissionGroupId,
                                        Guid? DiscountGroupId,
                                        Guid? VatOnTableId,
                                        Guid? VatDirectSellingId,
                                        Button? Button,
                                        string? Notes,
                                        bool IsDeleted);