using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class SendDocumentByEmailModal
    {
        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();
            VBox verticalLayout = new VBox(false, 0);
            verticalLayout.PackStart(TxtSubject.Component, false, false, 0);
            verticalLayout.PackStart(TxtTo.Component, false, false, 0);
            verticalLayout.PackStart(TxtCc.Component, false, false, 0);
            verticalLayout.PackStart(TxtBcc.Component, false, false, 0);
            verticalLayout.PackStart(TxtBody, true, true, 0);

            return verticalLayout;
        }
    }
}
