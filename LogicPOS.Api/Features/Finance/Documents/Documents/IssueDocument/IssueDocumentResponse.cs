using System;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument
{
    public struct IssueDocumentResponse
    {
        public Guid Id { get; set; }
        public string AtDocCodeId { get; set; }

        public bool HasAtRegistration => !string.IsNullOrWhiteSpace(AtDocCodeId);
    }
}
