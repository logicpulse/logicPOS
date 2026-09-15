using ESC_POS_USB_NET.Printer;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using LogicPOS.Globalization;
using LogicPOS.UI.Application.Services;

namespace LogicPOS.UI.Printing
{
    public class CashDrawerMovementPrinter : ThermalPrinter
    {

        private readonly decimal _totalAmountInCashDrawer;
        private readonly decimal _movementAmount;
        private readonly string _movementDescription;
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
            ThermalPrinterTarget.Commit(_printer);
            _printer.Clear();
        }

        private void PrintDocumentDetails()
        {
            _printer.AlignCenter();
            switch (_movementType)
            {
                case WorkSessionMovementType.CashDrawerOut:
                    _printer.DoubleWidth2();
                    AppendBoldLine(_printer, LocalizedString.Instance["ticket_title_worksession_money_out"]);
                    _printer.NormalWidth();
                    BlankSeparator();
                    break;

                case WorkSessionMovementType.CashDrawerIn:
                    _printer.DoubleWidth2();
                    AppendBoldLine(_printer, LocalizedString.Instance["ticket_title_worksession_money_in"]);
                    _printer.NormalWidth();
                    BlankSeparator();
                    break;

                case WorkSessionMovementType.CashDrawerClose:
                    _printer.DoubleWidth2();
                    AppendBoldLine(_printer, LocalizedString.Instance["ticket_title_worksession_terminal_close"]);
                    _printer.NormalWidth();
                    BlankSeparator();
                    break;

                case WorkSessionMovementType.CashDrawerOpen:
                    _printer.DoubleWidth2();
                    AppendBoldLine(_printer, LocalizedString.Instance["ticket_title_worksession_terminal_open"]);
                    _printer.NormalWidth();
                    BlankSeparator();
                    break;
            }

            _printer.Append(ToThermalText(LocalizedString.Instance["global_total_cashdrawer"]));
            BlankSeparator();
            _printer.DoubleWidth2();
            AppendBoldLine(_printer, _totalAmountInCashDrawer.ToString("F2"));
            _printer.NormalWidth();
            BlankSeparator();

            if (_movementType == WorkSessionMovementType.CashDrawerIn || _movementType == WorkSessionMovementType.CashDrawerOut)
            {
                _printer.Append(ToThermalText(LocalizedString.Instance["global_movement_amount"]));
                BlankSeparator();
                _printer.DoubleWidth2();
                AppendBoldLine(_printer, _movementAmount.ToString("F2"));
                BlankSeparator();
                _printer.NormalWidth();
            }

            string description = (_movementDescription != string.Empty) ? _movementDescription : "________________________________";
            _printer.Append(ToThermalText(LocalizedString.Instance["global_description"]));
            _printer.Append(ToThermalText(description));
            _printer.NewLine();
        }

        private void PrintFooter()
        {
            BlankSeparator();
            _printer.Append(LocalizedString.Instance["global_internal_document_footer1"]);
            _printer.Append(LocalizedString.Instance["global_internal_document_footer2"]);
            _printer.Append(LocalizedString.Instance["global_internal_document_footer3"]);
            BlankSeparator();
            _printer.NewLine();
            _printer.Append(string.Format("{0} - {1}", AuthenticationService.User.Name, TerminalService.Terminal.Designation));
            _printer.Append(string.Format("{1}: {2}{0}{3}: {4} {5}"
                , Environment.NewLine
                , LocalizedString.Instance["global_printed_on_date"]
                , DateTime.Now.ToLocalTime()
                , "LogicPulse"
                , "LogicPOS"
                , $"vs {SystemVersionService.ApiVersion}"
                ));
        }

    }
}
