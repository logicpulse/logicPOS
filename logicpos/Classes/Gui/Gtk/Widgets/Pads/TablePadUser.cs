using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.shared;
using logicpos.shared.App;
using LogicPOS.Shared;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class TablePadUser : TablePad
    {
        public TablePadUser(string pSql, string pOrder, string pFilter, Guid pActiveButtonOid, bool pToggleMode, uint pRows, uint pColumns, string pButtonNamePrefix, Color pColorButton, int pButtonWidth, int pButtonHeight, TouchButtonBase buttonPrev, TouchButtonBase buttonNext)
            : base(pSql, pOrder, pFilter, pActiveButtonOid, pToggleMode, pRows, pColumns, pButtonNamePrefix, pColorButton, pButtonWidth, pButtonHeight, buttonPrev, buttonNext)
        {
        }

        public override TouchButtonBase InitializeButton()
        {
            bool logged = POSSession.CurrentSession.LoggedUsers.ContainsKey(new Guid(_resultRow.Values[_fieldIndex["id"]].ToString()));
            return new TouchButtonUser(_strButtonName, _colorButton, _strButtonLabel, _fontPosBaseButtonSize.ToString(), _buttonWidth, _buttonHeight, logged);
        }
    }
}