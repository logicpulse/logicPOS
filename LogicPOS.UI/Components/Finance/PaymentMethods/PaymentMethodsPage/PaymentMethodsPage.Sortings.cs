using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PaymentMethodsPage
    {
        private void AddAcronymSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftPaymentMethod = (PaymentMethod)model.GetValue(left, 0);
                var rightPaymentMethod = (PaymentMethod)model.GetValue(right, 0);

                if (leftPaymentMethod == null || rightPaymentMethod == null)
                {
                    return 0;
                }

                return leftPaymentMethod.Acronym.CompareTo(rightPaymentMethod.Acronym);
            });
        }

    }
}
