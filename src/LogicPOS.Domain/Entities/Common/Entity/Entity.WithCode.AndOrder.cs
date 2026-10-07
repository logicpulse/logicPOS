namespace LogicPOS.Domain.Entities.Common.Entity;

public abstract partial class Entity
{
    public abstract partial class WithCode : Entity, IEntityWithCode
    {
        public abstract partial class AndOrder : WithCode
        {
            public uint Order { get; set; } = 0!;
        }
    }
}
