using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentPdf
{
    public class GetDocumentPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public GetDocumentPdfQuery(Guid id , IEnumerable<int> copies = null) {
            Id = id;
            
            if(copies == null)
            {
                copies = new int[] { 1 };
            }

            Copies = string.Join(",",copies);
        }

        public GetDocumentPdfQuery(string number, IEnumerable<int> copies = null)
        {
            Number = number;
            if (copies == null)
            {
                copies = new int[] { 1 };
            }
            Copies = string.Join(",", copies);
        }

        public string Number { get; set; }
        public string Copies { get; set; }
        public Guid? Id { get; set; }

        public string GetUrlQuery()
        {
            if (Id.HasValue)
            {
                return $"?id={Id}&copies={Copies}";
            }
            else
            {
                return $"?number={Number}&copies={Copies}";
            }
        }
    }
}
