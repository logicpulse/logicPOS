using LogicPOS.Api.Features.POS.Tables.Common;
using LogicPOS.UI.Components.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TablesPage
    {
        private void AddPlaceSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftTable = (TableViewModel)model.GetValue(left, 0);
                var rightTable = (TableViewModel)model.GetValue(right, 0);

                if (leftTable == null || rightTable == null)
                {
                    return 0;
                }

                return leftTable.Place.CompareTo(rightTable.Place);
            });
        }
    }
}
