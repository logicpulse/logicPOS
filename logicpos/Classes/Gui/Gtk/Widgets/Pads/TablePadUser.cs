using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class TablePadUser : TablePad
    {
        public TablePadUser(String pSql, String pOrder, String pFilter, Guid pActiveButtonOid, bool pToggleMode, uint pRows, uint pColumns, String pButtonNamePrefix, Color pColorButton, int pButtonWidth, int pButtonHeight, TouchButtonBase buttonPrev, TouchButtonBase buttonNext)
            : base(pSql, pOrder, pFilter, pActiveButtonOid, pToggleMode, pRows, pColumns, pButtonNamePrefix, pColorButton, pButtonWidth, pButtonHeight, buttonPrev, buttonNext)
        {
        }

        public override TouchButtonBase InitializeButton()
        {
            bool logged = GlobalFramework.SessionApp.LoggedUsers.ContainsKey(new Guid(_resultRow.Values[_fieldIndex["id"]].ToString()));
            return new TouchButtonUser(_strButtonName, _colorButton, _strButtonLabel, _fontPosBaseButtonSize.ToString(), _buttonWidth, _buttonHeight, logged);
        }
    }
}