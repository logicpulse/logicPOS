using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Entities
{
    public class Holiday: ApiEntity, IWithCode,IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Description { get; set; }
        public int Day {  get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
