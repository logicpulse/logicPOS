using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WorkSessionsModal
    {

        private IconButtonWithText BtnPrintDay { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "touchButton_Green");
        private IconButtonWithText BtnClose{ get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);

        private TextBox TxtSearch { get; set; }
        public IconButtonWithText BtnFilter { get; set; }

    }
}
