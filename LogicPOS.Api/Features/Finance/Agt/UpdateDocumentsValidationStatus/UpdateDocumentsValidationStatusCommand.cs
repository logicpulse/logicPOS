using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Agt.UpdateDocumentsValidationStatus
{
    public class UpdateDocumentsValidationStatusCommand : IRequest<ErrorOr<Success>>
    {
        public IEnumerable<Guid> Documents { get; set; }
        public UpdateDocumentsValidationStatusCommand(IEnumerable<Guid> documents) => Documents = documents;
    }
}
