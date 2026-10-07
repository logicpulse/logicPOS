namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateCustomerTypeDto(uint Order,
                                    string Code,
                                    string Designation,
                                    string? Notes,
                                    bool IsDeleted);