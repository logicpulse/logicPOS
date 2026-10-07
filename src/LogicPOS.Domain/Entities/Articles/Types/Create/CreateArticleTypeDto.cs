namespace LogicPOS.Domain.Entities.Dtos;

public record CreateArticleTypeDto (string Designation,
                                    bool HasPrice,
                                    string? Notes,
                                    bool? IsDeleted);