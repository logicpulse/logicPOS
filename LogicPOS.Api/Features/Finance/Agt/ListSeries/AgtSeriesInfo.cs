using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Features.Finance.Agt.ListSeries
{
    public class AgtSeriesInfo : ApiEntity
    {
        public string Code { get; set; }
        public string Year { get; set; }
        public string DocumentType { get; set; }
        public string Status { get; set; }
        public string SeriesCreationDate { get; set; }
        public string FirstDocumentCreated { get; set; }
        public string LastDocumentCreated { get; set; }
        public string InvoicingMethod { get; set; }
        public string Nif { get; set; }
        public string Name { get; set; }
        public string JoiningDate { get; set; }
        public string JoiningType { get; set; }
    }
}
