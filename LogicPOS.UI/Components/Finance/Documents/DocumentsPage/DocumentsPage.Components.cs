using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage
    {
        public List<DocumentViewModel> SelectedDocuments { get; private set; } = new List<DocumentViewModel>();
        public decimal SelectedDocumentsTotalFinal { get; private set; }
    }
}
