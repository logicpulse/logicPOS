using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.AddDocumentPrint
{
    public class AddDocumentPrintCommand : IRequest<ErrorOr<Guid>>
    {
        public string Copies { get; set; } 
        public string Reason { get; set; }
        public bool SecondPrint { get; set; }
        public Guid? DocumentId { get; set; }
        public Guid? ReceiptId { get; set; }

        public AddDocumentPrintCommand(Guid? documentId, string copies, bool secondPrint, string reason = null)
        {
            Copies = copies;
            Reason = reason;
            SecondPrint = secondPrint;
            DocumentId = documentId;
        }
    }
}
