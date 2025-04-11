using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.Api.Features.Documents.GetDocumentsRelations;
using LogicPOS.Api.Features.Documents.GetDocumentsTotals;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage
    {
        public GetDocumentsQuery Query { get; private set; } = GetDefaultQuery();
        public PaginatedResult<Document> Documents { get; private set; }
        private List<DocumentTotals> _totals = new List<DocumentTotals>();
        private List<DocumentRelation> _relations = new List<DocumentRelation>();
        public List<Document> SelectedDocuments { get; private set; } = new List<Document>();
        public decimal SelectedDocumentsTotalFinal { get; private set; }
        public event EventHandler PageChanged;
    }
}
