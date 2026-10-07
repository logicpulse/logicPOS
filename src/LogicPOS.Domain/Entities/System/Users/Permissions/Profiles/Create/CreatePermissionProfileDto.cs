namespace LogicPOS.Domain.Entities.Dtos;

public record CreatePermissionProfileDto (
    Guid PermissionItemId,
    Guid UserProfileId
);