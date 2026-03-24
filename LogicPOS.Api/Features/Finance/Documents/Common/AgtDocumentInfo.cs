using System;

namespace LogicPOS.Api.Features.Finance.Documents.Common
{
    public class AgtDocumentInfo
    {
        public DateTime? SubmissionDate { get; set; }
        public string RequestId { get; set; }
        public string SubmissionErrorCode { get; set; }
        public string SubmissionErrorDescription { get; set; }
        public int? HttpStatusCode { get; set; }
        public string ValidationResultCode { get; set; }
        public string ValidationStatus { get; set; }
        public string ValidationErrors { get; set; }
        public string SubmissionUuid { get; set; }
        public string RejectedDocumentNumber { get; set; }
    }
}
