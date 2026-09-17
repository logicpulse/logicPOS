using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.DocumentTypes.UpdateDocumentType
{
    public class UpdateDocumentTypeCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public int PrintCopies { get; set; }
        public bool PrintRequestConfirmation { get; set; }
        public bool PrintOpenDrawer { get; set; }
    }
}
