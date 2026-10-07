namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateMeasurementUnitDto (uint Order,
                                        string Code,
                                        string Designation,
                                        string Acronym,
                                        string? Notes,
                                        bool IsDeleted);