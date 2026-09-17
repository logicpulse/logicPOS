using Gtk;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.POS;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PartialPaymentPage : Box
    {
        public PosOrder Order { get; }
        public Window SourceWindow { get; }
        public TreeView GridView { get; set; }
        public List<SaleItem> SelectedItems { get; private set; } = new List<SaleItem>();
        public GridViewSettings GridViewSettings { get; } = new GridViewSettings();

        public PartialPaymentPage(Window parent,
                                PosOrder order)
        {
            Order = order;
            SourceWindow = parent;

            InitializeGridView();
            PresentOrderItems();
            Design();
            ShowAll();
        }

        private void PresentOrderItems()
        {
            var items = Order.Tickets.SelectMany(t => t.Items).ToList();
            items = SaleItem.Uncompact(items).ToList();

            var model = (ListStore)GridViewSettings.Model;

            items.ForEach(entity => model.AppendValues(entity));
        }

        private void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(SaleItem), typeof(bool));

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Model;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));
            AddColumns();
            AddGridViewEventHandlers();
        }

        private void AddColumns()
        {
            GridView.AppendColumn(CreateSelectColumn());
            GridView.AppendColumn(Columns.CreateCodeColumn(1));
            GridView.AppendColumn(CreateDesignationColumn());
            GridView.AppendColumn(CreatePriceColumn());
            GridView.AppendColumn(CreateVatColumn());
            GridView.AppendColumn(CreateDiscountColumn());
        }
        private void Design()
        {
            VBox verticalLayout = new VBox(false, 1);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            scrolledWindow.Add(GridView);

            verticalLayout.PackStart(scrolledWindow, true, true, 0);

            PackStart(verticalLayout);
        }
        protected virtual void AddGridViewEventHandlers()
        {
            GridView.CursorChanged += GridViewRow_Changed;
            GridView.RowActivated += GridView_RowActivated;
            GridView.Vadjustment.ValueChanged += delegate { };
            GridView.Vadjustment.Changed += delegate { };
        }

    }
}
