using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Documents.GetDocumentsRelations
{
    public class DocumentRelation
    {
        public Guid DocumentId { get; set; }
        public IEnumerable<string> RelatedDocuments { get; set; }
    }
}
