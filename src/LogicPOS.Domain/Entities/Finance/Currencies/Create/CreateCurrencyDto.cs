namespace LogicPOS.Domain.Entities.Dtos;

public record CreateCurrencyDto(
    string Designation, 
    string Acronym, 
    string Symbol, 
    string Entity, 
    decimal ExchangeRate,
    string? Notes,
    bool? IsDeleted
    );