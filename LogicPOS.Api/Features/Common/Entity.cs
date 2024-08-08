using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Common
{
    public abstract class Entity
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
        public DateTime DeletedAt { get; set; }
    }
}
