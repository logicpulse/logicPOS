namespace LogicPOS.Domain.Entities.Common.Entity;

public abstract partial class Entity
{
    public abstract partial class WithCode : Entity, IEntityWithCode
    {
        public string Code { get; set; } = null!;
    }
}
