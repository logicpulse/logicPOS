namespace LogicPOS.Api.Features.Finance.Agt.GetOnlineDocument
{
    public struct OnlineDocument
    {
        public string Type { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string CustomerNif { get; set; }
        public string TaxPayable { get; set; }
        public string NetTotal { get; set; }
        public string GrossTotal { get; set; }
    }
}
