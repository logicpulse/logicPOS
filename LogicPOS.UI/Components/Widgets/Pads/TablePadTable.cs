using LogicPOS.Domain.Enums;
using LogicPOS.UI.Buttons;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class TablePadTable : TablePad
    {
        public TablePadTable(string pSql,
                             string pOrder,
                             string pFilter,
                             Guid pActiveButtonOid,
                             bool pToggleMode,
                             uint pRows,
                             uint pColumns,
                             string pButtonNamePrefix,
                             Color pColorButton,
                             int pButtonWidth,
                             int pButtonHeight,
                             CustomButton buttonPrev,
                             CustomButton buttonNext)
            : base(pSql,
                   pOrder,
                   pFilter,
                   pActiveButtonOid,
                   pToggleMode,
                   pRows,
                   pColumns,
                   pButtonNamePrefix,
                   pColorButton,
                   pButtonWidth,
                   pButtonHeight,
                   buttonPrev,
                   buttonNext)
        {
           
        }

        public override CustomButton InitializeButton()
        {
            string fontTableDialogTableNumber = LogicPOS.Settings.AppSettings.Instance.fontTableDialogTableNumber;

            TableStatus tableStatus = (_resultRow.Values[_fieldIndex["status"]] != null) ? (TableStatus)Convert.ToInt16(_resultRow.Values[_fieldIndex["status"]]) : TableStatus.Free;
            decimal total = (_resultRow.Values[_fieldIndex["total"]] != null) ? Convert.ToDecimal(_resultRow.Values[_fieldIndex["total"]]) : Convert.ToDecimal(0);
            DateTime dateOpen = (_resultRow.Values[_fieldIndex["dateopen"]] != null) ? (DateTime)_resultRow.Values[_fieldIndex["dateopen"]] : new DateTime();
            DateTime dateClosed = (_resultRow.Values[_fieldIndex["dateclosed"]] != null) ? (DateTime)_resultRow.Values[_fieldIndex["dateclosed"]] : new DateTime();

            return new TableButton(
                new TableButtonSettings
                {
                    Name = _strButtonName,
                    BackgroundColor = _colorButton,
                    Text = _strButtonLabel,
                    Font = fontTableDialogTableNumber,
                    ButtonSize = new Size(_buttonWidth, _buttonHeight),
                    TableStatus = tableStatus,
                    Total = total,
                    OpenedAt = dateOpen,
                    ClosedAt = dateClosed
                });
        }
    }
}
