using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Entities
{
    public class FiscalYear : ApiEntity, IWithDesignation, IWithCode
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public int Year { get; set; }
        public string Acronym { get; set; }
        public bool SeriesForEachTerminal { get; set; }
    }
}
