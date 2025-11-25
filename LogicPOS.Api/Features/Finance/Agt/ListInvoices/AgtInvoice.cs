namespace LogicPOS.Api.Features.Finance.Agt.ListInvoices
{
    public class AgtInvoice
    {
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentDate { get; set; }
        public string DocumentStatus { get; set; }
        public string DocumentStatusDescription { get; set; }
        public string NetTotal { get; set; }
    }
}
