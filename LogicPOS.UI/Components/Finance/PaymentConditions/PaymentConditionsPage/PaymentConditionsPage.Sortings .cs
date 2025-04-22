using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PaymentConditionsPage
    {
        private void AddAcronymSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftPaymentCondition = (PaymentCondition)model.GetValue(left, 0);
                var rightPaymentCondition = (PaymentCondition)model.GetValue(right, 0);

                if (leftPaymentCondition == null || rightPaymentCondition == null)
                {
                    return 0;
                }

                return leftPaymentCondition.Acronym.CompareTo(rightPaymentCondition.Acronym);
            });
        }
    }
}
