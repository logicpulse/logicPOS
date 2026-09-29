using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetPrintingModel;
using System;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument
{
    public struct IssueDocumentResponse
    {
        public Guid Id { get; set; }
        public string AtDocCodeId { get; set; }

        /// <summary>
        /// Present for non-draft issues when the API embeds print data (avoids a follow-up GET).
        /// </summary>
        public DocumentPrintingModel PrintingModel { get; set; }

        public bool HasAtRegistration => !string.IsNullOrWhiteSpace(AtDocCodeId);

        public bool HasPrintingModel => PrintingModel != null;
    }
}
