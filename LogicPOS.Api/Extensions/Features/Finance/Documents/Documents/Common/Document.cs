using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Documents.Documents.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Api.Entities
{
    public class Document : ApiEntity
    {
        #region  References
        public List<DocumentPaymentMethod> PaymentMethods { get; set; }

        public PaymentCondition PaymentCondition { get; set; }
        public Guid? PaymentConditionId { get; set; }

        public Currency Currency { get; set; }
        public Guid CurrencyId { get; set; }

        public DocumentSeries Series { get; set; }
        public Guid SeriesId { get; set; }

        public Guid CustomerId { get; set; }
        #endregion
        public string Type { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string Date => CreatedAt.ToShortDateString();
        public ShipAddress ShipToAdress { get; set; }
        public ShipAddress ShipFromAdress { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalFinal { get; set; }
        public decimal TotalDelivery { get; set; }
        public decimal TotalChange { get; set; }
        public decimal Discount { get; set; }
        public bool Paid { get; set; }
        public decimal ExchangeRate { get; set; }
        public DocumentCustomer Customer { get; set; }
        public IList<DocumentDetail> Details { get; set; }
        public bool IsDraft { get; set; }
        public string ATQRCode { get; set; }
        public bool IsCancelled => Status == "A";
        public bool HasPassed48Hours => CreatedAt.AddHours(48) < DateTime.Now;

        public DocumentTypeAnalyzer TypeAnalyzer => new DocumentTypeAnalyzer(Type);

        public List<TaxResume> GetTaxResumes()
        {
            var taxResumes = new List<TaxResume>();

            if (Details == null)
            {
                return taxResumes;
            }

            var ratesGroups = Details.GroupBy(d => d.Tax.Designation);

            foreach (var group in ratesGroups)
            {
                var resume = new TaxResume();
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
