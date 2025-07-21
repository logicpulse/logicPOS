using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Countries.DeleteCountry
{
    public class DeleteCountryCommandHandler :
        RequestHandler<DeleteCountryCommand, ErrorOr<bool>>
    {
        public DeleteCountryCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"countries/{request.Id}", cancellationToken);
        }
    }
}
