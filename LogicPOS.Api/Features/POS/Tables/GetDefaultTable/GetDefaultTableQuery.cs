using ErrorOr;
using LogicPOS.Api.Features.POS.Tables.Common;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Tables.GetDefaultTable
{
    public class GetDefaultTableQuery : IRequest<ErrorOr<TableViewModel>>
    {
        public Guid? TerminalId { get; set; }
        public GetDefaultTableQuery(Guid terminalId) => TerminalId = terminalId;

        public string GetUrlQuery()
        {
            var queryBuilder = new StringBuilder("?");

            if (TerminalId != null)
            {
                queryBuilder.Append($"TerminalId={TerminalId.Value}");
            }

            return queryBuilder.ToString();
        }
    }
}
