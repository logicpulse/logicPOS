using Gtk;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;

namespace LogicPOS.UI.Components.Pages
{
    public partial class CustomersPage
    {
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddNameSorting();
            AddCardNumberSorting();
            AddFiscalNumberSorting();
            AddUpdatedAtSorting(4);
        }

        private void AddNameSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var leftCustomer = (Customer)model.GetValue(left, 0);
                var rightCustomer = (Customer)model.GetValue(right, 0);

                if (leftCustomer == null || rightCustomer == null)
                {
                    return 0;
                }

                return leftCustomer.Name.CompareTo(rightCustomer.Name);
            });
        }

        private void AddCardNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftCustomer = (Customer)model.GetValue(left, 0);
                var rightCustomer = (Customer)model.GetValue(right, 0);

                if (leftCustomer == null || rightCustomer == null)
                {
                    return 0;
                }

                return leftCustomer.CardNumber?.CompareTo(rightCustomer.CardNumber) ?? 0;
            });
        }

        private void AddFiscalNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftCustomer = (Customer)model.GetValue(left, 0);
                var rightCustomer = (Customer)model.GetValue(right, 0);

                if (leftCustomer == null || rightCustomer == null)
                {
                    return 0;
                }

                return leftCustomer.FiscalNumber.CompareTo(rightCustomer.FiscalNumber);
            });
        }

    }
}
