using LogicPOS.Api.Features.Documents.GetDocumentsRelations;
using LogicPOS.Api.Features.Documents.GetDocumentsTotals;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage
    {
        private List<DocumentTotals> _totals = new List<DocumentTotals>();
        private List<DocumentRelation> _relations = new List<DocumentRelation>();
        public List<DocumentViewModel> SelectedDocuments { get; private set; } = new List<DocumentViewModel>();
        public decimal SelectedDocumentsTotalFinal { get; private set; }
    }
}
