namespace LogicPOS.Domain.Entities.Dtos;

public record CreateMeasurementUnitDto (string Designation,
                                        string Acronym,
                                        string? Notes,
                                        bool? IsDeleted);