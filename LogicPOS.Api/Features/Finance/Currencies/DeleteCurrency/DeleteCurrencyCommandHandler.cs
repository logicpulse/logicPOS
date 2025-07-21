using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Currencies.DeleteCurrency
{
    public class DeleteCurrencyCommandHandler :
        RequestHandler<DeleteCurrencyCommand, ErrorOr<bool>>
    {
        public DeleteCurrencyCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteCurrencyCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"currencies/{command.Id}", cancellationToken);
        } 
    }
}
