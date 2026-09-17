namespace LogicPOS.Api.Features.Documents.Documents.Common
{
    public class TaxResume
    {
        public string Code { get; set; }
        public string Designation { get; set; } 
        public decimal Base { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
    }
}