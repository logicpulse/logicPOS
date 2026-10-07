namespace LogicPOS.Domain.Entities.Dtos;

public record CreateArticleClassDto(string Designation,
                                    string Acronym,
                                    bool WorkInStock,
                                    string? Notes,
                                    bool? IsDeleted);