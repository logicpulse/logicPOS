using System;

namespace LogicPOS.Api.Features.Finance.Documents.Common
{
    public class AgtInfo
    {
        public string RequestId { get; set; }
        public string Number { get; set; }
        public string Type { get; set; } 
        public string SubmissionErrorCode { get; set; }
        public string SubmissionErrorDescription { get; set; }
        public string ValidationResultCode { get; set; }
        public string ValidationStatus { get; set; }
        public string ValidationErrors { get; set; }
        public string RejectedDocumentNumber { get; set; }
    }
}
