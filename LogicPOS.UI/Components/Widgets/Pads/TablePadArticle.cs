using LogicPOS.UI.Buttons;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class TablePadArticle : TablePad
    {
        public TablePadArticle(string pSql,
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
            return new ImageButton(
                new ButtonSettings
                {
                    Name = _strButtonName,
                    BackgroundColor = _colorButton,
                    Text = _strButtonLabel,
                    FontSize = _fontPosBaseButtonSize,
                    Image = _strButtonImage,
                    Overlay = _fileBaseButtonOverlay,
                    ButtonSize = new Size(_buttonWidth, _buttonHeight)
                });
        }
    }
}