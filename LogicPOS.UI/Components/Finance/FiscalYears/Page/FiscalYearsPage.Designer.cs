using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Pages
{
    public partial class FiscalYearsPage
    {
        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateAcronymColumn());
            GridView.AppendColumn(CreateYearColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }

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
