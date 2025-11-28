using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.Finance.Agt.GetOnlineDocument
{
    public class GetOnlineDocumentQuery : IRequest<ErrorOr<OnlineDocument>>
    {
        public string DocumentNumber { get; set; }

        public GetOnlineDocumentQuery(string documentNumber)
        {
            DocumentNumber = documentNumber;
        }
    }
}
