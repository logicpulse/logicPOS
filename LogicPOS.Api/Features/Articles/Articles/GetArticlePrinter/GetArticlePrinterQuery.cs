using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.GetArticlePrinter
{
    public class GetArticlePrinterQuery : IRequest<ErrorOr<Printer>>
    {
        public Guid Id { get; set; }

        public GetArticlePrinterQuery(Guid id)
        {
            Id = id;
        }
    }
}
