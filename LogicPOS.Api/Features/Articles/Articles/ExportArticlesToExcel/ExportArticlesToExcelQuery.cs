using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;

namespace LogicPOS.Api.Features.Articles.Articles.ExportArticlesToExcel
{
    public class ExportArticlesToExcelQuery : IRequest<ErrorOr<TempFile>>
    {

    }
}
