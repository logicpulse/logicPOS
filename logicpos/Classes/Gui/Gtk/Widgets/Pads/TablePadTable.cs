using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.datalayer.Enums;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class TablePadTable : TablePad
    {
        public TablePadTable(string pSql, string pOrder, string pFilter, Guid pActiveButtonOid, bool pToggleMode, uint pRows, uint pColumns, string pButtonNamePrefix, Color pColorButton, int pButtonWidth, int pButtonHeight, TouchButtonBase buttonPrev, TouchButtonBase buttonNext)
            : base(pSql, pOrder, pFilter, pActiveButtonOid, pToggleMode, pRows, pColumns, pButtonNamePrefix, pColorButton, pButtonWidth, pButtonHeight, buttonPrev, buttonNext)
        {
            //_logger.Debug(string.Format("{0} {2} {1}", pSql, pOrder, pFilter));
        }

        public override TouchButtonBase InitializeButton()
        {
            //Settings
            string fontTableDialogTableNumber = LogicPOS.Settings.GeneralSettings.Settings["fontTableDialogTableNumber"];

            TableStatus tableStatus = (_resultRow.Values[_fieldIndex["status"]] != null) ? (TableStatus)Convert.ToInt16(_resultRow.Values[_fieldIndex["status"]]) : TableStatus.Free;
            decimal total = (_resultRow.Values[_fieldIndex["total"]] != null) ? Convert.ToDecimal(_resultRow.Values[_fieldIndex["total"]]) : Convert.ToDecimal(0);
            DateTime dateOpen = (_resultRow.Values[_fieldIndex["dateopen"]] != null) ? (DateTime)_resultRow.Values[_fieldIndex["dateopen"]] : new DateTime();
            DateTime dateClosed = (_resultRow.Values[_fieldIndex["dateclosed"]] != null) ? (DateTime)_resultRow.Values[_fieldIndex["dateclosed"]] : new DateTime();

            return new TouchButtonTable(_strButtonName, _colorButton, _strButtonLabel, fontTableDialogTableNumber, _buttonWidth, _buttonHeight, tableStatus, total, dateOpen, dateClosed);
        }
    }
}
