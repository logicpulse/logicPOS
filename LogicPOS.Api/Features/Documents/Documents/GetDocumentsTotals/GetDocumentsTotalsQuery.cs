using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPOS.Api.Features.Documents.GetDocumentsTotals
{
    public class GetDocumentsTotalsQuery : IRequest<ErrorOr<IEnumerable<DocumentTotals>>>
    {
        public IEnumerable<Guid> Ids { get; set; }

        public GetDocumentsTotalsQuery(IEnumerable<Guid> ids)
        {
            Ids = ids;
        }

        public string GetUrlQuery()
        {
            var query = new StringBuilder("?");

            foreach (var id in Ids)
            {
                query.Append($"&ids={id}");
            }

            return query.ToString();
        }
    }
}
