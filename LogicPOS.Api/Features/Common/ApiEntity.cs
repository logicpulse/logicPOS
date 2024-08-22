using System;

namespace LogicPOS.Api.Features.Common
{
    public abstract class ApiEntity
    {
        public Guid Id { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid CreatedWhere { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public Guid UpdatedWhere { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
