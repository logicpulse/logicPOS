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
        public DateTime LastUpdateAt { get; set; }
        public Guid LastUpdatedBy { get; set; }
        public Guid LastUpdatedWhere { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<DateTime> DeletedAt { get; set; }
    }
}
