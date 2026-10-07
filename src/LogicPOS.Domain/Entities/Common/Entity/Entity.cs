namespace LogicPOS.Domain.Entities.Common.Entity;

public abstract partial class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Guid CreatedBy { get; set; }
    public Guid CreatedWhere { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public Guid UpdatedBy { get; set; }
    public Guid UpdatedWhere { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DeletedAt { get; set; }

    public abstract class WithOrder : WithCode
    {
        public uint Order { get; set; } = 0;

        public abstract class AndDesignation : WithOrder, IEntityWithDesignation
        {
            public string Designation { get; set; } = null!;
        }
    }

}