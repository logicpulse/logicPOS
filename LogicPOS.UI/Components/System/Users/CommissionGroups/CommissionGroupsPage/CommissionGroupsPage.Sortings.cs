using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.Pages
{
    public partial class CommissionGroupsPage
    {
        private void AddCommissionSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftCommissionGroup = (CommissionGroup)model.GetValue(left, 0);
                var rightCommissionGroup = (CommissionGroup)model.GetValue(right, 0);

                if (leftCommissionGroup == null || rightCommissionGroup == null)
                {
                    return 0;
                }

                return leftCommissionGroup.Commission.CompareTo(rightCommissionGroup.Commission);
            });
        }
    }
}
