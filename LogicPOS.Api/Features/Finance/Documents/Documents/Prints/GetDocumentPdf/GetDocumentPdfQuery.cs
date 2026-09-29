using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentPdf
{
    public class GetDocumentPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public GetDocumentPdfQuery(Guid id, bool isSecondCopy, IEnumerable<int> copies = null)
        {
            Id = id;

            if (copies == null)
            {
                copies = new int[] { 1 };
            }

            Copies = string.Join(",", copies);
            IsSecondCopy = isSecondCopy;
        }

        public GetDocumentPdfQuery(string number, bool isSecondCopy, IEnumerable<int> copies = null)
        {
            Number = number;
            if (copies == null)
            {
                copies = new int[] { 1 };
            }
            Copies = string.Join(",", copies);
            IsSecondCopy = isSecondCopy;
        }

        public string Number { get; set; }
        public string Copies { get; set; }
        public Guid? Id { get; set; }
        public bool IsSecondCopy { get; set; }

        public string GetUrlQuery()
        {
            if (Id.HasValue)
            {
                return $"?id={Id}&copies={Copies}&issecondcopy={IsSecondCopy}";
            }
            else
            {
                return $"?number={Number}&copies={Copies}&issecondcopy={IsSecondCopy}";
            }
        }
    }
}
