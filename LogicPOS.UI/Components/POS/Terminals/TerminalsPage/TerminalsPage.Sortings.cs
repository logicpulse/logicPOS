using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TerminalsPage
    {
        private void AddIsDefaultSorting()
        {
            var sortIndex = Options != null && Options.Count != 0 ? 3 : 2;

            GridViewSettings.Sort.SetSortFunc(sortIndex, (model, left, right) =>
            {
                var leftTerminal = (Terminal)model.GetValue(left, 0);
                var rightTerminal = (Terminal)model.GetValue(right, 0);

                if (leftTerminal == null || rightTerminal == null)
                {
                    return 0;
                }

                return leftTerminal.IsDefault.CompareTo(rightTerminal.IsDefault);
            });
        }

        private void AddHardwareIdSorting()
        {
            var sortIndex = Options != null && Options.Count != 0 ? 4 : 3;

            GridViewSettings.Sort.SetSortFunc(sortIndex, (model, left, right) =>
            {
                var leftTerminal = (Terminal)model.GetValue(left, 0);
                var rightTerminal = (Terminal)model.GetValue(right, 0);

                if (leftTerminal == null || rightTerminal == null)
                {
                    return 0;
                }

                return leftTerminal.HardwareId.CompareTo(rightTerminal.HardwareId);
            });
        }
    }
}
