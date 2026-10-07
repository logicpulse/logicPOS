namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateVatExemptionReasonDto(uint Order,
                                          string Code,
                                          string Designation,
                                          string Acronym,
                                          string StandardApplicable,
                                          string? Notes,
                                          bool IsDeleted);