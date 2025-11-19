using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class FiscalYearsPage
    { 
        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var fiscalYear = (FiscalYear)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = fiscalYear.Acronym;
            }

            var title = GeneralUtils.GetResourceByName("global_acronym");
            return Columns.CreateColumn(title, 2, RenderValue);
        }

        private TreeViewColumn CreateYearColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var fiscalYear = (FiscalYear)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = fiscalYear.Year.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_year");
            return Columns.CreateColumn(title, 3, RenderValue);
        }
    }
}
