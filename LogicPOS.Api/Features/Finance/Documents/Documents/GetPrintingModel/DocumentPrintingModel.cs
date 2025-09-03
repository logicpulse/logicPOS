using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Documents.Documents.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.GetPrintingModel
{
    public class DocumentPrintingModel
    {
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public string Currency { get; set; }
        public Customer Customer { get; set; }
        public string PaymentCondition { get; set; }
        public List<Detail> Details { get; set; }
        public List<string> PaymentMethods { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalFinal { get; set; }
        public DocumentTypeAnalyzer TypeAnalyzer => new DocumentTypeAnalyzer(Type);
        public string ATQRCode { get; set; }
        public List<TaxResume> GetTaxResumes()
        {
            var taxResumes = new List<TaxResume>();

            if (Details == null)
            {
                return taxResumes;
            }

            var ratesGroups = Details.GroupBy(d => d.TaxDesignation);

            foreach (var group in ratesGroups)
            {
                var resume = new TaxResume
                {
                    Designation = group.First().TaxDesignation,
                    Rate = group.First().Tax,
                    Base = group.Sum(d => d.TotalNet),
                    Total = group.Sum(d => d.TotalTax)
                };
                taxResumes.Add(resume);
            }

            return taxResumes;
        }
    }

    public class Customer
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string FiscalNumber { get; set; }
        public string Address { get; set; }
    }

    public class Detail
    {
        public decimal Tax { get; set; }
        public string TaxDesignation { get; set; }
        public string Designation { get; set; }
        public decimal Quantity { get; set; }
        public string VatExemptionReason { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalFinal { get; set; }
        public decimal TotalTax { get; set; }
    }


    
}
