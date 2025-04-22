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
    public partial class PaymentConditionsPage
    {
        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var paymentCondition = (PaymentCondition)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = paymentCondition.Acronym.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_ConfigurationPaymentCondition_Acronym");
            return Columns.CreateColumn(title, 2, RenderMonth);
        }
    }
}
