namespace LogicPOS.Domain.Entities.Dtos;

public record CreateVatExemptionReasonDto(string Designation,
                                          string Acronym,
                                          string StandardApplicable,
                                          string? Notes,
                                          bool? IsDeleted);