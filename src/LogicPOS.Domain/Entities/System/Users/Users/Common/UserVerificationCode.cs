using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

/// <summary>
/// Single-use portal authentication code. A new send marks older unused rows as used.
/// </summary>
public class UserVerificationCode : Entity
{
    public Guid UserId { get; set; }
    public string CodeHash { get; set; } = null!;
    public DateTime? UsedAt { get; set; }
}
