using Gtk;
using LogicPOS.UI.Components.GridViews;

namespace LogicPOS.UI.Components.POS
{
    public partial class SaleItemsPage
    {
        public PosTicket Ticket { get; set; }
        public Window SourceWindow { get; }
        public TreeView GridView { get; set; }
        public SaleItem SelectedItem { get; private set; }
        public GridViewSettings GridViewSettings { get; } = new GridViewSettings();
        public Label LabelTotal { get; private set; }
        public Label LabelTotalValue { get; private set; }
    }
}
