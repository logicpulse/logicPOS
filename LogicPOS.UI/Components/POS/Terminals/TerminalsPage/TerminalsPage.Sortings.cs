using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.GridViews;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TerminalsPage
    {
        private void AddHardwareIdSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
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
