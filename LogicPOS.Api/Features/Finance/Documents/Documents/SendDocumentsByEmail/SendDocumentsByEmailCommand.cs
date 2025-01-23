using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Documents.SendDocumentsByEmail
{
    public class SendDocumentsByEmailCommand : IRequest<ErrorOr<Unit>>
    {
        public string Subject { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Body { get; set; }
        public IEnumerable<Guid> DocumentsIds { get; set; }
    }
}
