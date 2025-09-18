using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Api.Features.Articles.StockManagement.ChangeArticleLocation
{
    public class ChangeArticleLocationCommand : IRequest<ErrorOr<Success>>
    {
        public Guid WarehouseArticleId { get; set; }
        public Guid LocationId { get; set; }
    }
}
