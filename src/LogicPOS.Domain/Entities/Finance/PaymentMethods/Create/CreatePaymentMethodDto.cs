namespace LogicPOS.Domain.Entities.Dtos;

public record CreatePaymentMethodDto(string Designation,
                                     string Token,
                                     string? ResourceString,
                                     string? ButtonIcon,
                                     string Acronym,
                                     string? AllowPayback,
                                     string? Symbol,
                                     string? Notes,
                                     bool? IsDeleted);