namespace LogicPOS.Domain.Entities.Dtos;

public record CreateMovementTypeDto(string Designation,
                                    bool VatDirectSelling,
                                    string? Notes,
                                    bool? IsDeleted);