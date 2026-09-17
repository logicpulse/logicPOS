using LogicPOS.Api.Features.Finance.Documents.Types.Common;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentTypesPage
    {

        private void AddAcronymSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftType = (DocumentType)model.GetValue(left, 0);
                var rightType = (DocumentType)model.GetValue(right, 0);

                if (leftType == null || rightType == null)
                {
                    return 0;
                }

                return leftType.Acronym.CompareTo(rightType.Acronym);
            });
        }
    }
}
