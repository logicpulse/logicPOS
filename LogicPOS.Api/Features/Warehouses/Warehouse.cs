using LogicPOS.Api.Features.Common;
using System.Collections.Generic;

namespace LogicPOS.Api.Entities
{
    public class Warehouse : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public bool IsDefault { get; set; }
    }
}
