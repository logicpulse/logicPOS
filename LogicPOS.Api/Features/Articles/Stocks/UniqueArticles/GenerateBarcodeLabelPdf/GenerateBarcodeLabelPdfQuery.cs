using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Articles.Stocks.UniqueArticles.GenerateBarcodeLabelPdf
{
    public class GenerateBarcodeLabelPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public List<Guid> Ids { get; set; }

        public GenerateBarcodeLabelPdfQuery(List<Guid> ids) => Ids = ids;


        public string GetUrlQuery()
        {
            StringBuilder builder = new StringBuilder("?");
            for (int i = 0; i < Ids.Count; i++)
            {
                builder.Append($"ids={Ids[i]}" + (i == Ids.Count - 1 ? "" : "&"));
            }

            return builder.ToString();
        }
    }
}
