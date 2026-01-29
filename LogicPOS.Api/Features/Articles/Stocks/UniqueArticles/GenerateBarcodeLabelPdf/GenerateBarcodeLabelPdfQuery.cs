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
        public List<string> SerialNumbers { get; set; }
        public BarcodeLabelPrintModel PrintModel { get; set; }

        public GenerateBarcodeLabelPdfQuery(List<Guid> ids, BarcodeLabelPrintModel printModel)
        {
            Ids = ids;
            PrintModel = printModel;
        }
        public GenerateBarcodeLabelPdfQuery(List<string> serialNumbers, BarcodeLabelPrintModel printModel)
        {
            SerialNumbers = serialNumbers;
            PrintModel = printModel;
        }


        public string GetIdsUrlQuery()
        {
            StringBuilder builder = new StringBuilder("?");
            for (int i = 0; i < Ids.Count; i++)
            {
                builder.Append($"Ids={Ids[i]}" + (i == Ids.Count - 1 ? "" : "&"));
            }

            return builder.ToString();
        }

        public string GetSerialNumbersUrlQuery()
        {
            StringBuilder builder = new StringBuilder("?");
            for (int i = 0; i < SerialNumbers.Count; i++)
            {
                builder.Append($"SerialNumbers={SerialNumbers[i]}" + (i == SerialNumbers.Count - 1 ? "" : "&"));
            }

            return builder.ToString();
        }

        public string GetUrlQuery() => Ids != null ? GetIdsUrlQuery() : GetSerialNumbersUrlQuery();
    }
}
