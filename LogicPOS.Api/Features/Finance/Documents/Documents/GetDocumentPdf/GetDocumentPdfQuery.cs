using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf
{
    public class GetDocumentPdfQuery : IRequest<ErrorOr<string>>
    {
        public GetDocumentPdfQuery(Guid id , uint copyNumber = 1) {
            Id = id;
            CopyNumber = copyNumber;
        }

        public GetDocumentPdfQuery(string number, uint copyNumber = 2)
        {
            Number = number;
            CopyNumber = copyNumber;
        }

        public string Number { get; set; }
        public uint CopyNumber { get; set; }
        public Guid? Id { get; set; }

        public string GetUrlQuery()
        {
            if (Id.HasValue)
            {
                return $"?id={Id}&copyNumber={CopyNumber}";
            }
            else
            {
                return $"?number={Number}&copyNumber={CopyNumber}";
            }
        }
    }
}
