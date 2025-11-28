namespace LogicPOS.Api.Features.Finance.Agt.ListOnlineDocuments
{
    public class OnlineDocument
    {
        public string Type { get; set; }
        public string Number { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string NetTotal { get; set; }
    }
}
