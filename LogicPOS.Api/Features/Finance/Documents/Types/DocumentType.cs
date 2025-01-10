using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents;

namespace LogicPOS.Api.Entities
{
    public class DocumentType : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Acronym { get; set; }
        public int PrintCopies { get; set; }
        public bool PrintRequestConfirmation { get; set; }
        public bool PrintOpenDrawer { get; set; }

        public DocumentTypeAnalyzer Analyzer => new DocumentTypeAnalyzer(Acronym);
    }
}
