using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class DocumentSeries : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public int NextNumber { get; set; }
        public int NumberRangeBegin { get; set; }
        public int NumberRangeEnd { get; set; }
        public string Acronym { get; set; } 
        public DocumentType DocumentType { get; set; }
        public Guid DocumentTypeId { get; set; }
        public FiscalYear FiscalYear { get; set; }
        public Guid FiscalYearId { get; set; }
        public string ATDocCodeValidationSerie { get; set; }
    }
}
