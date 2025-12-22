using Gtk;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Common.Menus;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public partial class SaleItemsPage : Box
    {
        public event EventHandler TicketOpened;
        public SaleItem SelectedItem { get; private set; }
        public PosTicket Ticket { get; set; }
        public bool TicketMode { get; private set; }

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
            TicketMode = false;
            var orderItems = SaleContext.CurrentOrder.GetOrderItems();

            Clear();
            SetOrderModeBackGround();
            if (orderItems.Any() == false)
            {
                UpdateLabelTotalValue();
                return;
            }
            var model = (ListStore)GridViewSettings.Model;
            model.Clear();
            orderItems.ForEach(entity => model.AppendValues(entity));

            UpdateLabelTotalValue();
        }

        public void PresentTicketItems()
        {
            TicketMode = true;
            SetTicketModeBackGround();
            Clear();
            if (Ticket == null)
            {
                UpdateLabelTotalValue();
                return;
            }

            var model = (ListStore)GridViewSettings.Model;
            Ticket.Items.ForEach(entity => model.AppendValues(entity));
            if (Ticket.Items.Any())
            {
            SelectItem(Ticket.Items.Last());

            }
            UpdateLabelTotalValue();
        }

        public void UpdateLabelTotalValue()
        {
            var total = (TicketMode) ? Ticket?.TotalFinal ?? 0 : SaleContext.CurrentOrder?.TotalFinal ?? 0;
            LabelTotalValue.Text = total.ToString("C");
        }

        private void GridViewRow_Changed(object sender, EventArgs e)
        {
            TreeSelection selection = GridView.Selection;

            if (selection.GetSelected(out TreeModel model, out GridViewSettings.Iterator))
            {
                GridViewSettings.Path = model.GetPath(GridViewSettings.Iterator);
                SelectedItem = (SaleItem)model.GetValue(GridViewSettings.Iterator, 0);
            }
            ;
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
            if (Ticket != null)
            {
                return;
            }

            if (!AuthenticationService.UserHasPermission("WORKSESSION_ORDER_MOVE"))
            {
                return;
            }

            var confirmation = CustomAlerts.Question(POSWindow.Instance)
                                    .WithMessage($"Deseja mudar o artigo:  {SelectedItem.Article.Designation} \n" +
                                                            $"Quantidade: {SelectedItem.Quantity:N2} \n" +
                                                            $"Total Final: {SelectedItem.TotalFinal:N2}\n" +
                                                            $" para outra mesa?")
                                    .ShowAlert();

            if (confirmation != ResponseType.Yes)
            {
                return;
            }

            var modal = new TablesModal(MenuMode.SelectOther, POSWindow.Instance);
            var response = (ResponseType)modal.Run();
            modal.Destroy();

            if (response == ResponseType.Ok)
            {
                var selectedTable = modal.GetSelectedTable();
                var table = SaleContext.CurrentTable;
                OrdersService.MoveTicketItem(SaleContext.CurrentOrder.Id.Value, selectedTable.Id, SaleContext.ItemsPage.SelectedItem);
                SaleContext.SetCurrentTable(table);
            }
            return;
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
            POSWindow.Instance.UpdateUI();
        }

        public void FinishTicket()
        {
            Clear(true);
            SaleContext.SetCurrentTable(SaleContext.CurrentTable);
            UpdateLabelTotalValue();
            POSWindow.Instance.UpdateUI();
        }

        private void PresentLastItem()
        {
            if (Ticket == null)
            {
                return;
            }
            Clear();
            var model = (ListStore)GridViewSettings.Model;
            model.AppendValues(Ticket.Items.Last());
        }

        public bool ContainsItem(SaleItem item)
        {
            return Ticket.Items.Any(x => x.Article.Id == item.Article.Id);
        }

        private void SelectItem(SaleItem item)
        {
            var index = Ticket?.Items?.IndexOf(item);

            if (index == null)
            {
                return;
            }

            var path = new TreePath(new int[] { index.Value });
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
            item.SetUnitPrice(price);
            Refresh();
            SelectItem(item);
        }
    }
}
