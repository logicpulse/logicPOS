using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.DocumentTypes.UpdateDocumentType
{
    public class UpdateDocumentTypeCommand : IRequest<ErrorOr<Unit>>
    {
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public Guid Id { get; set; }
        public int NewPrintCopies { get; set; }
        public bool PrintRequestConfirmation { get; set; }
        public bool PrintOpenDrawer { get; set; }
        public string NewNotes { get; set; }
    }
}
