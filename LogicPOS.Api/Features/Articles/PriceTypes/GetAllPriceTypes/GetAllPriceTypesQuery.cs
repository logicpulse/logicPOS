using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes
{
    public class GetAllPriceTypesQuery : IRequest<ErrorOr<IEnumerable<PriceType>>>
    {

    }
}
