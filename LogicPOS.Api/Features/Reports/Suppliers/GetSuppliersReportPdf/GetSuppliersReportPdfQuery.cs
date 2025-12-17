using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Reports.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.GetSuppliersReportPdf
{
    public class GetSuppliersReportPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public GetSuppliersReportPdfQuery()
        {
        }
    }
}
