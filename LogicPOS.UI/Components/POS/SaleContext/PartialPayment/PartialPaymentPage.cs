using Gtk;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.POS;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public class PartialPaymentPage : Box
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
 
        private TreeViewColumn CreateSelectColumn()
        {
            TreeViewColumn selectColumn = new TreeViewColumn();

            var selectCellRenderer = new CellRendererToggle();
            selectColumn.PackStart(selectCellRenderer, true);

            selectCellRenderer.Toggled += CheckBox_Clicked;

            void RenderSelect(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererToggle).Active = SelectedItems.Contains(item);
            }

            selectColumn.SetCellDataFunc(selectCellRenderer, RenderSelect);

            return selectColumn;
        }

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            if (GridView.Model.GetIter(out TreeIter iterator, new TreePath(args.Path)))
            {
                var item = (SaleItem)GridView.Model.GetValue(iterator, 0);

                if (SelectedItems.Contains(item))
                {
                    SelectedItems.Remove(item);
                }
                else
                {
                    SelectedItems.Add(item);
                }
            }
        }

        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Article.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_designation");
            return Columns.CreateColumn(title, 2, RenderDesignation);
        }

        private TreeViewColumn CreatePriceColumn()
        {
            void RenderPrice(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.UnitPrice.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("global_price");
            return Columns.CreateColumn(title, 3, RenderPrice);
        }

        private TreeViewColumn CreateVatColumn()
        {
            void RenderVat(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Vat.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_vat_rate");
            return Columns.CreateColumn(title, 4, RenderVat);
        }

        private TreeViewColumn CreateDiscountColumn()
        {
            void RenderTotal(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Discount.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_discount");
            return Columns.CreateColumn(title, 5, RenderTotal);
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

        private void GridViewRow_Changed(object sender, EventArgs e)
        {
            TreeSelection selection = GridView.Selection;

            if (selection.GetSelected(out TreeModel model, out GridViewSettings.Iterator))
            {
                GridViewSettings.Path = model.GetPath(GridViewSettings.Iterator);
            };
        }

        protected virtual void AddGridViewEventHandlers()
        {
            GridView.CursorChanged += GridViewRow_Changed;
            GridView.RowActivated += GridView_RowActivated;
            GridView.Vadjustment.ValueChanged += delegate { };
            GridView.Vadjustment.Changed += delegate { };
        }

        private void GridView_RowActivated(object o, RowActivatedArgs args)
        {

        }
    }
}
