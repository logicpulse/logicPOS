using System;

namespace LogicPOS.Api.Features.Finance.Documents.Common
{
    public class AgtInfo
    {
        public Guid DocumentId { get; set; }
        public string RequestId { get; set; }
        public string ValidationResultCode { get; set; }
        public string ValidationStatus { get; set; }
        public string ValidationErrors { get; set; }
    }
}
