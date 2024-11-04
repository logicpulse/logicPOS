using LogicPOS.Shared;
using LogicPOS.UI.Buttons;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class TablePadUser : TablePad
    {
        public TablePadUser(string pSql,
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
            bool logged = POSSession.CurrentSession.LoggedUsers.ContainsKey(new Guid(_resultRow.Values[_fieldIndex["id"]].ToString()));

            var buttonSettings = new ButtonSettings
            {
                Name = _strButtonName,
                Text = _strButtonLabel,
                Font = _fontPosBaseButtonSize.ToString(),
                Logged = logged,
                ButtonSize = new Size(_buttonWidth, _buttonHeight)
            };

            return new UserButton(buttonSettings);
        }
    }
}