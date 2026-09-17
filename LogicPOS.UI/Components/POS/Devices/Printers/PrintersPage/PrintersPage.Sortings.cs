using Printer = LogicPOS.Api.Entities.Printer;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PrintersPage : Page<Printer>
    {
        
        private void AddPrinterTypeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftPrinter = (Printer)model.GetValue(left, 0);
                var rightPrinter = (Printer)model.GetValue(right, 0);

                if (leftPrinter == null || rightPrinter == null)
                {
                    return 0;
                }

                return leftPrinter.Type.Designation.CompareTo(rightPrinter.Type.Designation);
            });
        }

    }
}
