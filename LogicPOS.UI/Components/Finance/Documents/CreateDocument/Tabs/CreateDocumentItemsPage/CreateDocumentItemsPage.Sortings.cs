using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentItemsPage
    {
        private void AddTotalWithTaxSorting()
        {
            GridViewSettings.Sort.SetSortFunc(7, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.TotalFinal.CompareTo(rightItem.TotalFinal);
            });
        }

        private void AddQuantitySorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.Quantity.CompareTo(rightItem.Quantity);
            });
        }

        private void AddPriceSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.UnitPrice.CompareTo(rightItem.UnitPrice);
            });
        }

        private void AddDiscountSorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.Discount.CompareTo(rightItem.Discount);
            });
        }

        private void AddTaxSorting()
        {
            GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.VatRate.Designation.CompareTo(rightItem.VatRate.Designation);
            });
        }

        private void AddTotalSorting()
        {
            GridViewSettings.Sort.SetSortFunc(6, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.TotalNet.CompareTo(rightItem.TotalNet);
            });
        }
    }
}
