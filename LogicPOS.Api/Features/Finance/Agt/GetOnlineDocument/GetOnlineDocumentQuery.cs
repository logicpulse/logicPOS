using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Agt.GetOnlineDocument
{
    public class GetOnlineDocumentQuery : IRequest<ErrorOr<OnlineDocument>>
    {
        public string Number { get; set; }

        public GetOnlineDocumentQuery(string number)
        {
            Number = number;
        }
    }
}
