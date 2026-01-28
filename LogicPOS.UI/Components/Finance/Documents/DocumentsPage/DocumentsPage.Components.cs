using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage
    {
        public List<DocumentViewModel> SelectedDocuments { get; private set; } = new List<DocumentViewModel>();
        public decimal SelectedDocumentsTotalFinal { get; private set; }

        protected override void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(Document), typeof(bool));

            InitializeGridViewModel();

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Sort;
            GridView.EnableSearch = true;
            GridView.SearchColumn = 1;

            GridView.RulesHint = true;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();
            AddGridViewEventHandlers();
        }
    }
}
