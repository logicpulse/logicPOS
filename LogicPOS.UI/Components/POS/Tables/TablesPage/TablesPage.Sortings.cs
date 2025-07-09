using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TablesPage
    {
        private void AddPlaceSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftTable = (Table)model.GetValue(left, 0);
                var rightTable = (Table)model.GetValue(right, 0);

                if (leftTable == null || rightTable == null)
                {
                    return 0;
                }

                return leftTable.Place.Designation.CompareTo(rightTable.Place.Designation);
            });
        }
    }
}
