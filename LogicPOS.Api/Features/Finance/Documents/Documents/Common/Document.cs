using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Documents.Documents.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetPrintingModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Api.Entities
{
    public class Document : ApiEntity
    {
        public Guid CustomerId { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public ShipAddress ShipToAddress { get; set; }
        public ShipAddress ShipFromAddress { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalFinal { get; set; }
        public DocumentCustomer Customer { get; set; }
        public List<DocumentPaymentMethod> PaymentMethods { get; set; }
        public DocumentTypeAnalyzer TypeAnalyzer => new DocumentTypeAnalyzer(Type);

        public List<DocumentDetail> Details { get; set; }
        

        public List<TaxResume> GetTaxResumes()
        {
            var taxResumes = new List<TaxResume>();

            if (Details == null)
            {
                return taxResumes;
            }

            var ratesGroups = Details.GroupBy(d => d.Tax.Percentage);

            foreach (var group in ratesGroups)
            {
                var resume = new TaxResume();
                resume.Code = group.First().Tax.Code;
                resume.Designation = group.First().Tax.Designation;
                resume.Rate = group.First().Tax.Percentage;
                resume.Base = group.Sum(d => d.TotalNet);
                resume.Total = group.Sum(d => d.TotalTax);
                taxResumes.Add(resume);
            }

            return taxResumes;
        }
    }
}
