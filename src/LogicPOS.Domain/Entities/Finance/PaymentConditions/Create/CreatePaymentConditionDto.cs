namespace LogicPOS.Domain.Entities.Dtos;

public record CreatePaymentConditionDto(string Designation,
                                        string Acronym,
                                        string? Notes,
                                        bool? IsDeleted);