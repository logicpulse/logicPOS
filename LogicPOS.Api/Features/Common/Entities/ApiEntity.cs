using System;

namespace LogicPOS.Api.Features.Common
{
    public abstract class ApiEntity
    {
        public Guid Id { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
