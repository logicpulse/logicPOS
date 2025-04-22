using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.Pages
{
    public partial   class PrinterTypesPage
    {
        private void AddThermalPrinterSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftPrinterType = (PrinterType)model.GetValue(left, 0);
                var rightPrinterType = (PrinterType)model.GetValue(right, 0);

                if (leftPrinterType == null || rightPrinterType == null)
                {
                    return 0;
                }

                return leftPrinterType.ThermalPrinter.CompareTo(rightPrinterType.ThermalPrinter);
            });
        }
    }
}
