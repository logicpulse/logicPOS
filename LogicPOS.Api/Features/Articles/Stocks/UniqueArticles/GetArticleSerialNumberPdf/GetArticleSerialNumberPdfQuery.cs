using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.StockManagement.GetArticleSerialNumberPdf
{
    public class GetArticleSerialNumberPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public Guid Id { get; set; }

        public GetArticleSerialNumberPdfQuery(Guid id)
        {
            Id = id;
        }
    }
}
