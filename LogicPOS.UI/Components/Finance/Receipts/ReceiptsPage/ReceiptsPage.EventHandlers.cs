using Gtk;
using LogicPOS.Api.Entities;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ReceiptsPage
    {
        public void MoveToNextPage()
        {
            Query.Page = Receipts.Page + 1;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        public void MoveToPreviousPage()
        {
            Query.Page = Receipts.Page - 1;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            if (GridView.Model.GetIter(out TreeIter iterator, new TreePath(args.Path)))
            {
                var receipt = (Receipt)GridView.Model.GetValue(iterator, 0);

                if (SelectedReceipts.Contains(receipt))
                {
                    SelectedReceipts.Remove(receipt);
                    SelectedReceiptsTotalAmount -= receipt.Amount;
                }
                else
                {
                    SelectedReceipts.Add(receipt);
                    SelectedReceiptsTotalAmount += receipt.Amount;
                }

                PageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void UpdateButtonPrevileges()
        {
            //these buttons are not used in this page, so we do nothing here
        }
    }
}
