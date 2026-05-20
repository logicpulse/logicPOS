using Gtk;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using LogicPOS.Globalization;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentTypesPage
    {
        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var documentType = (DocumentType)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = documentType.Acronym.ToString();
            }

            var title = LocalizedString.Instance["global_acronym"];
            return Columns.CreateColumn(title, 2, RenderMonth);
        }
    }
}
