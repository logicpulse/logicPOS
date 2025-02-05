using ErrorOr;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetArticleReportPdf
{
    public class GetArticleReportPdfQuery : IRequest<ErrorOr<string>>
    {
        public GetArticleReportPdfQuery()
        {
        }
    }
}
