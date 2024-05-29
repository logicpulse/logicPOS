using System;

namespace LogicPOS.DTOs.Printing
{
    public class PrintingFinancePaymentDto
    {
        public Guid Id { get; set; }
        public decimal ExchangeRate { get; set; }
        public string ExtendedValue { get; set; }

        public PrintingDocumentTypeDto DocumentType { get; set; }
    }
}
