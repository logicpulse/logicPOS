using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class DocumentPrint : ApiEntity
    {
        public string Designation { get; set; }
        public string CopyNames { get; set; }
        public int PrintCopies { get; set; }
        public string PrintMotive { get; set; }
        public bool SecondPrint { get; set; }
        public Guid? DocumentId { get; set; }
        public Guid? ReceiptId { get; set; }
        public bool IsThermalPrint { get; set; }
    }
}
