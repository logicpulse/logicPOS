using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class TablePadArticle : TablePad
    {
        public TablePadArticle(string pSql, string pOrder, string pFilter, Guid pActiveButtonOid, bool pToggleMode, uint pRows, uint pColumns, string pButtonNamePrefix, Color pColorButton, int pButtonWidth, int pButtonHeight, TouchButtonBase buttonPrev, TouchButtonBase buttonNext)
            : base(pSql, pOrder, pFilter, pActiveButtonOid, pToggleMode, pRows, pColumns, pButtonNamePrefix, pColorButton, pButtonWidth, pButtonHeight, buttonPrev, buttonNext)
        {
        }

        public override TouchButtonBase InitializeButton()
        {
            //bool logged = GlobalFramework.SessionApp.LoggedUsers.ContainsKey(new Guid(_resultRow.Values[_fieldIndex["id"]].ToString()));
            //return new TouchButtonUser(_strButtonName, _colorButton, _strButtonLabel, _fontPosBaseButtonSize.ToString(), _buttonWidth, _buttonHeight, logged);

            //_logger.Debug(string.Format("FunctionName(): ButtonLabelHide: [{0}]", _resultRow.Values[_fieldIndex["ButtonLabelHide"]]));
            //if () {
            //  _strButtonLabel = string.Empty;
            //}

            return new TouchButtonImage(_strButtonName, _colorButton, _strButtonLabel, _fontPosBaseButtonSize, _strButtonImage, _fileBaseButtonOverlay, _buttonWidth, _buttonHeight);
        }
    }
}