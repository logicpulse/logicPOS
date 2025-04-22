using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ReceiptsPage
    {

        private void AddFiscalNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(6, (model, left, right) =>
            {
                var a = (Receipt)model.GetValue(left, 0);
                var b = (Receipt)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return 0;
            });
        }

        private void AddEntitySorting()
        {
            GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
            {
                var a = (Receipt)model.GetValue(left, 0);
                var b = (Receipt)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return 0;
            });
        }

        private void AddStatusSorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var a = (Receipt)model.GetValue(left, 0);
                var b = (Receipt)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Status.CompareTo(b.Status);
            });
        }

        private void AddDateSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var a = (Receipt)model.GetValue(left, 0);
                var b = (Receipt)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.CreatedAt.CompareTo(b.CreatedAt);
            });
        }

        private void AddNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var a = (Receipt)model.GetValue(left, 0);
                var b = (Receipt)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.RefNo.CompareTo(b.RefNo);
            });
        }

    }
}
