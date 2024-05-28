using LogicPOS.DTOs.Printing;
using LogicPOS.Globalization;
using LogicPOS.Printing.Enums;
using LogicPOS.Printing.Templates;
using System;

namespace LogicPOS.Printing.Documents
{
    public class ThermalPrinterInternalDocumentCashDrawer : ThermalPrinterBaseInternalTemplate
    {
        private readonly decimal _totalAmountInCashDrawer = 0.0m;
        private readonly decimal _movementAmount = 0.0m;
        private readonly string _movementDescription = string.Empty;

        public ThermalPrinterInternalDocumentCashDrawer(
            PrinterReferenceDto printer,
            string pTicketTitle,
            decimal pTotalAmountInCashDrawer)
            : this(
                  printer,
                  pTicketTitle,
                  pTotalAmountInCashDrawer,
                  0.0m)
        { }

        public ThermalPrinterInternalDocumentCashDrawer(
            PrinterReferenceDto printer,
            string pTicketTitle,
            decimal pTotalAmountInCashDrawer,
            decimal pMovementAmount)
            : this(
                  printer,
                  pTicketTitle,
                  pTotalAmountInCashDrawer,
                  pMovementAmount,
                  string.Empty)
        { }

        public ThermalPrinterInternalDocumentCashDrawer(
            PrinterReferenceDto printer,
            string pTicketTitle,
            decimal pTotalAmountInCashDrawer,
            decimal pMovementAmount,
            string pMovementDescription)
            : base(printer)
        {
            _ticketTitle = pTicketTitle;
            _totalAmountInCashDrawer = pTotalAmountInCashDrawer;
            _movementAmount = pMovementAmount;
            _movementDescription = pMovementDescription;
        }

        //Override Parent Template
        public override void PrintContent()
        {
            try
            {
                //Call Base Template PrintHeader
                PrintTitles();

                //Align Center
                _genericThermalPrinter.SetAlignCenter();

                PrintDocumentDetails();

                //Reset to Left
                _genericThermalPrinter.SetAlignLeft();

                //Line Feed
                _genericThermalPrinter.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintDocumentDetails()
        {
            _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(Settings.CultureSettings.CurrentCultureName, "global_total_cashdrawer"));
            _genericThermalPrinter.WriteLine(LogicPOS.Utility.DataConversionUtils.DecimalToString(_totalAmountInCashDrawer), WriteLineTextMode.Big);
            _genericThermalPrinter.LineFeed();

            if (_movementAmount < 0.0m || _movementAmount > 0.0m)
            {
                _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(Settings.CultureSettings.CurrentCultureName, "global_movement_amount"));
                _genericThermalPrinter.WriteLine(LogicPOS.Utility.DataConversionUtils.DecimalToString(_movementAmount), WriteLineTextMode.Big);
                _genericThermalPrinter.LineFeed();
            }

            string description = (_movementDescription != string.Empty) ? _movementDescription : "________________________________";
            _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(Settings.CultureSettings.CurrentCultureName, "global_description"));
            _genericThermalPrinter.WriteLine(description);
        }
    }
}
