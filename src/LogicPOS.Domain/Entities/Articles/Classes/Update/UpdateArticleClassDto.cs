namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateArticleClassDto(uint Order,
                                    string Code,
                                    string Designation,
                                    string Acronym,
                                    bool WorkInStock,
                                    string? Notes,
                                    bool IsDeleted);