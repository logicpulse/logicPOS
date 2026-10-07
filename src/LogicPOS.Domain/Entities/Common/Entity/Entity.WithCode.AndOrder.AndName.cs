namespace LogicPOS.Domain.Entities.Common.Entity;

public abstract partial class Entity
{
    public abstract partial class WithCode : Entity, IEntityWithCode
    {
        public abstract partial class AndOrder : WithCode
        {
            public abstract class AndName : AndOrder, IEntityWithName
            {
                public string Name { get; set; } = null!;
            }
        }
    }
}
