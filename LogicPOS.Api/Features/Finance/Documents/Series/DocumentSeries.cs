using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class DocumentSeries : ApiEntity, IWithCode, IWithDesignation
    {
        public string Code { get; set; }
        public string Designation { get; set; }
        public int NextNumber { get; set; }
        public int NumberRangeBegin { get; set; }
        public int NumberRangeEnd { get; set; }
        public string Acronym { get; set; } 
        public DocumentType DocumentType { get; set; }
        public FiscalYear FiscalYear { get; set; }
        public string ATDocCodeValidationSeries { get; set; }
        public Guid? TerminalId { get; set; }
        public Terminal Terminal { get; set; }
    }
}
