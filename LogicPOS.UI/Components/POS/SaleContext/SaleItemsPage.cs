using Gtk;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Settings;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public class SaleItemsPage : Box
    {
        public PosOrder Order { get; } = SaleContext.GetCurrentOrder();
        public PosTicket Ticket { get; set; }

        public Window SourceWindow { get; }
        public TreeView GridView { get; set; }
        public SaleItem SelectedItem { get; private set; }
        public GridViewSettings GridViewSettings { get; } = new GridViewSettings();
        public Label LabelTotalLabel { get; private set; }
        public Label LabelTotal { get; private set; }
        public event EventHandler TicketOpened;
        public dynamic Theme { get; }

        public SaleItemsPage(Window parent, dynamic theme)
        {
            SourceWindow = parent;
            this.BorderWidth = 10;
            Theme = theme;

            InitializeGridView();
            Design();
            ShowAll();
        }

        public void Refresh()
        {
            Clear();
            PresentTicketItems();
        }

        public void Clear()
        {
            var model = (ListStore)GridViewSettings.Model;
            model.Clear();
        }

        private void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(SaleItem));

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Model;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();
            AddGridViewEventHandlers();
        }

        public void PresentTicketItems()
        {
            if(Ticket == null)
            {
                return;
            }

            var model = (ListStore)GridViewSettings.Model;
            Ticket.Items.ForEach(entity => model.AppendValues(entity));
        }

        private void AddColumns()
        {
            GridView.AppendColumn(CreateDesignationColumn());
            GridView.AppendColumn(CreatePriceColumn());
            GridView.AppendColumn(CreateQuantityColumn());
            GridView.AppendColumn(CreateTotalColumn());
        }

        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Article.Designation;
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_designation");
            return Columns.CreateColumn(title, 1, RenderDesignation, resizable: false, clickable: false);
        }

        private TreeViewColumn CreatePriceColumn()
        {
            void RenderPrice(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.UnitPrice.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_price");
            return Columns.CreateColumn(title, 2, RenderPrice, resizable: false, clickable: false);
        }

        private TreeViewColumn CreateQuantityColumn()
        {
            void RenderQuantity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Quantity.ToString();
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_quantity");
            return Columns.CreateColumn(title, 3, RenderQuantity, resizable: false, clickable: false);
        }

        private TreeViewColumn CreateTotalColumn()
        {
            void RenderTotal(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.TotalPrice.ToString();
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_total");
            return Columns.CreateColumn(title, 4, RenderTotal, resizable: false, clickable: false);
        }

        private void Design()
        {
            VBox verticalLayout = new VBox(false, 1);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            scrolledWindow.Add(GridView);

            verticalLayout.PackStart(scrolledWindow, true, true, 0);
            verticalLayout.PackStart(CreateTotalPanel(), false, false, 0);

            PackStart(verticalLayout);
        }

        private EventBox CreateTotalPanel()
        {
            Gdk.Color eventBoxTotalBackgroundColor = (Theme.EventBoxTotal.BackgroundColor as string).StringToGdkColor();
            Pango.FontDescription columnsFontTitle = Pango.FontDescription.FromString(Theme.Columns.FontTitle);
            Pango.FontDescription columnsFontData = Pango.FontDescription.FromString(Theme.Columns.FontData);
            Pango.FontDescription labelLabelTotalFont = Pango.FontDescription.FromString(Theme.EventBoxTotal.LabelLabelTotal.Font);
            Gdk.Color labelLabelTotalFontColor = (Theme.EventBoxTotal.LabelLabelTotal.FontColor as string).StringToGdkColor();
            float labelLabelTotalAlignmentX = Convert.ToSingle(Theme.EventBoxTotal.LabelLabelTotal.AlignmentX);
            Pango.FontDescription labelTotalFont = Pango.FontDescription.FromString(Theme.EventBoxTotal.LabelTotal.Font);
            Gdk.Color labelTotalFontColor = (Theme.EventBoxTotal.LabelTotal.FontColor as string).StringToGdkColor();
            float labelTotalAlignmentX = Convert.ToSingle(Theme.EventBoxTotal.LabelTotal.AlignmentX);

            int columnDesignationWidth = Convert.ToInt16(Theme.Columns.DesignationWidth);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Always);

            LabelTotalLabel = new Label(GeneralUtils.GetResourceByName("global_total_ticket"));
            LabelTotalLabel.ModifyFont(labelLabelTotalFont);
            LabelTotalLabel.ModifyFg(StateType.Normal, labelLabelTotalFontColor);
            LabelTotalLabel.SetAlignment(labelLabelTotalAlignmentX, 0.0F);

            LabelTotal = new Label();
            LabelTotal.ModifyFont(labelTotalFont);
            LabelTotal.ModifyFg(StateType.Normal, labelTotalFontColor);
            LabelTotal.SetAlignment(labelTotalAlignmentX, 0.0F);
            LabelTotal.Text = DataConversionUtils.DecimalToStringCurrency(0, XPOSettings.ConfigurationSystemCurrency.Acronym);

            HBox hboxTotal = new HBox(false, 4);
            hboxTotal.PackStart(LabelTotalLabel, true, true, 5);
            hboxTotal.PackStart(LabelTotal, false, false, 5);

            EventBox eventBoxTotal = new EventBox() { BorderWidth = 0 };
            eventBoxTotal.ModifyBg(StateType.Normal, eventBoxTotalBackgroundColor);
            eventBoxTotal.Add(hboxTotal);

            return eventBoxTotal;
        }

        private void GridViewRow_Changed(object sender, EventArgs e)
        {
            TreeSelection selection = GridView.Selection;

            if (selection.GetSelected(out TreeModel model, out GridViewSettings.Iterator))
            {
                GridViewSettings.Path = model.GetPath(GridViewSettings.Iterator);
                SelectedItem = (SaleItem)model.GetValue(GridViewSettings.Iterator, 0);
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

        public void Next()
        {
            if (GridViewSettings.Path == null)
            {
                return;
            }

            GridViewSettings.Path.Next();
            GridView.SetCursor(GridViewSettings.Path, null, false);
        }

        public void Previous()
        {
            if (GridViewSettings.Path == null)
            {
                return;
            }

            GridViewSettings.Path.Prev();
            GridView.SetCursor(GridViewSettings.Path, null, false);
        }

        public void DecreaseQuantity(Guid articleId)
        {
            var item = Ticket.Items.FirstOrDefault(x => x.Article.Id == articleId);

            if (item == null)
            {
                return;
            }

            var defaultQuantity = item.Article.DefaultQuantity > 0 ? item.Article.DefaultQuantity : 1;
            item.Quantity -= defaultQuantity;

            if (item.Quantity <= 0)
            {
                RemoveItem(item);
                return;
            }

            Refresh();
            SelectItem(item);
        }

        public void RemoveItem(SaleItem item)
        {
            Ticket.Items.Remove(item);

            if (SelectedItem == item)
            {
                SelectedItem = null;
            }

            Refresh();
        }

        public void IncreaseQuantity(Guid articleId)
        {
            var item = Ticket.Items.FirstOrDefault(x => x.Article.Id == articleId);

            if (item == null)
            {
                return;
            }

            var defaultQuantity = item.Article.DefaultQuantity > 0 ? item.Article.DefaultQuantity : 1;
            item.Quantity += defaultQuantity;

            Refresh();
            SelectItem(item);
        }

        public void AddItem(SaleItem item)
        {
            if(Ticket == null)
            {
                OpenTicket(item);
                return;
            }

            if (ContainsItem(item))
            {
                IncreaseQuantity(item.Article.Id);
                return;
            }

            Ticket.Items.Add(item);
            PresentLastItem();
            SelectItem(item);
        }

        private void OpenTicket(SaleItem item)
        {
            Clear();
            GridView.ModifyBase(StateType.Normal, AppSettings.Instance.colorPosTicketListModeTicketBackground.ToGdkColor());
            Ticket = Order.AddTicket(new List<SaleItem> { item });
            PresentLastItem();
            SelectItem(item);
            TicketOpened?.Invoke(this, EventArgs.Empty);
        }

        private void PresentLastItem()
        {
            var model = (ListStore)GridViewSettings.Model;
            model.AppendValues(Ticket.Items.Last());
        }

        public bool ContainsItem(SaleItem item)
        {
            return Ticket.Items.Any(x => x.Article.Id == item.Article.Id);
        }

        private void SelectItem(SaleItem item)
        {
            var model = (ListStore)GridViewSettings.Model;
            var index = Ticket.Items.IndexOf(item);
            var path = new TreePath(new int[] { index });
            GridView.SetCursor(path, null, false);
            SelectedItem = item;
        }

        public void FinishTicket()
        {
            Ticket = null;
            Clear();
            PresentOrderItems();
            GridView.ModifyBase(StateType.Normal, AppSettings.Instance.colorPosTicketListModeOrderMainBackground.ToGdkColor());
        }

        public void PresentOrderItems()
        {
            var model = (ListStore)GridViewSettings.Model;
            var orderItems = Order.Tickets.SelectMany(x => x.Items).ToList();
            orderItems.ForEach(entity => model.AppendValues(entity));
        }

        public void ChangeItemQuantity(SaleItem item, decimal quantity)
        {
            item.Quantity = quantity;
            Refresh();
            SelectItem(item);
        }

        public void ChangeItemPrice(SaleItem item, decimal price)
        {
            item.UnitPrice = price;
            Refresh();
            SelectItem(item);
        }
    }
}
