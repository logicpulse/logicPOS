using System;

namespace LogicPOS.DTOs.Printing
{
    public class PrintDocumentMasterDto
    {
        public Guid Id { get; set; }
        public string ATDocQRCode { get; set; }
        public string Hash { get; set; }
        public string TableDesignation { get; set; }
        public string PlaceDesignation { get; set; }
        public bool HasValidPaymentMethod { get; set; }
        public PrintingDocumentTypeDto DocumentType { get; set; }
    }
}
