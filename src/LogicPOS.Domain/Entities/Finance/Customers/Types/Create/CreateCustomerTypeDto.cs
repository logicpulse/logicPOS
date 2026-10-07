namespace LogicPOS.Domain.Entities.Dtos;

public record CreateCustomerTypeDto(string Designation,
                                    string? Notes,
                                    bool? IsDeleted);