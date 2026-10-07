namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateArticleTypeDto(
    Guid? UserId,
    Guid? TerminalId,
    uint Order,
    string Code,
    string Designation,
    bool HasPrice,
    string? Notes,
    bool IsDeleted
);
