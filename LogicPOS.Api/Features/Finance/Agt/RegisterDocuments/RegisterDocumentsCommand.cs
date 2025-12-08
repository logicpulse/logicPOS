using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Agt.RegisterDocuments
{
    public class RegisterDocumentsCommand : IRequest<ErrorOr<Success>>
    {
        public IEnumerable<Guid> Documents { get; set; }
        public RegisterDocumentsCommand(IEnumerable<Guid> documents) => Documents = documents;
    }
}
