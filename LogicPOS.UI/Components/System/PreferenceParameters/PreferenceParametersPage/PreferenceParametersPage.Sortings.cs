using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PreferenceParametersPage
    {
        private void AddDesignationSorting()
        {
            GridViewSettings.Sort.SetSortFunc(0, (model, a, b) =>
            {
                var parameterA = (PreferenceParameter)model.GetValue(a, 0);
                var paramterB = (PreferenceParameter)model.GetValue(b, 0);

                if (parameterA == null || paramterB == null)
                {
                    return 0;
                }

                return parameterA.ResourceStringValue.CompareTo(paramterB.ResourceStringValue);
            });
        }

    }
}
