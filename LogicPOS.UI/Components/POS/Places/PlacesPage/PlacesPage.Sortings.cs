using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PlacesPage
    {
        private void AddPriceTypeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftPlace = (Place)model.GetValue(left, 0);
                var rightPlace = (Place)model.GetValue(right, 0);

                if (leftPlace == null || rightPlace == null)
                {
                    return 0;
                }

                return leftPlace.PriceType.Designation.CompareTo(rightPlace.PriceType.Designation);
            });
        }

        private void AddMovementTypeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftPlace = (Place)model.GetValue(left, 0);
                var rightPlace = (Place)model.GetValue(right, 0);

                if (leftPlace == null || rightPlace == null)
                {
                    return 0;
                }

                return leftPlace.MovementType.Designation.CompareTo(rightPlace.MovementType.Designation);
            });
        }
    }
}
