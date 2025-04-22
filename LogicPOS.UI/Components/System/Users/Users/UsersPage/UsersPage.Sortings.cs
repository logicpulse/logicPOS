using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.Pages
{
    public partial class UsersPage
    {
        private void AddNameSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var leftUser = (User)model.GetValue(left, 0);
                var rightUser = (User)model.GetValue(right, 0);

                if (leftUser == null || rightUser == null)
                {
                    return 0;
                }

                return leftUser.Name.CompareTo(rightUser.Name);
            });
        }

        private void AddProfileSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftUser = (User)model.GetValue(left, 0);
                var rightUser = (User)model.GetValue(right, 0);

                if (leftUser == null || rightUser == null)
                {
                    return 0;
                }

                return leftUser.Profile.Designation.CompareTo(rightUser.Profile.Designation);
            });
        }

        private void AddFiscalNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftUser = (User)model.GetValue(left, 0);
                var rightUser = (User)model.GetValue(right, 0);

                if (leftUser == null || rightUser == null)
                {
                    return 0;
                }

                return leftUser.FiscalNumber?.CompareTo(rightUser?.FiscalNumber) ?? 0;
            });
        }
    }
}
