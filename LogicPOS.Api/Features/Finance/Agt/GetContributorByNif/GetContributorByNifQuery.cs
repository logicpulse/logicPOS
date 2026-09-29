using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.Finance.Agt.GetContributorByNif
{
    public class GetContributorByNifQuery : IRequest<ErrorOr<Contributor>>
    {
        public string Nif { get; set; }

        public GetContributorByNifQuery(string nif) => Nif = nif;

    }
}
