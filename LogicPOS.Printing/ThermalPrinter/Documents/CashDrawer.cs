using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Enums;
using LogicPOS.Printing.Templates;
using LogicPOS.Utility;
using System;

namespace LogicPOS.Printing.Documents
{
    public class CashDrawer : BaseInternalTemplate
    {
        private readonly decimal _totalAmountInCashDrawer = 0.0m;
        private readonly decimal _movementAmount = 0.0m;
        private readonly string _movementDescription = string.Empty;

        public CashDrawer(
            PrinterDto printer,
            string pTicketTitle,
            decimal pTotalAmountInCashDrawer)
            : this(
                  printer,
                  pTicketTitle,
                  pTotalAmountInCashDrawer,
                  0.0m)
        { }

        public CashDrawer(
            PrinterDto printer,
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

        public CashDrawer(
            PrinterDto printer,
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
                _printer.SetAlignCenter();

                PrintDocumentDetails();

                //Reset to Left
                _printer.SetAlignLeft();

                //Line Feed
                _printer.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void PrintDocumentDetails()
        {
            _printer.WriteLine(GeneralUtils.GetResourceByName("global_total_cashdrawer"));
            _printer.WriteLine(DataConversionUtils.DecimalToString(_totalAmountInCashDrawer), WriteLineTextMode.Big);
            _printer.LineFeed();

            if (_movementAmount < 0.0m || _movementAmount > 0.0m)
            {
                _printer.WriteLine(GeneralUtils.GetResourceByName("global_movement_amount"));
                _printer.WriteLine(DataConversionUtils.DecimalToString(_movementAmount), WriteLineTextMode.Big);
                _printer.LineFeed();
            }

            string description = (_movementDescription != string.Empty) ? _movementDescription : "________________________________";
            _printer.WriteLine(GeneralUtils.GetResourceByName("global_description"));
            _printer.WriteLine(description);
        }
    }
}
