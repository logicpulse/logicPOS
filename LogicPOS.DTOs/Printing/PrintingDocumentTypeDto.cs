using System;

namespace LogicPOS.DTOs.Printing
{
    public class PrintingDocumentTypeDto
    {
        public Guid Id { get; set; }
        public bool IsSaftDocumentTypePayments { get; set; }
        public int PrintCopies { get; set; }
    }
}
