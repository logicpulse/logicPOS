using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PaymentMethodsPage
    {
        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var paymentMethod = (PaymentMethod)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = paymentMethod.Acronym;
            }

            var title = GeneralUtils.GetResourceByName("global_ConfigurationPaymentMethod_Acronym");
            return Columns.CreateColumn(title, 2, RenderMonth);
        }

    }
}
