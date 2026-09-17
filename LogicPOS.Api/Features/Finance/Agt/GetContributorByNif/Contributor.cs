namespace LogicPOS.Api.Features.Finance.Agt.GetContributorByNif
{
    public class Contributor
    {
        public string Nif { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string IvaRegime { get; set; }
        public string NonResidentIndicator { get; set; }
    }
}
