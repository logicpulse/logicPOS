namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateCurrencyDto(uint Order,
                                string Code,
                                string Designation,
                                string Acronym,
                                string Symbol,
                                string Entity,
                                decimal ExchangeRate,
                                string? Notes,
                                bool IsDeleted);