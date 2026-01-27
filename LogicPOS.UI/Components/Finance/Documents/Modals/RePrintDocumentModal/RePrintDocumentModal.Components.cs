using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class RePrintDocumentModal
    {
        private readonly string _documentNumber;
        public IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        public IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        public TextBox TxtMotive { get; set; }
        public List<int> Copies { get; set; }  = new List<int>();
        private CheckButtonExtended BtnOriginal { get; } = new CheckButtonExtended(LocalizedString.Instance["global_print_copy_title1"]);
        private CheckButtonExtended BtnCopy2 { get; } = new CheckButtonExtended(LocalizedString.Instance["global_print_copy_title2"]);
        private CheckButtonExtended BtnCopy3 { get; } = new CheckButtonExtended(LocalizedString.Instance["global_print_copy_title3"]);
        private CheckButtonExtended BtnCopy4 { get; } = new CheckButtonExtended(LocalizedString.Instance["global_print_copy_title4"]);
        private List<CheckButtonExtended> Buttons { get; set; }
        private CheckButtonBox CheckSecondCopy { get; } = new CheckButtonBox(GeneralUtils.GetResourceByName("global_second_copy"), true);
        public bool SecondPrint => CheckSecondCopy.Active;
        public string Reason => TxtMotive.Text.Trim();
    }
}
