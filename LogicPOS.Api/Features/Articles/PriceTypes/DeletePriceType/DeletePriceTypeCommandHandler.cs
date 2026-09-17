using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.PriceTypes.DeletePriceType
{
    public class DeletePriceTypeCommandHandler :
        RequestHandler<DeletePriceTypeCommand, ErrorOr<bool>>
    {
        public DeletePriceTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeletePriceTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"articles/pricetypes/{command.Id}", cancellationToken);
        }
    }
}
