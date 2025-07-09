using Gtk;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Windows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public partial class SaleItemsPage : Box
    {
        public event EventHandler TicketOpened;

        public dynamic Theme { get; }

        public SaleItemsPage(Window parent,
                             dynamic theme)
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
            UpdateLabelTotalValue();
        }

        public void Clear(bool removeTicket = false)
        {
            if (removeTicket)
            {
                Ticket = null;
            }

            var model = (ListStore)GridViewSettings.Model;
            model.Clear();

            SetTicketModeBackGround();
            UpdateLabelTotalValue();
        }

        public void PresentOrderItems()
        {
            var orderItems = SaleContext.CurrentOrder.GetOrderItems();

            if (orderItems.Any() == false)
            {
                SetTicketModeBackGround();
                return;
            }

            SetOrderModeBackGround();
            var model = (ListStore)GridViewSettings.Model;

            orderItems.ForEach(entity => model.AppendValues(entity));

            UpdateLabelTotalValue();
        }

        public void PresentTicketItems()
        {
            if (Ticket == null)
            {
                return;
            }

            var model = (ListStore)GridViewSettings.Model;
            Ticket.Items.ForEach(entity => model.AppendValues(entity));
        }

        public void UpdateLabelTotalValue()
        {
            var total = Ticket?.TotalFinal ?? SaleContext.CurrentOrder?.TotalFinal ?? 0;
            LabelTotalValue.Text = total.ToString("C");
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
            if (Ticket == null)
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
            UpdateLabelTotalValue();
        }
        private void OpenTicket(SaleItem item)
        {
            Clear(true);
            SetTicketModeBackGround();
            Ticket = SaleContext.CurrentOrder?.AddTicket(new List<SaleItem> { item });
            PresentLastItem();
            SelectItem(item);
            UpdateLabelTotalValue();
            TicketOpened?.Invoke(this, EventArgs.Empty);
        }

        public void FinishTicket()
        {
            Clear(true);
            SetOrderModeBackGround();
            PresentOrderItems();
            UpdateLabelTotalValue();
            POSWindow.Instance.UpdateUI();
        }

        private void PresentLastItem()
        {
            if (Ticket == null)
            {
                return;
            }

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
