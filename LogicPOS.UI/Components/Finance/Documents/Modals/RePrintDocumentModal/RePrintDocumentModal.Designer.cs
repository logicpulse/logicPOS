using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class RePrintDocumentModal
    {
        private void InitializeButtons()
        {
            Buttons = new List<CheckButtonExtended>
            {
                BtnOriginal,
                BtnCopy2,
                BtnCopy3,
                BtnCopy4
            };
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        private void CheckButtonOnly(CheckButtonExtended button)
        {
            foreach (var btn in Buttons)
            {
                if (btn != button)
                {
                    btn.Active = false;
                }
            }
        }

        private void InitializeTxtMotive()
        {
            TxtMotive = new TextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_reprint_original_motive"),
                                       isRequired: false,
                                       isValidatable:true,
                                       regex: "^(?!\\s)[A-Za-z0-9 ]+$",
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtMotive.Component.Sensitive = false;
        }

    }
}
