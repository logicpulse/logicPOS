using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.Pages
{
    public partial class VatRatesPage
    {
        private void AddValueSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftVatRate = (VatRate)model.GetValue(left, 0);
                var rightVatRate = (VatRate)model.GetValue(right, 0);

                if (leftVatRate == null || rightVatRate == null)
                {
                    return 0;
                }

                return leftVatRate.Value.CompareTo(rightVatRate.Value);
            });
        }

    }
}
