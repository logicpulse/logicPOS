using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;

namespace LogicPOS.Api.Features.Company.GetCompanyCurreny
{
    public class GetCompanyCurrencyQuery : IRequest<ErrorOr<Currency>>
    {

    }
}
