using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PreferenceParametersPage
    {
        private TreeViewColumn CreateValueColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var parameter = (PreferenceParameter)model.GetValue(iter, 0);


                if (parameter.Token.Contains("LOGO"))
                {
                    if(string.IsNullOrEmpty(parameter.Value) || parameter.Value.Contains("."))
                    {

                    (cell as CellRendererText).Text = parameter.Value;
                        return;
                    }
                    (cell as CellRendererText).Text = "ConvertedTempFileLogo.png";
                }
                else
                {
                    (cell as CellRendererText).Text = parameter.Value;
                }
            }

            var title = GeneralUtils.GetResourceByName("global_value");
            var valueColumn = Columns.CreateColumn(title, 1, RenderValue);

            return valueColumn;
        }

        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn col, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var parameter = (PreferenceParameter)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = parameter.ResourceStringValue;
            }

            var title = GeneralUtils.GetResourceByName("global_designation");

            var column = Columns.CreateColumn(title,
                                              0,
                                              RenderDesignation);
            column.MinWidth = 250;
            column.MaxWidth = 800;

            return column;
        }
    }
}
