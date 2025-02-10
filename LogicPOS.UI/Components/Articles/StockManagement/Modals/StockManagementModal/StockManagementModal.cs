using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Modals.Common;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class StockManagementModal : Modal
    {
        public StockManagementModal(Window parent) : base(parent,
                                                          LocalizedString.Instance["global_stock_movements"],
                                                          new Size(1200, 700),
                                                          windowMode: true)
        {
        }


    }
}
