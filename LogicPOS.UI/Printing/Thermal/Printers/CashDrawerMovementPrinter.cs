using ESC_POS_USB_NET.Printer;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Printing
{
    public class CashDrawerMovementPrinter : ThermalPrinter
    {

        private decimal _totalAmountInCashDrawer;
        private decimal _movementAmount;
        private string _movementDescription;
        private readonly WorkSessionMovementType _movementType;

        public CashDrawerMovementPrinter(Printer printer,
                                 decimal totalAmountInCashDrawer,
                                 decimal movementAmount,
                                 WorkSessionMovementType workSessionMovementType,
                                 string movementDescription) : base(printer)
        {
            _totalAmountInCashDrawer = totalAmountInCashDrawer;
            _movementAmount = movementAmount;
            _movementDescription = movementDescription;
            _movementType = workSessionMovementType;
        }

        public override void Print()
        {
            PrintHeader();
            PrintDocumentDetails();
            PrintFooter();
            _printer.FullPaperCut();
            _printer.PrintDocument();
            _printer.Clear();
        }
        
        private void PrintDocumentDetails()
        {
            _printer.AlignCenter();
            if (_movementType == WorkSessionMovementType.CashDrawerOut)
            {
                _printer.DoubleWidth2();
                _printer.BoldMode(GeneralUtils.GetResourceByName("ticket_title_worksession_money_out"));
                _printer.NormalWidth();
                _printer.Separator(' ');

            }
            else
            if (_movementType== WorkSessionMovementType.CashDrawerIn)
            {
                _printer.DoubleWidth2();
                _printer.BoldMode(GeneralUtils.GetResourceByName("ticket_title_worksession_money_in"));
                _printer.NormalWidth();
                _printer.Separator(' ');

            }
            else
            if(_movementType==WorkSessionMovementType.CashDrawerClose)
            {
                _printer.DoubleWidth2();
                _printer.BoldMode(GeneralUtils.GetResourceByName("ticket_title_worksession_terminal_close"));
                _printer.NormalWidth();
                _printer.Separator(' ');
            }
            else
            {
                _printer.DoubleWidth2();
                _printer.BoldMode(GeneralUtils.GetResourceByName("ticket_title_worksession_terminal_open"));
                _printer.NormalWidth();
                _printer.Separator(' ');
            }
            _printer.Append(GeneralUtils.GetResourceByName("global_total_cashdrawer"));
            _printer.Separator(' ');
            _printer.DoubleWidth2();
            _printer.BoldMode(DecimalToMoneyString(_totalAmountInCashDrawer));
            _printer.NormalWidth();
            _printer.Separator(' ');

            if (_movementType==WorkSessionMovementType.CashDrawerIn || _movementType==WorkSessionMovementType.CashDrawerOut)
            {
                _printer.Append(GeneralUtils.GetResourceByName("global_movement_amount"));
                _printer.Separator(' ');
                _printer.DoubleWidth2();
                _printer.BoldMode(DecimalToMoneyString(_movementAmount));
                _printer.Separator(' ');
                _printer.NormalWidth();
            }

            string description = (_movementDescription != string.Empty) ? _movementDescription : "________________________________";
            _printer.Append(GeneralUtils.GetResourceByName("global_description"));
            _printer.Append(description);
            _printer.NewLine();
        }

        private void PrintFooter()
        {
            _printer.Separator(' ');
            _printer.Append(GeneralUtils.GetResourceByName("global_internal_document_footer1"));
            _printer.Append(GeneralUtils.GetResourceByName("global_internal_document_footer2"));
            _printer.Separator(' ');
            _printer.NewLine();
            _printer.Append(GeneralUtils.GetResourceByName("global_internal_document_footer3"));
            _printer.Separator(' ');
            _printer.NewLine();
            _printer.Append(string.Format("{0} - {1}", AuthenticationService.User.Name, TerminalService.Terminal.Designation));
            _printer.NewLine();
            _printer.Append(string.Format("{1}: {2}{0}{3}: {4} {5}"
                , Environment.NewLine
                , GeneralUtils.GetResourceByName("global_printed_on_date")
                , DateTime.Now.ToLocalTime()
                , "LogicPulse"//_customVars["APP_COMPANY"]
                , "LogicPOS"//_customVars["APP_NAME"]
                , "vs1.010.1"//_customVars["APP_VERSION"]
                ));
        }

    }
}
